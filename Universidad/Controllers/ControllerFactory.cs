using System;
using Universidad.Models;
using Universidad.Util;

namespace Universidad.Controllers
{
    public static class ControllerFactory
    {
        public static IController CrearControlador(IAutentication usuario)
        {
            switch (usuario.Rol)
            {
                case Rol.Profesor:
                {
                    return Startup.InitProfesorController((Profesor)usuario);
                }
                case Rol.Estudiante:
                {
                    return Startup.InitEstudianteController((Estudiante)usuario);
                }
                default:
                {
                    throw new NotSupportedException($"El rol '{usuario.Rol}' no es soportado.");
                }
            }
        }
    }
}

