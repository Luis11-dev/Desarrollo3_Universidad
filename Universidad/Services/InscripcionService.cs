using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Util;

namespace Universidad.Services
{
    public class InscripcionService
    {
        private readonly HorarioClaseRepository _horarioRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly InscripcionRepository _inscripcionRepository;
        private readonly ProfesorRepository _profesorRepository;

        public InscripcionService(
            HorarioClaseRepository horarioRepository,
            MateriaRepository materiaRepository,
            InscripcionRepository inscripcionRepository,
            ProfesorRepository profesorRepository)
        {
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _materiaRepository = materiaRepository ?? throw new ArgumentNullException(nameof(materiaRepository));
            _inscripcionRepository = inscripcionRepository ?? throw new ArgumentNullException(nameof(inscripcionRepository));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
        }

        public List<Materia> ObtenerMaterias()
        {
            return _materiaRepository.ObtenerTodas();
        }

        public List<HorarioClase> ObtenerHorariosDisponibles(Estudiante estudiante, string periodo)
        {
            if (estudiante == null)
            {
                throw new ArgumentNullException(nameof(estudiante));
            }

            List<Inscripcion> inscritas = _inscripcionRepository.ObtenerPorEstudiante(estudiante.Cedula)
                .Where(i => i.PeriodoAcademico.Equals(periodo, StringComparison.OrdinalIgnoreCase))
                .ToList();

            List<string> codigosInscritos = inscritas.Select(i => i.Materia.Codigo).ToList();

            return _horarioRepository.GetAll()
                .Where(h =>
                    !codigosInscritos.Contains(h.Materia.Codigo) &&
                    (string.IsNullOrWhiteSpace(estudiante.Carrera) || h.Grupo.Carrera.Equals(estudiante.Carrera, StringComparison.OrdinalIgnoreCase)) &&
                    h.EstudiantesInscritos.Count < h.Grupo.Cupos)
                .ToList();
        }

        public List<Inscripcion> ObtenerInscripcionesEstudiante(string cedula)
        {
            return _inscripcionRepository.ObtenerPorEstudiante(cedula);
        }

        public bool InscribirEstudiante(
            Estudiante estudiante,
            HorarioClase horario,
            string periodoAcademico,
            out string mensaje)
        {
            try
            {
                if (estudiante == null)
                {
                    throw new ArgumentNullException(nameof(estudiante));
                }

                if (horario == null)
                {
                    throw new ArgumentNullException(nameof(horario));
                }

                _inscripcionRepository.Inscribir(
                    estudiante,
                    horario,
                    periodoAcademico);

                FileManager.GuardarHorarios(_horarioRepository.GetAll());
                FileManager.GuardarInscripciones(_inscripcionRepository.ObtenerTodas());

                mensaje = "Inscripcion realizada satisfactoriamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public string GenerarComprobante(Inscripcion inscripcion)
        {
            if (inscripcion == null)
            {
                throw new ArgumentNullException(nameof(inscripcion));
            }

            return
                "================================================================================\n" +
                "                       COMPROBANTE OFICIAL DE MATRICULA                         \n" +
                "================================================================================\n" +
                $"No. Matricula:       {inscripcion.IdInscripcion}\n" +
                $"Periodo Academico:   {inscripcion.PeriodoAcademico}\n" +
                $"Fecha de Registro:   {inscripcion.FechaInscripcion:dd/MM/yyyy HH:mm}\n" +
                "--------------------------------------------------------------------------------\n" +
                "DATOS DEL ESTUDIANTE:\n" +
                $"Nombre:              {inscripcion.Estudiante.Nombre}\n" +
                $"Carnet / Cedula:     {inscripcion.Estudiante.Cedula}\n" +
                $"Carrera:             {inscripcion.Estudiante.Carrera}\n" +
                "--------------------------------------------------------------------------------\n" +
                "DETALLE DE LA MATERIA INSCRITA:\n" +
                $"Asignatura:          [{inscripcion.Materia.Codigo}] {inscripcion.Materia.Nombre}\n" +
                $"Grupo / Cohorte:     {inscripcion.Grupo.NombreGrupo} (Año {inscripcion.Grupo.AnioAcademico} - Semestre {inscripcion.Grupo.Semestre})\n" +
                $"Modalidad:           {inscripcion.Horario.Modalidad}\n" +
                $"Horario de Clases:   {inscripcion.Horario.Dia} {inscripcion.Horario.HoraInicio:hh\\:mm} a {inscripcion.Horario.HoraFin:hh\\:mm}\n" +
                $"Profesor Impartidor: {inscripcion.Horario.Profesor.Nombre} ({inscripcion.Horario.Profesor.Profesion})\n" +
                "================================================================================\n" +
                "                    ESTADO DE LA INSCRIPCION: CONFIRMADA                        \n" +
                "================================================================================";
        }
    }
}
