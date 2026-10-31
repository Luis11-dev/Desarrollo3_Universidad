
using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class MateriaRepository
    {
        private readonly List<Materia> _materias = new();

        public void Agregar(Materia materia)
        {
            if (materia == null)
                throw new ArgumentNullException(nameof(materia));

            if (_materias.Any(m =>
                m.Codigo.Equals(
                    materia.Codigo,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Ya existe una materia con ese código.");
            }

            _materias.Add(materia);
        }

        public List<Materia> ObtenerTodas()
        {
            return _materias.ToList();
        }

        public Materia? ObtenerPorCodigo(string codigo)
        {
            return _materias.FirstOrDefault(m =>
                m.Codigo.Equals(
                    codigo,
                    StringComparison.OrdinalIgnoreCase));
        }

        public bool Existe(string codigo)
        {
            return ObtenerPorCodigo(codigo) != null;
        }
    }
}
