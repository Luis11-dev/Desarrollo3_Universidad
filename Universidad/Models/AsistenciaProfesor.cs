using System;

namespace Universidad.Models
{
    public class AsistenciaProfesor
    {
        public Profesor Profesor { get; private set; } = null!;
        public HorarioClase Horario { get; private set; } = null!;
        public Grupo Grupo
        {
            get
            {
                return Horario.Grupo;
            }
        }
        public DateTime Fecha { get; private set; }
        public TimeSpan? HoraEntrada { get; private set; }
        public bool Asistio { get; private set; }

        public AsistenciaProfesor() { }

        public AsistenciaProfesor(Profesor profesor, HorarioClase horario, DateTime fecha, TimeSpan? horaEntrada, bool asistio)
        {
            Profesor = profesor ?? throw new ArgumentNullException(nameof(profesor));
            Horario = horario ?? throw new ArgumentNullException(nameof(horario));
            Fecha = fecha.Date;
            HoraEntrada = horaEntrada;
            Asistio = asistio;
        }
    }
}
