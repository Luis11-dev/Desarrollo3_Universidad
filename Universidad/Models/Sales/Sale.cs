namespace Universidad.Models
{
    public class Sale
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public decimal SoldPrice { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public SaleStatus Status { get; set; } = SaleStatus.Completed;
    }
}
