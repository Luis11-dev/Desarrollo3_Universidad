namespace CarDealership.Models
{
    public class Stock
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public LocationType LocationType { get; set; }
        public Guid LocationId { get; set; }

        public int Quantity { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
