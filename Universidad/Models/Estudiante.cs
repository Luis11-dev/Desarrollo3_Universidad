
using System;

namespace Universidad.Models
{
    public class Estudiante : PerfilUniversitario, IAutentication
    {
        public DateTime FechaNacimiento { get; private set; }

        public string Carrera { get; private set; } = string.Empty;

        public bool Activo { get; private set; }

        public string User { get; private set; } = string.Empty;

        public string Password { get; private set; } = string.Empty;

        public Rol Rol { get; private set; } = Rol.Estudiante;

        public Estudiante() : base()
        {
        }

        public Estudiante(
            string cedula,
            string nombre,
            DateTime fechaNacimiento,
            string correoElectronico,
            int numeroCelular,
            string direccion,
            Genero genero,
            string carrera,
            bool activo,
            string user,
            string password)
            : base(
                cedula,
                nombre,
                correoElectronico,
                numeroCelular,
                direccion,
                genero)
        {
            FechaNacimiento = fechaNacimiento;
            Carrera = carrera;
            Activo = activo;
            User = user;
            Password = password;
            Rol = Rol.Estudiante;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
