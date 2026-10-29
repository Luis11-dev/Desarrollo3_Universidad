using CarDealership.Commands.ServiceCenter;
using CarDealership.Models;
using CarDealership.Services.ServiceCenter;
using CarDealership.Util;
using System.Collections.Generic;

namespace CarDealership.Controllers
{
    public class ServiceCenterController
    {
        private readonly ServiceCenterService _service;
        private readonly IConsoleWriter _writer;
        private readonly Dictionary<int, ICommand> _commands;

        public ServiceCenterController(ServiceCenterService service, IConsoleWriter writer)
        {
            _service = service;
            _writer = writer;

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new CheckWorkshopSpaceCommand(_service, _writer) },
                { 2, new CheckInVehicleCommand(_service, _writer) },
                { 3, new CreateWorkOrderCommand(_service, _writer) },
                { 4, new DeliverVehicleCommand(_service, _writer) },
                { 5, new PayWorkOrderCommand(_service, _writer) }
            };
        }

        public void DisplayMenu()
        {
            bool exitMenu = false;
            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("MODULO DE AUTOSERVICIO (SERVICE CENTER)");
                _writer.DisplayMenu(
                    "Seleccione una opcion: ",
                    "Verificar espacio en taller",
                    "Ingresar vehiculo a taller",
                    "Crear orden de trabajo",
                    "Entregar vehiculo",
                    "Pagar orden de trabajo",
                    "Volver al menu principal"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 6)
                    .ReadAndValidate();

                int option = int.Parse(input);

                if (option == 6)
                {
                    exitMenu = true;
                }
                else if (_commands.TryGetValue(option, out ICommand? command))
                {
                    command.Execute();
                }

            } while (!exitMenu);
        }
    }
}
