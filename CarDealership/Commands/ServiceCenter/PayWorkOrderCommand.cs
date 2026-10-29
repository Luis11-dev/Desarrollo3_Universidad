using CarDealership.Models;
using CarDealership.Models.ServiceCenter;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System;
using System.Linq;

namespace CarDealership.Commands.ServiceCenter
{
    public class PayWorkOrderCommand : ICommand
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;

        public PayWorkOrderCommand(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("PAGAR ORDEN DE TRABAJO");

            var orders = _service.GetAllWorkOrders().ToList();
            if (orders.Count == 0)
            {
                _writer.ShowMessage("No existen ordenes de trabajo registradas en el sistema.");
                _writer.WaitForKey();
                return;
            }

            _writer.DisplayWorkOrdersTable(orders);

            string optionInput = new Validator("\nIngrese el numero de la orden a consultar/pagar: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, orders.Count)
                .ReadAndValidate();

            int selectedIndex = int.Parse(optionInput) - 1;
            var chosenOrder = orders[selectedIndex];

            _writer.DisplayWorkOrderDetail(chosenOrder);

            // Si la orden esta InProgress, preguntar si ya fue completada antes de pagar
            if (chosenOrder.Status == WorkOrderStatus.InProgress)
            {
                _writer.ShowMessage($"\nEl estado de la orden es '{chosenOrder.Status}'.");
                string completeConfirm = new Validator("Desea marcar el trabajo como concluido para proceder al cobro? (1: Si / 2: No): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();

                if (completeConfirm == "1")
                {
                    _service.MarkWorkOrderAsCompleted(chosenOrder.Id);
                    _writer.ShowMessage("Orden marcada como 'Completed'.");
                }
                else
                {
                    _writer.ShowMessage("\nNo es posible procesar el pago de un trabajo que aun no ha concluido.");
                    _writer.WaitForKey();
                    return;
                }
            }

            if (chosenOrder.Status != WorkOrderStatus.Completed)
            {
                _writer.ShowError($"La orden no se encuentra en estado 'Completed' (Estado actual: '{chosenOrder.Status}').");
                _writer.WaitForKey();
                return;
            }

            _writer.ShowMessage($"\nMonto a pagar: ${chosenOrder.EstimatedCost:N2}");
            string payConfirm = new Validator("Desea procesar el pago ahora? (1: Si / 2: No): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 2)
                .ReadAndValidate();

            if (payConfirm != "1")
            {
                _writer.ShowMessage("\nOperacion de pago cancelada.");
                _writer.WaitForKey();
                return;
            }

            try
            {
                WorkOrder paidOrder = _service.PayWorkOrder(chosenOrder.Id);
                _writer.ShowMessage("\nPago procesado exitosamente.");
                _writer.DisplayWorkOrderDetail(paidOrder);
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
