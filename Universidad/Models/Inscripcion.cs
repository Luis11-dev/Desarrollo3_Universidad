using System;

namespace Universidad.Models
{
    public class Inscripcion
    {
        public string IdInscripcion { get; private set; } = string.Empty;
        public Estudiante Estudiante { get; private set; } = null!;
        public HorarioClase Horario { get; private set; } = null!;

        public Grupo Grupo
        {
            get
            {
                return Horario.Grupo;
            }
        }

        public Materia Materia
        {
            get
            {
                return Horario.Materia;
            }
        }

        public string PeriodoAcademico { get; private set; } = string.Empty;
        public DateTime FechaInscripcion { get; private set; }

        public Inscripcion() { }

        public Inscripcion(
            string idInscripcion,
            Estudiante estudiante,
            HorarioClase horario,
            string periodoAcademico)
        {
            if (string.IsNullOrWhiteSpace(idInscripcion))
            {
                throw new ArgumentException("El identificador es obligatorio.", nameof(idInscripcion));
            }
            if (estudiante == null)
            {
                throw new ArgumentNullException(nameof(estudiante));
            }
            if (horario == null)
            {
                throw new ArgumentNullException(nameof(horario));
            }
            if (string.IsNullOrWhiteSpace(periodoAcademico))
            {
                throw new ArgumentException("El periodo academico es obligatorio.", nameof(periodoAcademico));
            }

            IdInscripcion = idInscripcion.Trim();
            Estudiante = estudiante;
            Horario = horario;
            PeriodoAcademico = periodoAcademico.Trim();
            FechaInscripcion = DateTime.Now;
        }
    }
}
