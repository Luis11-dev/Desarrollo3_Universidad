using CarDealership.Models;
using CarDealership.Models.ServiceCenter;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System;
using System.Linq;

namespace CarDealership.Commands.ServiceCenter
{
    public class CreateWorkOrderCommand : ICommand
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;

        public CreateWorkOrderCommand(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("CREAR ORDEN DE TRABAJO");

            var workshops = _service.GetAllWorkshops().ToList();
            if (workshops.Count == 0)
            {
                _writer.ShowMessage("No existen talleres registrados.");
                _writer.WaitForKey();
                return;
            }

            string[] workshopNames = new string[workshops.Count];
            for (int i = 0; i < workshops.Count; i++)
            {
                workshopNames[i] = $"{workshops[i].Name} ({workshops[i].OccupiedSpaces()}/{workshops[i].Capacity} ocupados)";
            }
            _writer.DisplayMenu("Seleccione el taller:", workshopNames);

            string workshopInput = new Validator("Ingrese su opcion: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, workshops.Count)
                .ReadAndValidate();

            int index = int.Parse(workshopInput) - 1;
            var workshop = workshops[index];

            string plate = new Validator("Ingrese la placa del vehiculo: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim()
                .ToUpper();

            string customerName = new Validator("Ingrese el nombre del cliente: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string customerPhone = new Validator("Ingrese el telefono de contacto: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string description = new Validator("Ingrese el motivo / descripcion del trabajo: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string costInput = new Validator("Ingrese el costo estimado del servicio: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();

            decimal cost = decimal.Parse(costInput);

            try
            {
                WorkOrder order = _service.CreateWorkOrder(
                    workshop.Id,
                    plate,
                    customerName,
                    customerPhone,
                    description,
                    cost
                );

                _writer.ShowMessage("\nOrden de trabajo generada exitosamente.");
                _writer.DisplayWorkOrderDetail(order);
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
