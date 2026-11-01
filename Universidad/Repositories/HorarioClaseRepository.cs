using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class HorarioClaseRepository
    {
        private readonly List<HorarioClase> _horarios = new();

        public void Add(HorarioClase horario)
        {
            if (horario == null)
            {
                throw new ArgumentNullException(nameof(horario));
            }
            _horarios.Add(horario);
        }

        public List<HorarioClase> GetAll()
        {
            return _horarios.ToList();
        }

        public List<HorarioClase> GetByProfesor(string cedulaProfesor)
        {
            return _horarios
                .Where(h => h.Profesor.Cedula.Equals(cedulaProfesor, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<HorarioClase> GetByGrupo(string idGrupo)
        {
            return _horarios
                .Where(h => h.Grupo.IdGrupo.Equals(idGrupo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public bool ExisteCruceProfesor(string cedulaProfesor, string dia, TimeSpan inicio, TimeSpan fin)
        {
            return _horarios.Any(h =>
                h.Profesor.Cedula.Equals(cedulaProfesor, StringComparison.OrdinalIgnoreCase) &&
                h.Dia.Equals(dia, StringComparison.OrdinalIgnoreCase) &&
                inicio < h.HoraFin && fin > h.HoraInicio);
        }

        public bool ExisteCruceGrupo(string idGrupo, string dia, TimeSpan inicio, TimeSpan fin)
        {
            return _horarios.Any(h =>
                h.Grupo.IdGrupo.Equals(idGrupo, StringComparison.OrdinalIgnoreCase) &&
                h.Dia.Equals(dia, StringComparison.OrdinalIgnoreCase) &&
                inicio < h.HoraFin && fin > h.HoraInicio);
        }
    }
}

