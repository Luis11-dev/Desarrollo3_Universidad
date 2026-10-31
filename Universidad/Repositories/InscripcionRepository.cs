
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
            _materiaRepository = materiaRepository;
            _profesorRepository = profesorRepository;
        }

        public List<Inscripcion> ObtenerTodas()
        {
            return _inscripciones.ToList();
        }

        public List<Inscripcion> ObtenerPorEstudiante(string cedula)
        {
            return _inscripciones
                .Where(i => i.Estudiante.Cedula == cedula)
                .ToList();
        }

        public List<Inscripcion> ObtenerPorPeriodo(string periodo)
        {
            return _inscripciones
                .Where(i => i.PeriodoAcademico.Equals(
                    periodo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void Inscribir(
            Estudiante estudiante,
            Grupo grupo,
            string periodoAcademico)
        {
            if (estudiante == null)
                throw new ArgumentNullException(nameof(estudiante));

            if (grupo == null)
                throw new ArgumentNullException(nameof(grupo));

            if (!estudiante.Activo)
                throw new InvalidOperationException(
                    "El estudiante está inactivo.");

            if (string.IsNullOrWhiteSpace(periodoAcademico))
                throw new InvalidOperationException(
                    "Debe indicar el período académico.");

            if (_profesorRepository.GetGrupoById(grupo.IdGrupo) == null)
                throw new InvalidOperationException(
                    "El grupo no está registrado.");

            Materia materia = BuscarMateria(grupo);

            ValidarPrerrequisitos(estudiante, materia);

            ValidarMateriaAprobada(estudiante, materia);

            ValidarInscripcionDuplicada(
                estudiante, grupo, periodoAcademico);

            ValidarCupo(grupo);

            ValidarCruceHorario(
                estudiante, grupo, periodoAcademico);

            string id = Guid.NewGuid().ToString("N");

            Inscripcion inscripcion = new Inscripcion(
                id,
                estudiante,
                grupo,
                periodoAcademico);

            _inscripciones.Add(inscripcion);
            grupo.AgregarEstudiante(estudiante);
        }

        private Materia BuscarMateria(Grupo grupo)
        {
            Materia? materia = _materiaRepository.ObtenerPorCodigo(
                grupo.NombreMateria);

            if (materia == null)
            {
                materia = _materiaRepository.ObtenerTodas()
                    .FirstOrDefault(m =>
                        m.Nombre.Equals(
                            grupo.NombreMateria,
                            StringComparison.OrdinalIgnoreCase));
            }

            if (materia == null)
                throw new InvalidOperationException(
                    $"No se encontró la materia asociada al grupo: {grupo.NombreMateria}.");

            return materia;
        }

        private void ValidarPrerrequisitos(
            Estudiante estudiante,
            Materia materia)
        {
            foreach (string codigo in materia.Prerrequisitos)
            {
                Materia? prerrequisito =
                    _materiaRepository.ObtenerPorCodigo(codigo);

                if (prerrequisito == null)
                    throw new InvalidOperationException(
                        $"No existe el prerrequisito {codigo}.");

                bool aprobado = ObtenerCalificaciones(estudiante)
                    .Any(c =>
                        c.Grupo != null &&
                        c.Grupo.NombreMateria.Equals(
                            prerrequisito.Nombre,
                            StringComparison.OrdinalIgnoreCase) &&
                        c.Estado == EstadoCalificacion.Aprobado);

                if (!aprobado)
                    throw new InvalidOperationException(
                        $"No puede inscribirse. Primero debe aprobar: {prerrequisito.Nombre}.");
            }
        }

        private void ValidarMateriaAprobada(
            Estudiante estudiante,
            Materia materia)
        {
            bool aprobada = ObtenerCalificaciones(estudiante)
                .Any(c =>
                    c.Grupo != null &&
                    c.Grupo.NombreMateria.Equals(
                        materia.Nombre,
                        StringComparison.OrdinalIgnoreCase) &&
                    c.Estado == EstadoCalificacion.Aprobado);

            if (aprobada)
                throw new InvalidOperationException(
                    "El estudiante ya aprobó esta materia.");
        }

        private List<Calificacion> ObtenerCalificaciones(
            Estudiante estudiante)
        {
            return _profesorRepository.GetAllGrupos()
                .SelectMany(g =>
                    _profesorRepository.GetCalificacionesByGrupo(g.IdGrupo))
                .Where(c =>
                    c.Estudiante != null &&
                    c.Estudiante.Cedula == estudiante.Cedula)
                .ToList();
        }

        private void ValidarInscripcionDuplicada(
            Estudiante estudiante,
            Grupo grupo,
            string periodo)
        {
            bool existe = _inscripciones.Any(i =>
                i.Estudiante.Cedula == estudiante.Cedula &&
                i.Grupo.NombreMateria.Equals(
                    grupo.NombreMateria,
                    StringComparison.OrdinalIgnoreCase) &&
                i.PeriodoAcademico.Equals(
                    periodo,
                    StringComparison.OrdinalIgnoreCase));

            if (existe)
                throw new InvalidOperationException(
                    "El estudiante ya está inscrito en esta materia durante ese período.");
        }

        private void ValidarCupo(Grupo grupo)
        {
            int inscritos = _inscripciones
                .Where(i => i.Grupo.IdGrupo == grupo.IdGrupo)
                .Select(i => i.Estudiante.Cedula)
                .Distinct()
                .Count();

            inscritos = Math.Max(inscritos, grupo.Estudiantes.Count);

            if (inscritos >= grupo.Cupos)
                throw new InvalidOperationException(
                    "El grupo ya no tiene cupos disponibles.");
        }

        private void ValidarCruceHorario(
            Estudiante estudiante,
            Grupo grupo,
            string periodo)
        {
            bool existeCruce = _inscripciones.Any(i =>
                i.Estudiante.Cedula == estudiante.Cedula &&
                i.PeriodoAcademico.Equals(
                    periodo,
                    StringComparison.OrdinalIgnoreCase) &&
                i.Grupo.Dia.Equals(
                    grupo.Dia,
                    StringComparison.OrdinalIgnoreCase) &&
                grupo.HoraInicio < i.Grupo.HoraFin &&
                grupo.HoraFin > i.Grupo.HoraInicio);

            if (existeCruce)
                throw new InvalidOperationException(
                    "Existe un cruce de horario con otra materia inscrita.");
        }
    }
}
