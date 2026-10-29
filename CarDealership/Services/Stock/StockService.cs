using CarDealership.Models;
using CarDealership.Repositories.Stock;
using System;
using System.Collections.Generic;

namespace CarDealership.Services.Stock
{
    public class StockService
    {
        private readonly StockRepository _stockRepository;

        public StockService(StockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public bool AddVehicle(Vehicle vehicle)
        {
            if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
            {
                throw new ArgumentException("La placa no puede estar vacía.");
            }

            if (_stockRepository.GetByLicensePlate(vehicle.LicensePlate) != null)
            {
                throw new CarDealership.Util.PlacaDuplicadaException(vehicle.LicensePlate, $"Ya existe un vehiculo registrado con la placa '{vehicle.LicensePlate}' en el inventario.");
            }

            return _stockRepository.Add(vehicle);
        }

        public IEnumerable<Vehicle> GetAllVehicles()
        {
            return _stockRepository.GetAll();
        }

        public Vehicle GetVehicleByLicensePlate(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                throw new ArgumentException("Debe ingresar una placa válida.");
            }

            var vehicle = _stockRepository.GetByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                throw new CarDealership.Util.VehiculoNoEncontradoException(licensePlate, $"No se encontro ningun vehiculo con la placa '{licensePlate}'.");
            }

            return vehicle;
        }

        public bool RemoveVehicle(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                throw new ArgumentException("Debe ingresar una placa válida.");
            }

            var vehicle = _stockRepository.GetByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                throw new CarDealership.Util.VehiculoNoEncontradoException(licensePlate, $"No existe ningun vehiculo registrado con la placa '{licensePlate}'.");
            }

            return _stockRepository.Remove(licensePlate);
        }
    }
}
