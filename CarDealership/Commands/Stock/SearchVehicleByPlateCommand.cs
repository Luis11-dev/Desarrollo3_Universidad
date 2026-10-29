using CarDealership.Models;
using CarDealership.Services.Stock;
using CarDealership.Util;
using System;

namespace CarDealership.Commands.Stock
{
    public class SearchVehicleByPlateCommand : ICommand
    {
        private readonly StockService _stockService;
        private readonly IConsoleWriter _writer;

        public SearchVehicleByPlateCommand(StockService stockService, IConsoleWriter writer)
        {
            _stockService = stockService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("BUSCAR VEHICULO POR PLACA");

            try
            {
                string plate = new Validator("Ingrese la placa a buscar: ")
                    .NotEmpty()
                    .ReadAndValidate()
                    .Trim()
                    .ToUpper();

                var vehicle = _stockService.GetVehicleByLicensePlate(plate);
                _writer.DisplayVehicleDetail(vehicle);
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
