using System;
using System.Collections.Generic;

namespace Universidad.Models
{
    public class PlanAcademico
    {
        public string IdPlan { get; private set; }
        public string IdCarrera { get; private set; }
        public int IdAnioAcademico { get; private set; }
        public int IdSemestre { get; private set; }

        private readonly List<string> _codigosMaterias = new();
        public IReadOnlyCollection<string> CodigosMaterias => _codigosMaterias.AsReadOnly();

        public PlanAcademico(string idPlan, string idCarrera, int idAnioAcademico, int idSemestre)
        {
            IdPlan = idPlan;
            IdCarrera = idCarrera;
            IdAnioAcademico = idAnioAcademico;
            IdSemestre = idSemestre;
        }

        public void AgregarCodigoMateria(string codigoMateria)
        {
            if (!string.IsNullOrWhiteSpace(codigoMateria))
            {
                string codigo = codigoMateria.Trim().ToUpper();
                if (!_codigosMaterias.Contains(codigo))
                {
                    _codigosMaterias.Add(codigo);
                }
            }
        }

        public void RemoverCodigoMateria(string codigoMateria)
        {
            if (!string.IsNullOrWhiteSpace(codigoMateria))
            {
                _codigosMaterias.Remove(codigoMateria.Trim().ToUpper());
            }
        }
    }
}

