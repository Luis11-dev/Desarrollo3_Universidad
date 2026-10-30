using System;

namespace Universidad.Models
{
    public class Calificacion
    {
        public PerfilUniversitario? Estudiante { get; private set; }
        public Grupo? Grupo { get; private set; }
        public int? Acumulado { get; private set; }
        public int? Examen { get; private set; }
        public int? Nota { get; private set; }
        public int? NotaConvocatoria { get; private set; }
        public int? NotaFinalConvocatoria { get; private set; }
        public EstadoCalificacion Estado { get; private set; } = EstadoCalificacion.Pendiente;

        public const int NotaAprobacion = 60;
        public const int NotaMinimaConvocatoria = 40;
        public const int TopeConvocatoria = 80;

        public Calificacion() { }

        public Calificacion(PerfilUniversitario estudiante, Grupo grupo)
        {
            Estudiante = estudiante;
            Grupo = grupo;
            Estado = EstadoCalificacion.Pendiente;
        }

        public void RegistrarNotas(int acumulado, int examen)
        {
            Acumulado = acumulado;
            Examen = examen;
            Nota = CalcularNota(acumulado, examen);
            Estado = DeterminarEstado(Nota);
        }

        public void RegistrarConvocatoria(int notaExtraordinaria)
        {
            NotaConvocatoria = notaExtraordinaria;
            NotaFinalConvocatoria = Math.Min(notaExtraordinaria, TopeConvocatoria);
            Estado = NotaFinalConvocatoria >= NotaAprobacion
                ? EstadoCalificacion.Aprobado
                : EstadoCalificacion.ReprobadoSinConvocatoria;
        }

        public int CalcularNota(int acumulado, int examen)
        {
            return acumulado + examen;
        }

        public EstadoCalificacion DeterminarEstado(int? nota)
        {
            if (nota == null)
            {
                return EstadoCalificacion.Pendiente;
            }
            else if (nota >= NotaAprobacion)
            {
                return EstadoCalificacion.Aprobado;
            }
            else if (nota >= NotaMinimaConvocatoria && nota < NotaAprobacion)
            {
                return EstadoCalificacion.ReprobadoConConvocatoria;
            }
            else
            {
                return EstadoCalificacion.ReprobadoSinConvocatoria;
            }
        }
    }
}
