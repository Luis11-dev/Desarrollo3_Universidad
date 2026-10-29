using CarDealership.Commands.Sales;
using CarDealership.Models;
using CarDealership.Services.Sales;
using CarDealership.Util;
using System.Collections.Generic;

namespace CarDealership.Controllers
{
    public class SalesController
    {
        private readonly SalesService _salesService;
        private readonly IConsoleWriter _writer;
        private readonly Dictionary<int, ICommand> _commands;

        public SalesController(SalesService salesService, IConsoleWriter writer)
        {
            _salesService = salesService;
            _writer = writer;

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new ShowShowroomVehiclesCommand(_salesService, _writer) },
                { 2, new SellVehicleCommand(_salesService, _writer) },
                { 3, new ListSalesCommand(_salesService, _writer) }
            };
        }

        public void DisplayMenu()
        {
            bool exitMenu = false;
            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("MODULO DE VENTAS");
                _writer.DisplayMenu(
                    "Seleccione una opcion: ",
                    "Mostrar vehiculos de un Showroom",
                    "Vender un vehiculo",
                    "Mostrar lista de ventas",
                    "Volver al menu principal"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 4)
                    .ReadAndValidate();

                int option = int.Parse(input);

                if (option == 4)
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
