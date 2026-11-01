using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;

namespace Universidad.Repositories
{
    public class PlanAcademicoRepository
    {
        private readonly List<PlanAcademico> _planes = new();

        public void Add(PlanAcademico plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }
            _planes.Add(plan);
        }

        public List<PlanAcademico> GetAll()
        {
            return _planes.ToList();
        }

        public PlanAcademico? GetByCarreraAnioSemestre(string idCarrera, int anioAcademico, int semestre)
        {
            return _planes.FirstOrDefault(p =>
                p.IdCarrera.Equals(idCarrera, StringComparison.OrdinalIgnoreCase) &&
                p.IdAnioAcademico == anioAcademico &&
                p.IdSemestre == semestre);
        }
    }
}
