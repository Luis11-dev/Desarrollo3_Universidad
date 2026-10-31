
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Models
{
    public class Materia
    {
        private readonly List<string> _prerrequisitos = new();

        public string Codigo { get; private set; }
        public string Nombre { get; private set; }

        public IReadOnlyCollection<string> Prerrequisitos =>
            _prerrequisitos.AsReadOnly();

        public Materia()
        {
            Codigo = string.Empty;
            Nombre = string.Empty;
        }

        public Materia(string codigo, string nombre)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es obligatorio.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            Codigo = codigo.Trim().ToUpper();
            Nombre = nombre.Trim();
        }

        public void AgregarPrerrequisito(string codigoMateria)
        {
            if (string.IsNullOrWhiteSpace(codigoMateria))
                throw new ArgumentException("El código del prerrequisito es obligatorio.");

            codigoMateria = codigoMateria.Trim().ToUpper();

            if (codigoMateria == Codigo)
                throw new InvalidOperationException(
                    "Una materia no puede ser prerrequisito de sí misma.");

            if (_prerrequisitos.Contains(codigoMateria))
                throw new InvalidOperationException(
                    "Ese prerrequisito ya está registrado.");

            _prerrequisitos.Add(codigoMateria);
        }

        public void QuitarPrerrequisito(string codigoMateria)
        {
            _prerrequisitos.Remove(
                codigoMateria.Trim().ToUpper());
        }
    }
}
