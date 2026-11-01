using System;
using System.Collections.Generic;

namespace Universidad.Models
{
    public class Grupo
    {
        public string IdGrupo { get; private set; } = string.Empty;
        public string NombreGrupo { get; private set; } = string.Empty;
        public string Carrera { get; private set; } = string.Empty;
        public int AnioAcademico { get; private set; }
        public int Semestre { get; private set; }
        public Modalidad Modalidad { get; private set; }
        public int Cupos { get; private set; }

        // Propiedades de retrocompatibilidad
        public string NombreMateria { get; private set; } = string.Empty;
        public string Dia { get; private set; } = string.Empty;
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public Profesor? ProfesorAsignado { get; private set; }

        private readonly List<PerfilUniversitario> _estudiantes = new();
        public IReadOnlyCollection<PerfilUniversitario> Estudiantes => _estudiantes.AsReadOnly();

        public Grupo() { }

        // Constructor orientado a cohorte de estudiantes con Carrera y Año Académico
        public Grupo(
            string idGrupo,
            string nombreGrupo,
            string carrera,
            int anioAcademico,
            int semestre,
            Modalidad modalidad,
            int cupos)
        {
            IdGrupo = idGrupo;
            NombreGrupo = nombreGrupo;
            Carrera = carrera;
            AnioAcademico = anioAcademico;
            Semestre = semestre;
            Modalidad = modalidad;
            Cupos = cupos;
        }

        // Constructor con retrocompatibilidad para llamadas existentes
        public Grupo(
            string idGrupo,
            string nombreGrupo,
            string materia,
            int cupos,
            int semestre,
            Modalidad modalidad,
            string dia,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            Profesor? profesorAsignado = null,
            string carrera = "Ingenieria en Sistemas",
            int anioAcademico = 1)
            : this(idGrupo, nombreGrupo, carrera, anioAcademico, semestre, modalidad, cupos)
        {
            NombreMateria = materia;
            Dia = dia;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            ProfesorAsignado = profesorAsignado;
        }

        public void AsignarProfesor(Profesor profesor)
        {
            ProfesorAsignado = profesor;
        }

        public void AgregarEstudiante(PerfilUniversitario estudiante)
        {
            if (estudiante != null && !_estudiantes.Contains(estudiante))
            {
                _estudiantes.Add(estudiante);
            }
        }

        public int ObtenerHorasContrato() => Modalidad == Modalidad.Dominical ? 60 : 80;
        public decimal ObtenerSalarioContrato() => ObtenerHorasContrato() * 400m;
        public int ObtenerHorasPorSesion() => Math.Max(1, (int)(HoraFin - HoraInicio).TotalHours);
    }
}
