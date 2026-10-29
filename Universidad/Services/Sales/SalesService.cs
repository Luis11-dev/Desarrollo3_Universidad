using Universidad.Models;
using Universidad.Repositories.Sales;
using Universidad.Repositories.Stock;
using System;
using System.Collections.Generic;

namespace Universidad.Services.Sales
{
    public class SalesService
    {
        private readonly SalesRepository _salesRepository;
        private readonly ShowroomStockRepository _showroomStockRepository;

        public SalesService(SalesRepository salesRepository, ShowroomStockRepository showroomStockRepository)
        {
            _salesRepository = salesRepository;
            _showroomStockRepository = showroomStockRepository;
        }

        public IEnumerable<Showroom> GetAllShowrooms()
        {
            return _showroomStockRepository.GetAllShowrooms();
        }

        public IEnumerable<Sale> GetAllSales()
        {
            return _salesRepository.GetAll();
        }

        public Showroom? GetShowroomById(Guid showroomId)
        {
            return _showroomStockRepository.GetShowroomById(showroomId);
        }

        public IEnumerable<Models.Stock> GetShowroomVehicles(Guid showroomId)
        {
            return _showroomStockRepository.GetStocksByShowroom(showroomId);
        }

        public Models.Stock? GetStockItem(Guid stockId)
        {
            return _showroomStockRepository.GetStockById(stockId);
        }

        public Sale ProcessSaleTransaction(Guid stockId, string customerName, string customerEmail)
        {
            var stockItem = _showroomStockRepository.GetStockById(stockId);

            if (stockItem == null)
            {
                throw new KeyNotFoundException("No se encontro el registro de inventario seleccionado.");
            }

            if (stockItem.Quantity <= 0)
            {
                throw new InvalidOperationException("El vehiculo seleccionado no cuenta con unidades disponibles en stock.");
            }

            if (stockItem.Vehicle == null)
            {
                throw new InvalidOperationException("El vehiculo asociado al inventario no es valido.");
            }

            int initialQuantity = stockItem.Quantity;

            try
            {
                decimal priceToCharge = stockItem.Vehicle.CalculateSalePrice();
                if (priceToCharge <= 0)
                {
                    throw new InvalidOperationException("El precio del vehiculo debe ser mayor a 0.");
                }

                bool quantityUpdated = _showroomStockRepository.UpdateStockQuantity(stockId, initialQuantity - 1);
                if (!quantityUpdated)
                {
                    throw new InvalidOperationException("Fallo al actualizar la cantidad de inventario.");
                }

                Sale sale = new Sale
                {
                    VehicleId = stockItem.Vehicle.Id,
                    Vehicle = stockItem.Vehicle,
                    CustomerName = customerName,
                    CustomerEmail = customerEmail,
                    SoldPrice = priceToCharge,
                    SaleDate = DateTime.UtcNow,
                    Status = SaleStatus.Completed
                };

                _salesRepository.Add(sale);
                return sale;
            }
            catch (Exception)
            {
                _showroomStockRepository.UpdateStockQuantity(stockId, initialQuantity);
                throw;
            }
        }
    }
}
