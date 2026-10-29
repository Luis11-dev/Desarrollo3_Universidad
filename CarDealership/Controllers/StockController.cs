using CarDealership.Commands.Stock;
using CarDealership.Models;
using CarDealership.Services.Stock;
using CarDealership.Util;
using System.Collections.Generic;

namespace CarDealership.Controllers
{
    public class StockController
    {
        private readonly StockService _stockService;
        private readonly IConsoleWriter _writer;
        private readonly Dictionary<int, ICommand> _commands;

        public StockController(StockService stockService, IConsoleWriter writer)
        {
            _stockService = stockService;
            _writer = writer;

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new AddVehicleStockCommand(_stockService, _writer) },
                { 2, new ListVehiclesStockCommand(_stockService, _writer) },
                { 3, new RemoveVehicleStockCommand(_stockService, _writer) },
                { 4, new SearchVehicleByPlateCommand(_stockService, _writer) }
            };
        }

        public void DisplayMenu()
        {
            bool exitMenu = false;
            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("MODULO DE INVENTARIO");
                _writer.DisplayMenu(
                    "Seleccione una opcion: ",
                    "Ingresar vehiculo a inventario",
                    "Listar vehiculos en inventario",
                    "Eliminar vehiculo de inventario",
                    "Buscar por placa",
                    "Volver al menu principal"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 5)
                    .ReadAndValidate();

                int option = int.Parse(input);

                if (option == 5)
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
