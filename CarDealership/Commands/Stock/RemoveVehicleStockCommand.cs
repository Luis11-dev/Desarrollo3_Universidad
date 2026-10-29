using CarDealership.Models;
using CarDealership.Services.Stock;
using CarDealership.Util;
using System;

namespace CarDealership.Commands.Stock
{
    public class RemoveVehicleStockCommand : ICommand
    {
        private readonly StockService _stockService;
        private readonly IConsoleWriter _writer;

        public RemoveVehicleStockCommand(StockService stockService, IConsoleWriter writer)
        {
            _stockService = stockService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("ELIMINAR VEHICULO DE INVENTARIO");

            try
            {
                string plate = new Validator("Ingrese la placa del vehiculo a eliminar: ")
                    .NotEmpty()
                    .ReadAndValidate()
                    .Trim()
                    .ToUpper();

                var vehicle = _stockService.GetVehicleByLicensePlate(plate);
                _writer.ShowMessage($"\nVehiculo encontrado: {vehicle.Make} {vehicle.Model} ({vehicle.Year}) - Color: {vehicle.Color}");

                string confirm = new Validator("Esta seguro de eliminar este vehiculo? (1: Si / 2: No): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();

                if (confirm == "1")
                {
                    _stockService.RemoveVehicle(plate);
                    _writer.ShowMessage("\nVehiculo eliminado exitosamente del inventario.");
                }
                else
                {
                    _writer.ShowMessage("\nOperacion cancelada.");
                }
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
