using CarDealership.Models.ServiceCenter;
using CarDealership.Repositories.ServiceCenter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Services.ServiceCenter
{
    public class ServiceCenterService
    {
        private readonly ServiceCenterRepository _repository;

        public ServiceCenterService(ServiceCenterRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Workshop> GetAllWorkshops()
        {
            return _repository.GetAllWorkshops();
        }

        public Workshop GetWorkshopById(Guid workshopId)
        {
            var workshop = _repository.GetWorkshopById(workshopId);
            if (workshop == null)
            {
                throw new KeyNotFoundException("No se encontro el taller especificado.");
            }
            return workshop;
        }

        public (bool HasSpace, int Available, int Capacity, int Occupied) CheckWorkshopSpace(Guid workshopId)
        {
            var workshop = GetWorkshopById(workshopId);
            return (workshop.HasFreeSpace(), workshop.AvailableSpaces(), workshop.Capacity, workshop.OccupiedSpaces());
        }

        public bool CheckInVehicle(Guid workshopId, string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
            {
                throw new ArgumentException("La placa del vehiculo no puede estar vacia.");
            }

            var workshop = GetWorkshopById(workshopId);

            if (!workshop.HasFreeSpace())
            {
                throw new InvalidOperationException($"El taller '{workshop.Name}' tiene todos sus espacios ocupados ({workshop.Capacity}/{workshop.Capacity}).");
            }

            string normalizedPlate = plate.Trim().ToUpper();

            if (workshop.CurrentVehicles.Any(p => p.Equals(normalizedPlate, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CarDealership.Util.PlacaDuplicadaException(normalizedPlate, $"El vehiculo con placa '{normalizedPlate}' ya se encuentra dentro de este taller.");
            }

            workshop.CurrentVehicles.Add(normalizedPlate);
            return true;
        }

        public WorkOrder CreateWorkOrder(Guid workshopId, string plate, string customerName, string customerPhone, string description, decimal estimatedCost)
        {
            if (string.IsNullOrWhiteSpace(plate))
            {
                throw new ArgumentException("La placa del vehiculo es requerida.");
            }
            if (string.IsNullOrWhiteSpace(customerName))
            {
                throw new ArgumentException("El nombre del cliente es requerido.");
            }
            if (estimatedCost <= 0)
            {
                throw new ArgumentException("El costo estimado debe ser mayor a 0.");
            }

            var workshop = GetWorkshopById(workshopId);
            string normalizedPlate = plate.Trim().ToUpper();

            // Si el vehiculo no ha sido ingresado aun al taller fisicamente, lo ingresa verificando capacidad
            if (!workshop.CurrentVehicles.Any(p => p.Equals(normalizedPlate, StringComparison.OrdinalIgnoreCase)))
            {
                if (!workshop.HasFreeSpace())
                {
                    throw new InvalidOperationException($"No hay espacio disponible en el taller '{workshop.Name}' para recibir el vehiculo.");
                }
                workshop.CurrentVehicles.Add(normalizedPlate);
            }

            WorkOrder order = new WorkOrder
            {
                WorkshopId = workshopId,
                VehiclePlate = normalizedPlate,
                CustomerName = customerName.Trim(),
                CustomerPhone = customerPhone.Trim(),
                Description = description.Trim(),
                EstimatedCost = estimatedCost,
                Status = WorkOrderStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            };

            _repository.AddWorkOrder(order);
            return order;
        }

        public IEnumerable<WorkOrder> GetAllWorkOrders()
        {
            return _repository.GetAllWorkOrders();
        }

        public WorkOrder GetWorkOrderById(Guid id)
        {
            var order = _repository.GetWorkOrderById(id);
            if (order == null)
            {
                throw new KeyNotFoundException("No se encontro la orden de trabajo especificada.");
            }
            return order;
        }

        public void MarkWorkOrderAsCompleted(Guid orderId)
        {
            var order = GetWorkOrderById(orderId);
            if (order.Status != WorkOrderStatus.InProgress)
            {
                throw new InvalidOperationException($"No se puede marcar como completada una orden en estado {order.Status}.");
            }

            order.Status = WorkOrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
        }

        public WorkOrder PayWorkOrder(Guid orderId)
        {
            var order = GetWorkOrderById(orderId);

            if (order.Status == WorkOrderStatus.Paid)
            {
                throw new InvalidOperationException("Esta orden de trabajo ya ha sido pagada previamente.");
            }

            if (order.Status == WorkOrderStatus.Delivered)
            {
                throw new InvalidOperationException("El vehiculo ya fue entregado y la orden concluida.");
            }

            if (order.Status != WorkOrderStatus.Completed)
            {
                throw new InvalidOperationException($"La orden de trabajo no puede ser pagada porque su estado actual es '{order.Status}'. Debe estar 'Completed'.");
            }

            order.Status = WorkOrderStatus.Paid;
            order.PaidAt = DateTime.UtcNow;
            return order;
        }

        public WorkOrder DeliverVehicle(Guid orderId)
        {
            var order = GetWorkOrderById(orderId);

            if (order.Status != WorkOrderStatus.Paid)
            {
                throw new InvalidOperationException($"El vehiculo no puede ser entregado porque la orden de trabajo no ha sido pagada (Estado actual: '{order.Status}').");
            }

            var workshop = GetWorkshopById(order.WorkshopId);
            workshop.CurrentVehicles.RemoveAll(p => p.Equals(order.VehiclePlate, StringComparison.OrdinalIgnoreCase));

            order.Status = WorkOrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            return order;
        }
    }
}
