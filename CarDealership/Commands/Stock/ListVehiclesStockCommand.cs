using CarDealership.Models;
using CarDealership.Services.Stock;
using CarDealership.Util;
using System.Collections.Generic;

namespace CarDealership.Commands.Stock
{
    public class ListVehiclesStockCommand : ICommand
    {
        private readonly StockService _stockService;
        private readonly IConsoleWriter _writer;

        public ListVehiclesStockCommand(StockService stockService, IConsoleWriter writer)
        {
            _stockService = stockService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("VEHICULOS EN INVENTARIO");

            IEnumerable<Vehicle> vehicles = _stockService.GetAllVehicles();
            _writer.DisplayVehiclesTable(vehicles);

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
