using CarDealership.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Repositories.Stock
{
    public class ShowroomStockRepository
    {
        private readonly List<Models.Stock> _stocks = new List<Models.Stock>();
        private readonly List<Showroom> _showrooms = new List<Showroom>();

        public void AddShowroom(Showroom showroom)
        {
            _showrooms.Add(showroom);
        }

        public IEnumerable<Showroom> GetAllShowrooms()
        {
            return _showrooms.ToList();
        }

        public Showroom? GetShowroomById(Guid id)
        {
            return _showrooms.FirstOrDefault(s => s.Id == id);
        }

        public void AddStock(Models.Stock stock)
        {
            if (stock.Vehicle != null && !string.IsNullOrWhiteSpace(stock.Vehicle.LicensePlate))
            {
                bool exists = _stocks.Any(s => s.Vehicle != null &&
                    s.Vehicle.LicensePlate.Equals(stock.Vehicle.LicensePlate, StringComparison.OrdinalIgnoreCase) &&
                    s.LocationId == stock.LocationId);

                if (exists)
                {
                    throw new CarDealership.Util.PlacaDuplicadaException(stock.Vehicle.LicensePlate,
                        $"El vehiculo con placa '{stock.Vehicle.LicensePlate}' ya esta registrado en este showroom.");
                }
            }

            _stocks.Add(stock);
        }

        public IEnumerable<Models.Stock> GetAllStocks()
        {
            return _stocks.ToList();
        }

        public IEnumerable<Models.Stock> GetStocksByShowroom(Guid showroomId)
        {
            return _stocks
                .Where(s => s.LocationType == LocationType.Showroom && s.LocationId == showroomId && s.Quantity > 0)
                .ToList();
        }

        public Models.Stock? GetStockById(Guid stockId)
        {
            return _stocks.FirstOrDefault(s => s.Id == stockId);
        }

        public bool UpdateStockQuantity(Guid stockId, int newQuantity)
        {
            var item = GetStockById(stockId);
            if (item == null)
            {
                return false;
            }

            item.Quantity = newQuantity;
            item.LastUpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}
