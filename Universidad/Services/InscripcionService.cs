
using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Services
{
    public class InscripcionService
    {
        private readonly MateriaRepository _materiaRepository;
        private readonly ProfesorRepository _profesorRepository;
        private readonly InscripcionRepository _inscripcionRepository;

        public InscripcionService(
            MateriaRepository materiaRepository,
            ProfesorRepository profesorRepository,
            InscripcionRepository inscripcionRepository)
        {
            _materiaRepository = materiaRepository;
            _profesorRepository = profesorRepository;
            _inscripcionRepository = inscripcionRepository;
        }

        public List<Materia> ObtenerMaterias()
        {
            return _materiaRepository.ObtenerTodas();
        }

        public List<Grupo> ObtenerGruposDisponibles()
        {
            return _profesorRepository.GetAllGrupos()
                .Where(g => g.ProfesorAsignado != null)
                .ToList();
        }

        public List<Inscripcion> ObtenerInscripcionesEstudiante(
            string cedula)
        {
            return _inscripcionRepository.ObtenerPorEstudiante(cedula);
        }

        public bool InscribirEstudiante(
            Estudiante estudiante,
            Grupo grupo,
            string periodoAcademico,
            out string mensaje)
        {
            try
            {
                if (estudiante == null)
                    throw new ArgumentNullException(nameof(estudiante));

                if (grupo == null)
                    throw new ArgumentNullException(nameof(grupo));

                _inscripcionRepository.Inscribir(
                    estudiante,
                    grupo,
                    periodoAcademico);

                mensaje = "Inscripción realizada correctamente.";
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
                throw new ArgumentNullException(nameof(inscripcion));

            return
                "========================================\n" +
                "       COMPROBANTE DE INSCRIPCIÓN       \n" +
                "========================================\n" +
                $"Código: {inscripcion.IdInscripcion}\n" +
                $"Estudiante: {inscripcion.Estudiante.Nombre}\n" +
                $"Cédula: {inscripcion.Estudiante.Cedula}\n" +
                $"Materia: {inscripcion.Grupo.NombreMateria}\n" +
                $"Grupo: {inscripcion.Grupo.NombreGrupo}\n" +
                $"Período: {inscripcion.PeriodoAcademico}\n" +
                $"Profesor: {inscripcion.Grupo.ProfesorAsignado?.Nombre ?? "Sin asignar"}\n" +
                $"Fecha: {inscripcion.FechaInscripcion:dd/MM/yyyy HH:mm}\n" +
                "========================================\n" +
                "       INSCRIPCIÓN REGISTRADA           \n" +
                "========================================";
        }
    }
}
