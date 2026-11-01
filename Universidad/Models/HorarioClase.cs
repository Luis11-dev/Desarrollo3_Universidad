using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Models
{
    public class HorarioClase
    {
        public string IdHorario { get; private set; }
        public Grupo Grupo { get; private set; }
        public Materia Materia { get; private set; }
        public Profesor Profesor { get; private set; }
        public string Dia { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }
        public Modalidad Modalidad { get; private set; }

        public int HorasPorSesion => Math.Max(1, (int)(HoraFin - HoraInicio).TotalHours);
        public int HorasMensuales => Modalidad == Modalidad.Dominical ? 60 : 80;

        private readonly List<Estudiante> _estudiantesInscritos = new();
        public IReadOnlyCollection<Estudiante> EstudiantesInscritos => _estudiantesInscritos.AsReadOnly();

        public HorarioClase(
            string idHorario,
            Grupo grupo,
            Materia materia,
            Profesor profesor,
            string dia,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            Modalidad modalidad)
        {
            IdHorario = idHorario;
            Grupo = grupo;
            Materia = materia;
            Profesor = profesor;
            Dia = dia;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Modalidad = modalidad;
        }

        public void InscribirEstudiante(Estudiante estudiante)
        {
            if (estudiante != null && !_estudiantesInscritos.Any(e => e.Cedula.Equals(estudiante.Cedula, StringComparison.OrdinalIgnoreCase)))
            {
                _estudiantesInscritos.Add(estudiante);
            }
        }

        public void DesinscribirEstudiante(string cedulaEstudiante)
        {
            _estudiantesInscritos.RemoveAll(e => e.Cedula.Equals(cedulaEstudiante, StringComparison.OrdinalIgnoreCase));
        }
    }
}

