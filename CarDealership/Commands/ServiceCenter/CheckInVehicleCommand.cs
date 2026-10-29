using CarDealership.Models;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System;
using System.Linq;

namespace CarDealership.Commands.ServiceCenter
{
    public class CheckInVehicleCommand : ICommand
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;

        public CheckInVehicleCommand(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("INGRESAR VEHICULO A TALLER");

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
            _writer.DisplayMenu("Talleres disponibles:", workshopNames);

            string workshopInput = new Validator("Seleccione el taller destino: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, workshops.Count)
                .ReadAndValidate();

            int index = int.Parse(workshopInput) - 1;
            var workshop = workshops[index];

            var spaceInfo = _service.CheckWorkshopSpace(workshop.Id);
            if (!spaceInfo.HasSpace)
            {
                _writer.ShowError($"El taller '{workshop.Name}' ya tiene todos sus espacios ocupados ({spaceInfo.Capacity}/{spaceInfo.Capacity}). No es posible ingresar otro vehiculo.");
                _writer.WaitForKey();
                return;
            }

            string plate = new Validator("Ingrese la placa del vehiculo a ingresar: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim()
                .ToUpper();

            try
            {
                _service.CheckInVehicle(workshop.Id, plate);
                _writer.ShowMessage($"\nVehiculo con placa '{plate}' ingresado exitosamente al taller '{workshop.Name}'.");
                _writer.ShowMessage($"Espacios disponibles restantes: {workshop.AvailableSpaces()}");
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
