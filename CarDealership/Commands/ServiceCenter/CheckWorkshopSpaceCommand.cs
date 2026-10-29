using CarDealership.Models;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System.Linq;

namespace CarDealership.Commands.ServiceCenter
{
    public class CheckWorkshopSpaceCommand : ICommand
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;

        public CheckWorkshopSpaceCommand(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("VERIFICAR ESPACIO EN TALLER");

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
                workshopNames[i] = $"{workshops[i].Name} ({workshops[i].Address})";
            }
            _writer.DisplayMenu("Talleres disponibles:", workshopNames);

            string input = new Validator("Seleccione el taller a consultar: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, workshops.Count)
                .ReadAndValidate();

            int index = int.Parse(input) - 1;
            var workshop = workshops[index];

            var spaceInfo = _service.CheckWorkshopSpace(workshop.Id);

            _writer.ShowMessage($"\n--- Estado del Taller: {workshop.Name} ---");
            _writer.ShowMessage($"Capacidad Total:    {spaceInfo.Capacity} espacios");
            _writer.ShowMessage($"Espacios Ocupados:  {spaceInfo.Occupied} vehiculos");
            _writer.ShowMessage($"Espacios Libres:    {spaceInfo.Available} espacios disponibles");

            if (spaceInfo.HasSpace)
            {
                _writer.ShowMessage("\nEl taller CUENTA con espacio libre para recibir vehiculos.");
            }
            else
            {
                _writer.ShowMessage("\nEl taller NO cuenta con espacio disponible (esta lleno).");
            }

            if (workshop.CurrentVehicles.Count > 0)
            {
                _writer.ShowMessage($"Vehiculos actualmente en taller: {string.Join(", ", workshop.CurrentVehicles)}");
            }

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
