using CarDealership.Models;
using CarDealership.Models.ServiceCenter;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System;
using System.Linq;

namespace CarDealership.Commands.ServiceCenter
{
    public class DeliverVehicleCommand : ICommand
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;

        public DeliverVehicleCommand(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("ENTREGAR VEHICULO");

            var orders = _service.GetAllWorkOrders().ToList();
            if (orders.Count == 0)
            {
                _writer.ShowMessage("No existen ordenes de trabajo registradas.");
                _writer.WaitForKey();
                return;
            }

            _writer.DisplayWorkOrdersTable(orders);

            string optionInput = new Validator("\nIngrese el numero de la orden a entregar: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, orders.Count)
                .ReadAndValidate();

            int selectedIndex = int.Parse(optionInput) - 1;
            var chosenOrder = orders[selectedIndex];

            _writer.DisplayWorkOrderDetail(chosenOrder);

            if (chosenOrder.Status == WorkOrderStatus.Delivered)
            {
                _writer.ShowMessage("\nEste vehiculo ya fue entregado previamente.");
                _writer.WaitForKey();
                return;
            }

            if (chosenOrder.Status != WorkOrderStatus.Paid)
            {
                _writer.ShowError($"El vehiculo no puede entregarse hasta que la orden este pagada. Estado actual: '{chosenOrder.Status}'.");
                _writer.WaitForKey();
                return;
            }

            string confirm = new Validator("Confirma la entrega del vehiculo al cliente? (1: Si / 2: No): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 2)
                .ReadAndValidate();

            if (confirm != "1")
            {
                _writer.ShowMessage("\nEntrega cancelada.");
                _writer.WaitForKey();
                return;
            }

            try
            {
                WorkOrder deliveredOrder = _service.DeliverVehicle(chosenOrder.Id);
                _writer.ShowMessage($"\nVehiculo con placa '{deliveredOrder.VehiclePlate}' entregado exitosamente al cliente {deliveredOrder.CustomerName}.");
                _writer.ShowMessage("El espacio en el taller ha sido liberado.");
                _writer.DisplayWorkOrderDetail(deliveredOrder);
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
            }

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
