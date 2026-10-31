
using System;

namespace Universidad.Models
{
    public class Inscripcion
    {
        public string IdInscripcion { get; private set; }
        public Estudiante Estudiante { get; private set; }
        public Grupo Grupo { get; private set; }
        public string PeriodoAcademico { get; private set; }
        public DateTime FechaInscripcion { get; private set; }

        public Inscripcion(
            string idInscripcion,
            Estudiante estudiante,
            Grupo grupo,
            string periodoAcademico)
        {
            if (string.IsNullOrWhiteSpace(idInscripcion))
                throw new ArgumentException("El identificador es obligatorio.");

            if (estudiante == null)
                throw new ArgumentNullException(nameof(estudiante));

            if (grupo == null)
                throw new ArgumentNullException(nameof(grupo));

            if (string.IsNullOrWhiteSpace(periodoAcademico))
                throw new ArgumentException("El período académico es obligatorio.");

            IdInscripcion = idInscripcion;
            Estudiante = estudiante;
            Grupo = grupo;
            PeriodoAcademico = periodoAcademico.Trim();
            FechaInscripcion = DateTime.Now;
        }
    }
}
