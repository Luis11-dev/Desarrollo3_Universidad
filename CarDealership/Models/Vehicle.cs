namespace CarDealership.Models
{
    public abstract class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LicensePlate { get; set; } = string.Empty;
        public string Vin { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Mileage { get; set; }

        public const decimal IVA = 1.15m;
        public virtual decimal SpecificTaxRate => 1.0m;

        public abstract decimal CalculateSalePrice();
    }
}
