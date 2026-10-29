using System;
using System.Collections.Generic;

namespace CarDealership.Models.ServiceCenter
{
    public class Workshop
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public List<string> CurrentVehicles { get; set; } = new List<string>();

        public int OccupiedSpaces()
        {
            return CurrentVehicles.Count;
        }

        public int AvailableSpaces()
        {
            int available = Capacity - CurrentVehicles.Count;
            if (available < 0)
            {
                return 0;
            }
            return available;
        }

        public bool HasFreeSpace()
        {
            return CurrentVehicles.Count < Capacity;
        }
    }
}
