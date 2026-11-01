using Universidad.Models;
using Universidad.ProfesorExceptions;
using Universidad.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Services
{
    public class ProfesorService
    {
        private readonly ProfesorRepository _profesorRepository;
        public const decimal TarifaPorHora = 400m;
        public const int MinutosToleranciaAsistencia = 15;

        public ProfesorService(ProfesorRepository profesorRepository)
        {
            _profesorRepository = profesorRepository;
        }

        // Validación general de rol
        public void ValidarRolProfesor(IAutentication usuarioActual)
        {
            if (usuarioActual == null || usuarioActual.Rol != Rol.Profesor)
            {
                throw new AccesoDenegadoProfesorException();
            }
        }

        // Obtener los grupos asignados únicamente a este profesor
        public List<Grupo> ObtenerGruposAsignados(Profesor profesor)
        {
            ValidarRolProfesor(profesor);
            return _profesorRepository.GetGruposByProfesor(profesor.Cedula);
        }

        // Valida que el grupo exista y pertenezca al profesor que consulta
        private Grupo ObtenerYValidarGrupoDelProfesor(Profesor profesor, string idGrupo)
        {
            ValidarRolProfesor(profesor);

            Grupo? grupo = _profesorRepository.GetGrupoById(idGrupo);
            if (grupo == null)
            {
                throw new KeyNotFoundException($"No se encontro el grupo con codigo '{idGrupo}'.");
            }

            if (grupo.ProfesorAsignado == null || grupo.ProfesorAsignado.Cedula != profesor.Cedula)
            {
                throw new GrupoNoAsignadoException();
            }

            return grupo;
        }

        // 1. Ver los estudiantes que pertenecen a su grupo
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

        // 5. Marcar su asistencia a cada clase (15 minutos hábiles después de la hora de inicio)
        public AsistenciaProfesor MarcarAsistencia(Profesor profesor, string idGrupo, DateTime fecha, TimeSpan horaEntrada)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);

            TimeSpan horaInicio = grupo.HoraInicio;
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

            AsistenciaProfesor asistencia = new AsistenciaProfesor(profesor, grupo, fecha, horaEntrada, asistio: true);
            _profesorRepository.AddAsistencia(asistencia);
            return asistencia;
        }

        // Si el profesor no asiste a una clase, se registra como ausencia
        public AsistenciaProfesor RegistrarAusencia(Profesor profesor, string idGrupo, DateTime fecha)
        {
            Grupo grupo = ObtenerYValidarGrupoDelProfesor(profesor, idGrupo);
            AsistenciaProfesor ausencia = new AsistenciaProfesor(profesor, grupo, fecha, horaEntrada: null, asistio: false);
            _profesorRepository.AddAsistencia(ausencia);
            return ausencia;
        }

        // 6. Consultar su contrato y su planilla de pago
        public (List<Grupo> GruposContratados, decimal SalarioBaseTotal, int CantidadAusencias, int HorasDescontadas, decimal DescuentoTotal, decimal SalarioNeto) ConsultarContratoYPlanilla(Profesor profesor)
        {
            ValidarRolProfesor(profesor);

            List<Grupo> grupos = _profesorRepository.GetGruposByProfesor(profesor.Cedula);
            decimal salarioBaseTotal = grupos.Sum(g => g.ObtenerSalarioContrato());

            List<AsistenciaProfesor> ausencias = _profesorRepository
                .GetAsistenciasByProfesor(profesor.Cedula)
                .Where(a => !a.Asistio)
                .ToList();

            int cantidadAusencias = ausencias.Count;
            int horasDescontadas = ausencias.Sum(a => a.Grupo.ObtenerHorasPorSesion());
            decimal descuentoTotal = horasDescontadas * TarifaPorHora;
            decimal salarioNeto = Math.Max(0m, salarioBaseTotal - descuentoTotal);

            return (grupos, salarioBaseTotal, cantidadAusencias, horasDescontadas, descuentoTotal, salarioNeto);
        }
    }
}
