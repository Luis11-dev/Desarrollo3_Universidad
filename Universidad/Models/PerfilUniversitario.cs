using System;

namespace Universidad.Models
{
    public abstract class PerfilUniversitario
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Cedula { get; private set; } = string.Empty;
        public string Nombre { get; private set; } = string.Empty;
        public string CorreoElectronico { get; private set; } = string.Empty;
        public int NumeroCelular { get; private set; }
        public string Direccion { get; private set; } = string.Empty;
        public Genero Genero { get; private set; }

        protected PerfilUniversitario() { }

        protected PerfilUniversitario(
            string cedula,
            string nombre,
            string correoElectronico,
            int numeroCelular,
            string direccion,
            Genero genero,
            Guid? id = null)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Id = id.Value;
            }

            Cedula = cedula;
            Nombre = nombre;
            CorreoElectronico = correoElectronico;
            NumeroCelular = numeroCelular;
            Direccion = direccion;
            Genero = genero;
        }
    }
}
