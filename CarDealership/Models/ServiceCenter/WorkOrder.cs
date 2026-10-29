using System;

namespace CarDealership.Models.ServiceCenter
{
    public class WorkOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkshopId { get; set; }
        public string VehiclePlate { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.InProgress;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
