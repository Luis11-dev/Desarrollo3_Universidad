namespace CarDealership.Models
{
    public class Showroom
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int DisplayCapacity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
