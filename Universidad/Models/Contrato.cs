using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Models
{
    public class Contrato
    {
        public string IdContrato { get; private set; }
        public Profesor Profesor { get; private set; }
        public string PeriodoAcademico { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public decimal TarifaPorHora { get; private set; }
        public bool Activo { get; private set; }

        private readonly List<HorarioClase> _materiasAsignadas = new();
        public IReadOnlyCollection<HorarioClase> MateriasAsignadas => _materiasAsignadas.AsReadOnly();

        public int HorasMensualesTotales
        {
            get
            {
                return _materiasAsignadas.Sum(m => m.HorasMensuales);
            }
        }

        public decimal SalarioMensualPactado
        {
            get
            {
                return HorasMensualesTotales * TarifaPorHora;
            }
        }

        public Contrato(string idContrato, Profesor profesor, string periodoAcademico, decimal tarifaPorHora = 400m)
        {
            if (string.IsNullOrWhiteSpace(idContrato))
            {
                throw new ArgumentException("El identificador del contrato es obligatorio.", nameof(idContrato));
            }
            if (profesor == null)
            {
                throw new ArgumentNullException(nameof(profesor));
            }
            if (string.IsNullOrWhiteSpace(periodoAcademico))
            {
                throw new ArgumentException("El periodo academico es obligatorio.", nameof(periodoAcademico));
            }

            IdContrato = idContrato;
            Profesor = profesor;
            PeriodoAcademico = periodoAcademico;
            FechaEmision = DateTime.Now;
            TarifaPorHora = tarifaPorHora;
            Activo = true;
        }

        public void AsignarMateria(HorarioClase horario)
        {
            if (horario != null && !_materiasAsignadas.Contains(horario))
            {
                _materiasAsignadas.Add(horario);
            }
        }

        public void DesasignarMateria(string idHorario)
        {
            _materiasAsignadas.RemoveAll(h => h.IdHorario.Equals(idHorario, StringComparison.OrdinalIgnoreCase));
        }

        public void CancelarContrato()
        {
            Activo = false;
        }
    }
}

