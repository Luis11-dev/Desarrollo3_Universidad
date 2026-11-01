using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Universidad.Data;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Util;

namespace Universidad.Commands.Administrador
{
    public class AsignarHorarioClaseCommand : ICommand
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly HorarioClaseRepository _horarioRepository;
        private readonly PlanAcademicoRepository _planRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly IConsoleWriter _writer;

        public AsignarHorarioClaseCommand(
            ProfesorRepository profesorRepository,
            HorarioClaseRepository horarioRepository,
            PlanAcademicoRepository planRepository,
            MateriaRepository materiaRepository,
            IConsoleWriter writer)
        {
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _materiaRepository = materiaRepository ?? throw new ArgumentNullException(nameof(materiaRepository));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("ASIGNACION DE MATERIA Y HORARIO A PROFESOR");

            try
            {
                // 1. Seleccionar Carrera
                List<Carrera> carreras = Seeds.Carreras;
                string[] opcionesCarreras = carreras.Select(c => c.Nombre).ToArray();
                _writer.DisplayMenu("Seleccione la Carrera:", opcionesCarreras);

                string inputCarrera = new Validator("Ingrese opcion de carrera: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, carreras.Count)
                    .ReadAndValidate();
                Carrera carreraSeleccionada = carreras[int.Parse(inputCarrera) - 1];

                // 2. Seleccionar Año Académico
                List<AnioAcademico> anios = Seeds.AniosAcademicos;
                string[] opcionesAnios = anios.Select(a => a.Nombre).ToArray();
                _writer.DisplayMenu("Seleccione el Año Academico:", opcionesAnios);

                string inputAnio = new Validator("Ingrese opcion de año academico: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, anios.Count)
                    .ReadAndValidate();
                AnioAcademico anioSeleccionado = anios[int.Parse(inputAnio) - 1];

                // 3. Seleccionar Semestre
                List<Semestre> semestres = Seeds.Semestres;
                string[] opcionesSemestres = semestres.Select(s => s.Nombre).ToArray();
                _writer.DisplayMenu("Seleccione el Semestre:", opcionesSemestres);

                string inputSemestre = new Validator("Ingrese opcion de semestre: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, semestres.Count)
                    .ReadAndValidate();
                Semestre semestreSeleccionado = semestres[int.Parse(inputSemestre) - 1];

                // 4. Validar materias disponibles en el Plan Académico
                PlanAcademico? plan = _planRepository.GetByCarreraAnioSemestre(
                    carreraSeleccionada.IdCarrera,
                    anioSeleccionado.IdAnioAcademico,
                    semestreSeleccionado.IdSemestre
                );

                if (plan == null || plan.CodigosMaterias.Count == 0)
                {
                    _writer.ShowError($"\nNo hay materias configuradas en el plan academico para {carreraSeleccionada.Nombre} - {anioSeleccionado.Nombre} ({semestreSeleccionado.Nombre}).");
                    _writer.WaitForKey();
                    return;
                }

                // Cargar instancias de Materia desde MateriaRepository
                List<Materia> materiasDisponibles = plan.CodigosMaterias
                    .Select(cod => _materiaRepository.ObtenerPorCodigo(cod))
                    .Where(m => m != null)
                    .Select(m => m!)
                    .ToList();

                if (materiasDisponibles.Count == 0)
                {
                    _writer.ShowError("\nNo se encontraron materias registradas en el repositorio para los codigos del plan academico.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesMaterias = materiasDisponibles.Select(m => $"{m.Codigo} - {m.Nombre}").ToArray();
                _writer.DisplayMenu("Seleccione la Materia a impartir:", opcionesMaterias);

                string inputMateria = new Validator("Ingrese opcion de materia: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, materiasDisponibles.Count)
                    .ReadAndValidate();
                Materia materiaSeleccionada = materiasDisponibles[int.Parse(inputMateria) - 1];

                // 5. Seleccionar Grupo
                List<Grupo> todosGrupos = _profesorRepository.GetAllGrupos();
                List<Grupo> gruposCandidatos = todosGrupos
                    .Where(g => g.Carrera.Equals(carreraSeleccionada.Nombre, StringComparison.OrdinalIgnoreCase) ||
                                g.AnioAcademico == anioSeleccionado.IdAnioAcademico)
                    .ToList();

                if (gruposCandidatos.Count == 0)
                {
                    gruposCandidatos = todosGrupos;
                }

                if (gruposCandidatos.Count == 0)
                {
                    _writer.ShowError("\nNo hay grupos de estudiantes registrados en el sistema.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesGrupos = gruposCandidatos.Select(g => $"{g.NombreGrupo} ({g.Carrera} - Año {g.AnioAcademico})").ToArray();
                _writer.DisplayMenu("Seleccione el Grupo de estudiantes:", opcionesGrupos);

                string inputGrupo = new Validator("Ingrese opcion de grupo: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, gruposCandidatos.Count)
                    .ReadAndValidate();
                Grupo grupoSeleccionado = gruposCandidatos[int.Parse(inputGrupo) - 1];

                // 6. Seleccionar Profesor
                List<Models.Profesor> profesores = _profesorRepository.GetAll();
                if (profesores.Count == 0)
                {
                    _writer.ShowError("\nNo hay profesores registrados en el sistema.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesProfesores = profesores.Select(p => $"{p.Nombre} (Cedula: {p.Cedula} - {p.Profesion})").ToArray();
                _writer.DisplayMenu("Seleccione el Profesor a asignar:", opcionesProfesores);

                string inputProfesor = new Validator("Ingrese opcion de profesor: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, profesores.Count)
                    .ReadAndValidate();
                Models.Profesor profesorSeleccionado = profesores[int.Parse(inputProfesor) - 1];

                // 7. Seleccionar Día
                string[] diasSemana = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" };
                _writer.DisplayMenu("Seleccione el dia de la clase:", diasSemana);

                string inputDia = new Validator("Ingrese opcion de dia (1-7): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 7)
                    .ReadAndValidate();
                string diaSeleccionado = diasSemana[int.Parse(inputDia) - 1];

                // 8. Ingresar Horas
                TimeSpan horaInicio;
                TimeSpan horaFin;
                string[] formatosHora = { @"hh\:mm", @"h\:mm" };

                while (true)
                {
                    string inputInicio = new Validator("Ingrese hora de inicio (HH:mm, ej. 08:00): ")
                        .NotEmpty("La hora de inicio es requerida.")
                        .ReadAndValidate();

                    if (!TimeSpan.TryParseExact(inputInicio, formatosHora, CultureInfo.InvariantCulture, out horaInicio))
                    {
                        _writer.ShowError("Formato de hora de inicio invalido. Use HH:mm.");
                        continue;
                    }

                    string inputFin = new Validator("Ingrese hora de finalizacion (HH:mm, ej. 11:00): ")
                        .NotEmpty("La hora de fin es requerida.")
                        .ReadAndValidate();

                    if (!TimeSpan.TryParseExact(inputFin, formatosHora, CultureInfo.InvariantCulture, out horaFin))
                    {
                        _writer.ShowError("Formato de hora de fin invalido. Use HH:mm.");
                        continue;
                    }

                    if (horaFin <= horaInicio)
                    {
                        _writer.ShowError("La hora de fin debe ser posterior a la hora de inicio.");
                        continue;
                    }

                    break;
                }

                // 9. Modalidad
                Modalidad modalidad = diaSeleccionado.Equals("Domingo", StringComparison.OrdinalIgnoreCase) 
                    ? Modalidad.Dominical 
                    : Modalidad.Regular;

                // 10. Validar Cruces de Horario
                if (_horarioRepository.ExisteCruceProfesor(profesorSeleccionado.Cedula, diaSeleccionado, horaInicio, horaFin))
                {
                    _writer.ShowError($"\nError de cruce: El profesor {profesorSeleccionado.Nombre} ya tiene una clase asignada el {diaSeleccionado} en el horario de {horaInicio:hh\\:mm} a {horaFin:hh\\:mm}.");
                    _writer.WaitForKey();
                    return;
                }

                if (_horarioRepository.ExisteCruceGrupo(grupoSeleccionado.IdGrupo, diaSeleccionado, horaInicio, horaFin))
                {
                    _writer.ShowError($"\nError de cruce: El grupo {grupoSeleccionado.NombreGrupo} ya tiene una clase asignada el {diaSeleccionado} en el horario de {horaInicio:hh\\:mm} a {horaFin:hh\\:mm}.");
                    _writer.WaitForKey();
                    return;
                }

                // 11. Crear HorarioClase y Guardar
                string idHorario = "H" + (_horarioRepository.GetAll().Count + 1).ToString("D2");
                HorarioClase nuevoHorario = new HorarioClase(
                    idHorario,
                    grupoSeleccionado,
                    materiaSeleccionada,
                    profesorSeleccionado,
                    diaSeleccionado,
                    horaInicio,
                    horaFin,
                    modalidad
                );

                // Inscribir a los estudiantes del grupo que correspondan
                foreach (PerfilUniversitario estudiantePerfil in grupoSeleccionado.Estudiantes)
                {
                    if (estudiantePerfil is Estudiante estudiante)
                    {
                        nuevoHorario.InscribirEstudiante(estudiante);
                    }
                }

                _horarioRepository.Add(nuevoHorario);

                _writer.ShowMessage("\nAsignacion de horario guardada exitosamente.");
                _writer.ShowMessage($"ID Horario: {nuevoHorario.IdHorario}");
                _writer.ShowMessage($"Clase: {materiaSeleccionada.Nombre} ({materiaSeleccionada.Codigo})");
                _writer.ShowMessage($"Grupo: {grupoSeleccionado.NombreGrupo} | Carrera: {carreraSeleccionada.Nombre}");
                _writer.ShowMessage($"Profesor: {profesorSeleccionado.Nombre}");
                _writer.ShowMessage($"Horario: {diaSeleccionado} de {horaInicio:hh\\:mm} a {horaFin:hh\\:mm} ({modalidad})");
                _writer.ShowMessage($"Carga horaria: {nuevoHorario.HorasPorSesion}h por sesion | {nuevoHorario.HorasMensuales}h mensuales");
                _writer.ShowMessage($"Estudiantes formalmente inscritos: {nuevoHorario.EstudiantesInscritos.Count}\n");
                _writer.WaitForKey();
            }
            catch (Exception ex)
            {
                _writer.ShowError($"\nError en la asignacion de horario: {ex.Message}");
                _writer.WaitForKey();
            }
        }

        public void Undo()
        {
            throw new NotSupportedException("Deshacer no soportado para la asignacion de horario.");
        }
    }
}
