namespace CarDealership.Models
{
    public class Motorcycle : Vehicle
    {
        public int EngineDisplacementCc { get; set; }
        public bool HasSidecar { get; set; }

        public const decimal TaxRate = 1.03m;
        public override decimal SpecificTaxRate => TaxRate;

        public override decimal CalculateSalePrice()
        {
            return BasePrice * IVA * TaxRate;
        }
    }
}
