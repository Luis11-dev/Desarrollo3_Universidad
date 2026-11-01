using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class InscripcionRepository
    {
        private readonly List<Inscripcion> _inscripciones = new();
        private readonly MateriaRepository _materiaRepository;
        private readonly ProfesorRepository _profesorRepository;

        public InscripcionRepository(
            MateriaRepository materiaRepository,
            ProfesorRepository profesorRepository)
        {
            _materiaRepository = materiaRepository ?? throw new ArgumentNullException(nameof(materiaRepository));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
        }

        public List<Inscripcion> ObtenerTodas()
        {
            return _inscripciones.ToList();
        }

        public List<Inscripcion> ObtenerPorEstudiante(string cedula)
        {
            return _inscripciones
                .Where(i => i.Estudiante.Cedula.Equals(cedula, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Inscripcion> ObtenerPorPeriodo(string periodo)
        {
            return _inscripciones
                .Where(i => i.PeriodoAcademico.Equals(periodo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Inscripcion> ObtenerPorHorario(string idHorario)
        {
            return _inscripciones
                .Where(i => i.Horario.IdHorario.Equals(idHorario, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void Inscribir(
            Estudiante estudiante,
            HorarioClase horario,
            string periodoAcademico)
        {
            if (estudiante == null)
            {
                throw new ArgumentNullException(nameof(estudiante));
            }

            if (horario == null)
            {
                throw new ArgumentNullException(nameof(horario));
            }

            if (!estudiante.Activo)
            {
                throw new InvalidOperationException("El estudiante esta inactivo y no puede inscribir materias.");
            }

            if (string.IsNullOrWhiteSpace(periodoAcademico))
            {
                throw new InvalidOperationException("Debe indicar el periodo academico para la inscripcion.");
            }

            ValidarPrerrequisitos(estudiante, horario.Materia);
            ValidarMateriaAprobada(estudiante, horario.Materia);
            ValidarInscripcionDuplicada(estudiante, horario.Materia, periodoAcademico);
            ValidarCupo(horario);
            ValidarCruceHorario(estudiante, horario, periodoAcademico);

            string id = "INS-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

            Inscripcion inscripcion = new Inscripcion(
                id,
                estudiante,
                horario,
                periodoAcademico);

            _inscripciones.Add(inscripcion);
            horario.InscribirEstudiante(estudiante);
            horario.Grupo.AgregarEstudiante(estudiante);

            if (_profesorRepository.GetCalificacion(horario.Grupo.IdGrupo, estudiante.Cedula) == null)
            {
                _profesorRepository.AddCalificacion(new Calificacion(estudiante, horario.Grupo, horario.Materia));
            }
        }

        private void ValidarPrerrequisitos(Estudiante estudiante, Materia materia)
        {
            if (materia.Prerrequisitos == null || materia.Prerrequisitos.Count == 0)
            {
                return;
            }

            List<Calificacion> calificaciones = _profesorRepository.GetCalificacionesByEstudiante(estudiante.Cedula);

            foreach (string codigo in materia.Prerrequisitos)
            {
                Materia? prerrequisito = _materiaRepository.ObtenerPorCodigo(codigo);
                string nombrePrerreq = prerrequisito != null ? prerrequisito.Nombre : codigo;

                bool aprobado = calificaciones.Any(c =>
                    c.Estado == EstadoCalificacion.Aprobado &&
                    c.Materia != null &&
                    (c.Materia.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase) ||
                     c.Materia.Nombre.Equals(nombrePrerreq, StringComparison.OrdinalIgnoreCase)));

                if (!aprobado)
                {
                    throw new InvalidOperationException($"No cumple con los prerrequisitos. Debe aprobar primero: {nombrePrerreq} ({codigo}).");
                }
            }
        }

        private void ValidarMateriaAprobada(Estudiante estudiante, Materia materia)
        {
            List<Calificacion> calificaciones = _profesorRepository.GetCalificacionesByEstudiante(estudiante.Cedula);

            bool yaAprobada = calificaciones.Any(c =>
                c.Estado == EstadoCalificacion.Aprobado &&
                c.Materia != null &&
                (c.Materia.Codigo.Equals(materia.Codigo, StringComparison.OrdinalIgnoreCase) ||
                 c.Materia.Nombre.Equals(materia.Nombre, StringComparison.OrdinalIgnoreCase)));

            if (yaAprobada)
            {
                throw new InvalidOperationException($"El estudiante ya aprobo la materia '{materia.Nombre}'. No es posible cursarla nuevamente.");
            }
        }

        private void ValidarInscripcionDuplicada(Estudiante estudiante, Materia materia, string periodo)
        {
            bool existe = _inscripciones.Any(i =>
                i.Estudiante.Cedula.Equals(estudiante.Cedula, StringComparison.OrdinalIgnoreCase) &&
                i.Materia.Codigo.Equals(materia.Codigo, StringComparison.OrdinalIgnoreCase) &&
                i.PeriodoAcademico.Equals(periodo, StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                throw new InvalidOperationException($"El estudiante ya se encuentra inscrito en la materia '{materia.Nombre}' para el periodo {periodo}.");
            }
        }

        private void ValidarCupo(HorarioClase horario)
        {
            if (horario.EstudiantesInscritos.Count >= horario.Grupo.Cupos)
            {
                throw new InvalidOperationException($"El grupo {horario.Grupo.NombreGrupo} ya no tiene cupos disponibles (Capacidad: {horario.Grupo.Cupos}).");
            }
        }

        private void ValidarCruceHorario(Estudiante estudiante, HorarioClase horario, string periodo)
        {
            bool existeCruce = _inscripciones.Any(i =>
                i.Estudiante.Cedula.Equals(estudiante.Cedula, StringComparison.OrdinalIgnoreCase) &&
                i.PeriodoAcademico.Equals(periodo, StringComparison.OrdinalIgnoreCase) &&
                i.Horario.Dia.Equals(horario.Dia, StringComparison.OrdinalIgnoreCase) &&
                horario.HoraInicio < i.Horario.HoraFin &&
                horario.HoraFin > i.Horario.HoraInicio);

            if (existeCruce)
            {
                throw new InvalidOperationException($"Existe un cruce de horario el dia {horario.Dia} entre {horario.HoraInicio:hh\\:mm}-{horario.HoraFin:hh\\:mm} con otra materia ya inscrita.");
            }
        }
    }
}
