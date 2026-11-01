using System;

namespace Universidad.ProfesorExceptions
{
    // Excepcion base para cualquier error de negocio del modulo de profesores
    public class ProfesorException : Exception
    {
        public ProfesorException(string mensaje) : base(mensaje) { }
    }

    // Cuando un usuario sin rol de profesor intenta acceder al modulo
    public class AccesoDenegadoProfesorException : ProfesorException
    {
        public AccesoDenegadoProfesorException(string mensaje = "Acceso denegado: El usuario no tiene permisos de Profesor.")
            : base(mensaje) { }
    }

    // Cuando la fecha tope fijada por el administrador ya vencio
    public class FechaTopeVencidaException : ProfesorException
    {
        public DateTime FechaTope { get; }

        public FechaTopeVencidaException(DateTime fechaTope)
            : base($"La fecha tope para el registro de calificaciones ({fechaTope:dd/MM/yyyy}) ya vencio.")
        {
            FechaTope = fechaTope;
        }
    }

    // Cuando se intenta calificar fuera del rango 0-60, 0-40 o 0-100
    public class NotaFueraDeRangoException : ProfesorException
    {
        public NotaFueraDeRangoException(string campo, int min, int max)
            : base($"La nota de {campo} debe estar en el rango de {min} a {max} puntos.") { }
    }

    // Cuando un estudiante ya tiene nota y se intenta re-calificar
    public class CalificacionYaRegistradaException : ProfesorException
    {
        public CalificacionYaRegistradaException(string nombreEstudiante)
            : base($"El estudiante '{nombreEstudiante}' ya tiene una calificacion registrada y no se puede editar.") { }
    }

    // Cuando no tiene derecho a convocatoria o ya la realizo
    public class ConvocatoriaNoPermitidaException : ProfesorException
    {
        public ConvocatoriaNoPermitidaException(string mensaje) : base(mensaje) { }
    }

    // Cuando se intenta marcar asistencia fuera de los 15 minutos habiles
    public class AsistenciaFueraDeTiempoException : ProfesorException
    {
        public AsistenciaFueraDeTiempoException(string mensaje) : base(mensaje) { }
    }

    // Cuando el profesor intenta consultar o calificar un grupo que no le fue asignado
    public class GrupoNoAsignadoException : ProfesorException
    {
        public GrupoNoAsignadoException(string mensaje = "Acceso denegado: Solo puede ver y calificar grupos que tiene asignados.")
            : base(mensaje) { }
    }
}

