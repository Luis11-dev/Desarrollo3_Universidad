namespace CarDealership.Models
{
    public class Storage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public string SecurityLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
