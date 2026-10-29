using CarDealership.Models;
using CarDealership.Services.Stock;
using CarDealership.Util;
using System;
using System.Collections.Generic;

namespace CarDealership.Commands.Stock
{
    public class AddVehicleStockCommand : ICommand
    {
        private readonly StockService _stockService;
        private readonly IConsoleWriter _writer;

        private readonly Dictionary<int, Func<Vehicle>> _vehicleFactories;

        public AddVehicleStockCommand(StockService stockService, IConsoleWriter writer)
        {
            _stockService = stockService;
            _writer = writer;

            _vehicleFactories = new Dictionary<int, Func<Vehicle>>
            {
                { 1, CreateAutomobile },
                { 2, CreateMotorcycle },
                { 3, CreateTruck }
            };
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("INGRESAR VEHICULO A INVENTARIO");

            try
            {
                _writer.DisplayMenu(
                    "Seleccione el tipo de vehiculo: ",
                    "Automovil",
                    "Motocicleta",
                    "Camion"
                );

                string typeInput = new Validator("Ingrese el tipo (1-3): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 3)
                    .ReadAndValidate();

                int vehicleType = int.Parse(typeInput);

                if (_vehicleFactories.TryGetValue(vehicleType, out var factory))
                {
                    Vehicle vehicle = factory();
                    _stockService.AddVehicle(vehicle);
                    _writer.ShowMessage("\nVehiculo registrado exitosamente en el inventario.");
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

        private (string plate, string vin, string make, string model, int year, string color, decimal price, int mileage) ReadCommonData()
        {
            string plate = new Validator("Ingrese la placa: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim()
                .ToUpper();

            string vin = new Validator("Ingrese el numero VIN: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string make = new Validator("Ingrese la marca: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string model = new Validator("Ingrese el modelo: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string yearInput = new Validator("Ingrese el anio: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1900, 2030)
                .ReadAndValidate();
            int year = int.Parse(yearInput);

            string color = new Validator("Ingrese el color: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string priceInput = new Validator("Ingrese el precio base: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();
            decimal price = decimal.Parse(priceInput);

            string mileageInput = new Validator("Ingrese el kilometraje: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();
            int mileage = int.Parse(mileageInput);

            return (plate, vin, make, model, year, color, price, mileage);
        }

        private Automobile CreateAutomobile()
        {
            var common = ReadCommonData();

            string doorsInput = new Validator("Ingrese el numero de puertas (2-5): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(2, 5)
                .ReadAndValidate();

            string capacityInput = new Validator("Ingrese capacidad de pasajeros (1-10): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 10)
                .ReadAndValidate();

            string transmission = new Validator("Ingrese la transmision (Manual/Automatica): ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            return new Automobile
            {
                LicensePlate = common.plate,
                Vin = common.vin,
                Make = common.make,
                Model = common.model,
                Year = common.year,
                Color = common.color,
                BasePrice = common.price,
                Mileage = common.mileage,
                NumberOfDoors = int.Parse(doorsInput),
                PassengerCapacity = int.Parse(capacityInput),
                Transmission = transmission
            };
        }

        private Motorcycle CreateMotorcycle()
        {
            var common = ReadCommonData();

            string displacementInput = new Validator("Ingrese el cilindraje en CC: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();

            string sidecarInput = new Validator("Tiene sidecar? (1: Si / 2: No): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 2)
                .ReadAndValidate();

            return new Motorcycle
            {
                LicensePlate = common.plate,
                Vin = common.vin,
                Make = common.make,
                Model = common.model,
                Year = common.year,
                Color = common.color,
                BasePrice = common.price,
                Mileage = common.mileage,
                EngineDisplacementCc = int.Parse(displacementInput),
                HasSidecar = sidecarInput == "1"
            };
        }

        private Truck CreateTruck()
        {
            var common = ReadCommonData();

            string loadInput = new Validator("Ingrese la capacidad de carga en toneladas: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();

            string towingInput = new Validator("Ingrese la capacidad de remolque en toneladas: ")
                .NotEmpty()
                .OnlyDigits()
                .ReadAndValidate();

            string axleInput = new Validator("Ingrese la cantidad de ejes: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(2, 10)
                .ReadAndValidate();

            return new Truck
            {
                LicensePlate = common.plate,
                Vin = common.vin,
                Make = common.make,
                Model = common.model,
                Year = common.year,
                Color = common.color,
                BasePrice = common.price,
                Mileage = common.mileage,
                LoadCapacityTons = double.Parse(loadInput),
                TowingCapacityTons = double.Parse(towingInput),
                AxleCount = int.Parse(axleInput)
            };
        }
    }
}
