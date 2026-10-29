using CarDealership.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Repositories.Stock
{
    public class StockRepository
    {
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();

        public IEnumerable<Vehicle> GetAll()
        {
            return _vehicles.ToList();
        }

        public Vehicle? GetByLicensePlate(string licensePlate)
        {
            return _vehicles.FirstOrDefault(v => v.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase));
        }

        public Vehicle? GetById(Guid id)
        {
            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public bool Add(Vehicle vehicle)
        {
            if (GetByLicensePlate(vehicle.LicensePlate) != null)
            {
                return false;
            }
            _vehicles.Add(vehicle);
            return true;
        }

        public bool Remove(string licensePlate)
        {
            var vehicle = GetByLicensePlate(licensePlate);
            if (vehicle == null)
            {
                return false;
            }
            return _vehicles.Remove(vehicle);
        }
    }
}
