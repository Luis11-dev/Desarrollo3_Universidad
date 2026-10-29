using CarDealership.Models.ServiceCenter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Repositories.ServiceCenter
{
    public class ServiceCenterRepository
    {
        private readonly List<Workshop> _workshops = new List<Workshop>();
        private readonly List<WorkOrder> _workOrders = new List<WorkOrder>();

        public void AddWorkshop(Workshop workshop)
        {
            _workshops.Add(workshop);
        }

        public IEnumerable<Workshop> GetAllWorkshops()
        {
            return _workshops.ToList();
        }

        public Workshop? GetWorkshopById(Guid id)
        {
            return _workshops.FirstOrDefault(w => w.Id == id);
        }

        public void AddWorkOrder(WorkOrder order)
        {
            _workOrders.Add(order);
        }

        public IEnumerable<WorkOrder> GetAllWorkOrders()
        {
            return _workOrders.ToList();
        }

        public WorkOrder? GetWorkOrderById(Guid id)
        {
            return _workOrders.FirstOrDefault(o => o.Id == id);
        }

        public WorkOrder? GetActiveWorkOrderByPlate(string plate)
        {
            return _workOrders.FirstOrDefault(o =>
                o.VehiclePlate.Equals(plate, StringComparison.OrdinalIgnoreCase) &&
                o.Status != WorkOrderStatus.Delivered);
        }
    }
}
