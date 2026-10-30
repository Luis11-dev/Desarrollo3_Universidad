using System;
using System.Collections.Generic;

namespace Universidad.Models
{
    public class Grupo
    {
        public string IdGrupo { get; private set; } = string.Empty;
        public string NombreGrupo { get; private set; } = string.Empty;
        public string NombreMateria { get; private set; } = string.Empty;
        public int Cupos { get; private set; }
        public int Semestre { get; private set; }
        public Modalidad Modalidad { get; private set; }
        public string Dia { get; private set; } = string.Empty;
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public Profesor? ProfesorAsignado { get; private set; }

        private readonly List<PerfilUniversitario> _estudiantes = new();
        public IReadOnlyCollection<PerfilUniversitario> Estudiantes => _estudiantes.AsReadOnly();

        public Grupo() { }

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
            Profesor? profesorAsignado = null)
        {
            IdGrupo = idGrupo;
            NombreGrupo = nombreGrupo;
            NombreMateria = materia;
            Cupos = cupos;
            Semestre = semestre;
            Modalidad = modalidad;
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
            _estudiantes.Add(estudiante);
        }

        public int ObtenerHorasContrato() => Modalidad == Modalidad.Dominical ? 60 : 80;
        public decimal ObtenerSalarioContrato() => ObtenerHorasContrato() * 400m;
        public int ObtenerHorasPorSesion() => Math.Max(1, (int)(HoraFin - HoraInicio).TotalHours);
    }
}
