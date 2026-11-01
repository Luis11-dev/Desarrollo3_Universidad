using Universidad.Models;
using Universidad.ProfesorExceptions;
using Universidad.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Universidad.Services
{
    public class ProfesorService
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly HorarioClaseRepository _horarioRepository;
        private readonly ContratoRepository _contratoRepository;
        private readonly PlanillaService _planillaService;

        public const decimal TarifaPorHoraDefault = 400m;
        public const int MinutosToleranciaAsistencia = 15;

        public ProfesorService(
            ProfesorRepository profesorRepository,
            HorarioClaseRepository horarioRepository,
            ContratoRepository contratoRepository,
            PlanillaService planillaService)
        {
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _contratoRepository = contratoRepository ?? throw new ArgumentNullException(nameof(contratoRepository));
            _planillaService = planillaService ?? throw new ArgumentNullException(nameof(planillaService));
        }

        // Validación general de rol
        public void ValidarRolProfesor(IAutentication usuarioActual)
        {
            if (usuarioActual == null || usuarioActual.Rol != Rol.Profesor)
            {
                throw new AccesoDenegadoProfesorException();
            }
        }

        // Registrar un nuevo profesor en el sistema
        public Profesor RegistrarProfesor(
            string cedula,
            string nombre,
            string correo,
            int numeroCelular,
            string direccion,
            Genero genero,
            string estadoCivil,
            string profesion,
            string? especialidad,
            string user,
            string password)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                throw new ArgumentException("La cedula es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(correo))
            {
                throw new ArgumentException("El correo electronico es obligatorio.");
            }
            if (numeroCelular <= 0)
            {
                throw new ArgumentException("El numero de celular no es valido.");
            }
            if (string.IsNullOrWhiteSpace(direccion))
            {
                throw new ArgumentException("La direccion es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(estadoCivil))
            {
                throw new ArgumentException("El estado civil es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(profesion))
            {
                throw new ArgumentException("La profesion es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("La contrasenia es obligatoria.");
            }

            cedula = cedula.Trim().ToUpperInvariant();
            nombre = nombre.Trim();
            correo = correo.Trim();
            direccion = direccion.Trim();
            estadoCivil = estadoCivil.Trim();
            profesion = profesion.Trim();
            especialidad = string.IsNullOrWhiteSpace(especialidad) ? null : especialidad.Trim();
            user = user.Trim();

            string patronCedula = @"^\d{3}-\d{6}-\d{4}[A-Za-z]$";
            if (!Regex.IsMatch(cedula, patronCedula))
            {
                throw new ArgumentException("El formato de la cedula debe ser 000-000000-0000X.");
            }

            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(correo, patronCorreo))
            {
                throw new ArgumentException("El formato del correo electronico no es valido.");
            }

            if (_profesorRepository.GetByCedula(cedula) != null)
            {
                throw new InvalidOperationException("Ya existe un profesor registrado con esa cedula.");
            }

            if (_profesorRepository.GetByUser(user) != null)
            {
                throw new InvalidOperationException("El nombre de usuario ya esta en uso por otro profesor.");
            }

            bool correoExiste = _profesorRepository.GetAll()
                .Any(p => p.CorreoElectronico.Equals(correo, StringComparison.OrdinalIgnoreCase));
            if (correoExiste)
            {
                throw new InvalidOperationException("El correo electronico ya esta registrado.");
            }

            Profesor profesor = new Profesor(
                cedula,
                nombre,
                correo,
                numeroCelular,
                direccion,
                genero,
                estadoCivil,
                profesion,
                especialidad,
                user,
                password
            );

            _profesorRepository.Add(profesor);
            return profesor;
        }

        public List<Profesor> ObtenerProfesores()
        {
            return _profesorRepository.GetAll();
        }

        public List<HorarioClase> ObtenerMateriasAsignadas(Profesor profesor)
        {
            ValidarRolProfesor(profesor);
            return _horarioRepository.GetByProfesor(profesor.Cedula);
        }

        public List<HorarioClase> ObtenerHorariosAsignados(Profesor profesor)
        {
            return ObtenerMateriasAsignadas(profesor);
        }

        public List<Grupo> ObtenerGruposAsignados(Profesor profesor)
        {
            ValidarRolProfesor(profesor);
            return _horarioRepository.GetByProfesor(profesor.Cedula)
                .Select(h => h.Grupo)
                .GroupBy(g => g.IdGrupo)
                .Select(g => g.First())
                .ToList();
        }

        public Contrato? ObtenerContratoVigente(Profesor profesor)
        {
            ValidarRolProfesor(profesor);
            return _contratoRepository.GetContratoActivo(profesor.Cedula);
        }

        public PlanillaProfesor ConsultarPlanilla(Profesor profesor)
        {
            ValidarRolProfesor(profesor);
            return _planillaService.CalcularPlanillaProfesor(profesor);
        }

        public PlanillaProfesor ConsultarContratoYPlanilla(Profesor profesor)
        {
            return ConsultarPlanilla(profesor);
        }

        public HorarioClase ObtenerYValidarHorarioDelProfesor(Profesor profesor, string idHorario)
        {
            ValidarRolProfesor(profesor);

            HorarioClase? horario = _horarioRepository.GetAll()
                .FirstOrDefault(h => h.IdHorario.Equals(idHorario, StringComparison.OrdinalIgnoreCase));

            if (horario == null)
            {
                throw new KeyNotFoundException($"No se encontro la materia con codigo de horario '{idHorario}'.");
            }

            if (!horario.Profesor.Cedula.Equals(profesor.Cedula, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Acceso denegado: Solo puede gestionar materias que tiene asignadas.");
            }

            return horario;
        }

        public Grupo ObtenerYValidarGrupoDelProfesor(Profesor profesor, string idGrupo)
        {
            ValidarRolProfesor(profesor);

            Grupo? grupo = _profesorRepository.GetGrupoById(idGrupo);
            if (grupo == null)
            {
                throw new KeyNotFoundException($"No se encontro el grupo con codigo '{idGrupo}'.");
            }

            bool asignado = _horarioRepository.GetByProfesor(profesor.Cedula)
                .Any(h => h.Grupo.IdGrupo.Equals(idGrupo, StringComparison.OrdinalIgnoreCase));

            if (!asignado)
            {
                throw new GrupoNoAsignadoException();
            }

            return grupo;
        }

        public IReadOnlyCollection<PerfilUniversitario> VerEstudiantesDeGrupo(Profesor profesor, string idGrupo)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            return grupo.Estudiantes;
        }

        // 2. Ver las notas de los estudiantes de su grupo + Resumen
        public (List<Calificacion> Calificaciones, int Aprobados, int ReprobadosConConvocatoria, int ReprobadosSinDerecho) VerNotasDeGrupo(Profesor profesor, string idGrupo)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            List<Calificacion> calificaciones = _profesorRepository.GetCalificacionesByGrupo(grupo.IdGrupo);

            int aprobados = calificaciones.Count(c => c.Estado == EstadoCalificacion.Aprobado);
            int conConvocatoria = calificaciones.Count(c => c.Estado == EstadoCalificacion.ReprobadoConConvocatoria);
            int sinDerecho = calificaciones.Count(c => c.Estado == EstadoCalificacion.ReprobadoSinConvocatoria);

            return (calificaciones, aprobados, conConvocatoria, sinDerecho);
        }

        // Validación de fecha tope para notas y convocatoria
        public void ValidarFechaTope()
        {
            if (_profesorRepository.FechaTopeNotas.HasValue && DateTime.Now > _profesorRepository.FechaTopeNotas.Value)
            {
                throw new FechaTopeVencidaException(_profesorRepository.FechaTopeNotas.Value);
            }
        }

        // Lista los estudiantes inscritos en el grupo que todavía no tienen nota
        public List<Calificacion> ObtenerEstudiantesSinNota(Profesor profesor, string idGrupo)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            ValidarFechaTope();

            return _profesorRepository
                .GetCalificacionesByGrupo(grupo.IdGrupo)
                .Where(c => c.Nota == null && c.Estado == EstadoCalificacion.Pendiente)
                .ToList();
        }

        // Precalcula la nota final y el estado antes de guardar para mostrar al profesor y pedir confirmación
        public (int NotaFinal, EstadoCalificacion Estado) PrecalcularNota(int acumulado, int examen)
        {
            ValidarRangoNotas(acumulado, examen);
            Calificacion temporal = new Calificacion();
            int notaFinal = temporal.CalcularNota(acumulado, examen);
            EstadoCalificacion estado = temporal.DeterminarEstado(notaFinal);
            return (notaFinal, estado);
        }

        private void ValidarRangoNotas(int acumulado, int examen)
        {
            if (acumulado < 0 || acumulado > 60)
            {
                throw new NotaFueraDeRangoException("Acumulado", 0, 60);
            }

            if (examen < 0 || examen > 40)
            {
                throw new NotaFueraDeRangoException("Examen", 0, 40);
            }
        }

        // 3. Registrar las notas de los estudiantes
        public Calificacion RegistrarNotaEstudiante(Profesor profesor, string idGrupo, string cedulaEstudiante, int acumulado, int examen)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            ValidarFechaTope();
            ValidarRangoNotas(acumulado, examen);

            Calificacion? calificacion = _profesorRepository.GetCalificacion(grupo.IdGrupo, cedulaEstudiante);
            if (calificacion == null)
            {
                throw new KeyNotFoundException("El estudiante indicado no se encuentra inscrito en este grupo.");
            }

            if (calificacion.Nota != null || calificacion.Estado != EstadoCalificacion.Pendiente)
            {
                throw new CalificacionYaRegistradaException(calificacion.Estudiante?.Nombre ?? "Estudiante");
            }

            calificacion.RegistrarNotas(acumulado, examen);
            return calificacion;
        }

        // Verifica si el grupo tiene estudiantes con derecho a convocatoria pendientes de realizarla
        public List<Calificacion> ObtenerEstudiantesConDerechoAConvocatoria(Profesor profesor, string idGrupo)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);

            return _profesorRepository
                .GetCalificacionesByGrupo(grupo.IdGrupo)
                .Where(c => c.Estado == EstadoCalificacion.ReprobadoConConvocatoria && c.NotaConvocatoria == null)
                .ToList();
        }

        // 4. Registrar la nota extraordinaria de los estudiantes con derecho a convocatoria
        public Calificacion RegistrarNotaConvocatoria(Profesor profesor, string idGrupo, string cedulaEstudiante, int notaExtraordinaria)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            ValidarFechaTope();

            if (notaExtraordinaria < 0 || notaExtraordinaria > 100)
            {
                throw new NotaFueraDeRangoException("Convocatoria", 0, 100);
            }

            Calificacion? calificacion = _profesorRepository.GetCalificacion(grupo.IdGrupo, cedulaEstudiante);
            if (calificacion == null)
            {
                throw new KeyNotFoundException("El estudiante indicado no se encuentra inscrito en este grupo.");
            }

            if (calificacion.NotaConvocatoria != null)
            {
                throw new ConvocatoriaNoPermitidaException("El estudiante ya realizo su unica convocatoria para esta clase y no se puede editar.");
            }

            if (calificacion.Estado != EstadoCalificacion.ReprobadoConConvocatoria)
            {
                throw new ConvocatoriaNoPermitidaException("El estudiante no tiene derecho a convocatoria (su nota final debe estar entre 40 y 59).");
            }

            calificacion.RegistrarConvocatoria(notaExtraordinaria);
            return calificacion;
        }

        public AsistenciaProfesor MarcarAsistencia(Profesor profesor, string idHorario, DateTime fecha, TimeSpan horaEntrada)
        {
            HorarioClase horario = ObtenerYValidarHorarioDelProfesor(profesor, idHorario);

            TimeSpan horaInicio = horario.HoraInicio;
            TimeSpan horaLimite = horaInicio.Add(TimeSpan.FromMinutes(MinutosToleranciaAsistencia));

            if (horaEntrada < horaInicio)
            {
                throw new AsistenciaFueraDeTiempoException(
                    $"Aun no inicia la clase. La hora de inicio es {horaInicio:hh\\:mm}.");
            }

            if (horaEntrada > horaLimite)
            {
                throw new AsistenciaFueraDeTiempoException(
                    $"Tiempo limite excedido: La clase inicia a las {horaInicio:hh\\:mm} y el limite de 15 minutos vencio a las {horaLimite:hh\\:mm}. Ya no puede marcar asistencia.");
            }

            AsistenciaProfesor asistencia = new AsistenciaProfesor(profesor, horario, fecha, horaEntrada, asistio: true);
            _profesorRepository.AddAsistencia(asistencia);
            return asistencia;
        }

        public AsistenciaProfesor RegistrarAusencia(Profesor profesor, string idHorario, DateTime fecha)
        {
            HorarioClase horario = ObtenerYValidarHorarioDelProfesor(profesor, idHorario);
            AsistenciaProfesor ausencia = new AsistenciaProfesor(profesor, horario, fecha, horaEntrada: null, asistio: false);
            _profesorRepository.AddAsistencia(ausencia);
            return ausencia;
        }
    }
}
