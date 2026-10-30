using System;

namespace Universidad.Models
{
    public class AsistenciaProfesor
    {
        public Profesor Profesor { get; private set; } = null!;
        public Grupo Grupo { get; private set; } = null!;
        public DateTime Fecha { get; private set; }
        public TimeSpan? HoraEntrada { get; private set; }
        public bool Asistio { get; private set; }

        public AsistenciaProfesor() { }

        public AsistenciaProfesor(Profesor profesor, Grupo grupo, DateTime fecha, TimeSpan? horaEntrada, bool asistio)
        {
            Profesor = profesor;
            Grupo = grupo;
            Fecha = fecha.Date;
            HoraEntrada = horaEntrada;
            Asistio = asistio;
        }
    }
}

