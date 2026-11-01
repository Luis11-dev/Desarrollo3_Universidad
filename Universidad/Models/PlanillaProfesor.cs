using System.Collections.Generic;

namespace Universidad.Models
{
    public class PlanillaProfesor
    {
        public Profesor Profesor { get; set; } = null!;
        public Contrato? Contrato { get; set; }
        public List<HorarioClase> MateriasAsignadas { get; set; } = new();
        public List<HorarioClase> HorariosAsignados
        {
            get
            {
                return MateriasAsignadas;
            }
            set
            {
                MateriasAsignadas = value;
            }
        }
        public int HorasContratadasMes { get; set; }
        public int CantidadAusencias { get; set; }
        public int HorasDescontadas { get; set; }
        public int HorasEfectivasTrabajadas { get; set; }
        public decimal TarifaPorHora { get; set; }
        public decimal SalarioBruto { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal SalarioNeto { get; set; }
    }
}
