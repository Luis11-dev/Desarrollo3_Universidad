namespace CarDealership.Models
{
    public class Automobile : Vehicle
    {
        public int NumberOfDoors { get; set; }
        public int PassengerCapacity { get; set; }
        public string Transmission { get; set; } = string.Empty;

        public const decimal TaxRate = 1.05m;
        public override decimal SpecificTaxRate => TaxRate;

        public override decimal CalculateSalePrice()
        {
            return BasePrice * IVA * TaxRate;
        }
    }
}
