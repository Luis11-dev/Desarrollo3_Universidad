namespace CarDealership.Models
{
    public class Truck : Vehicle
    {
        public double LoadCapacityTons { get; set; }
        public double TowingCapacityTons { get; set; }
        public int AxleCount { get; set; }

        public const decimal TaxRate = 1.08m;
        public override decimal SpecificTaxRate => TaxRate;

        public override decimal CalculateSalePrice()
        {
            return BasePrice * IVA * TaxRate;
        }
    }
}
