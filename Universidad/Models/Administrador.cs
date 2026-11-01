using System;

namespace Universidad.Models
{
    public class Administrador : PerfilUniversitario, IAutentication
    {
        public string User { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public Rol Rol { get; private set; } = Rol.Administrador;

        public Administrador() : base() { }

        public Administrador(
            string cedula,
            string nombre,
            string correoElectronico,
            int numeroCelular,
            string direccion,
            Genero genero,
            string user,
            string password,
            Guid? id = null)
            : base(cedula, nombre, correoElectronico, numeroCelular, direccion, genero, id)
        {
            User = user;
            Password = password;
            Rol = Rol.Administrador;
        }
    }
}

