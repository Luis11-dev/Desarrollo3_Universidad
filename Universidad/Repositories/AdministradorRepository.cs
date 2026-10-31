using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class AdministradorRepository
    {
        private readonly List<Administrador> _administradores = new();

        public void Add(Administrador administrador)
        {
            if (administrador == null)
            {
                throw new ArgumentNullException(nameof(administrador));
            }
            _administradores.Add(administrador);
        }

        public List<Administrador> GetAll()
        {
            return _administradores.ToList();
        }

        public Administrador? GetByUser(string user)
        {
            return _administradores.FirstOrDefault(a => a.User.Equals(user, StringComparison.OrdinalIgnoreCase));
        }

        public Administrador? GetByCedula(string cedula)
        {
            return _administradores.FirstOrDefault(a => a.Cedula.Equals(cedula, StringComparison.OrdinalIgnoreCase));
        }
    }
}

