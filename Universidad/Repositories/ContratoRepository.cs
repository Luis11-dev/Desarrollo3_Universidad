using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class ContratoRepository
    {
        private readonly List<Contrato> _contratos = new();

        public void Add(Contrato contrato)
        {
            if (contrato == null)
            {
                throw new ArgumentNullException(nameof(contrato));
            }
            _contratos.Add(contrato);
        }

        public List<Contrato> GetAll()
        {
            return _contratos.ToList();
        }

        public Contrato? GetById(string idContrato)
        {
            return _contratos.FirstOrDefault(c => c.IdContrato.Equals(idContrato, StringComparison.OrdinalIgnoreCase));
        }

        public List<Contrato> GetByProfesor(string cedulaProfesor)
        {
            return _contratos
                .Where(c => c.Profesor.Cedula.Equals(cedulaProfesor, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Contrato? GetContratoActivo(string cedulaProfesor)
        {
            return _contratos.FirstOrDefault(c =>
                c.Profesor.Cedula.Equals(cedulaProfesor, StringComparison.OrdinalIgnoreCase) && c.Activo);
        }
    }
}

