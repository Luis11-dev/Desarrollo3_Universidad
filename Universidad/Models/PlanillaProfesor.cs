using System.Collections.Generic;

namespace Universidad.Models
{
    public class PlanillaProfesor
    {
        public Profesor Profesor { get; set; } = null!;
        public List<HorarioClase> HorariosAsignados { get; set; } = new();
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

