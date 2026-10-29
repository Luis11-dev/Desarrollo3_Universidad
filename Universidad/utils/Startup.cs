using Universidad.Controllers;
using Universidad.Data;
using Universidad.Models;
using Universidad.Models.ServiceCenter;
using Universidad.Repositories.Sales;
using Universidad.Repositories.ServiceCenter;
using Universidad.Repositories.Stock;
using Universidad.Services.Sales;
using Universidad.Services.ServiceCenter;
using Universidad.Services.Stock;
using System.Collections.Generic;

namespace Universidad.Util
{
    public static class Startup
    {
        private static ShowroomStockRepository? _showroomStockRepository;
        private static SalesRepository? _salesRepository;
        private static ServiceCenterRepository? _serviceCenterRepository;

        public static ShowroomStockRepository GetShowroomStockRepository()
        {
            if (_showroomStockRepository == null)
            {
                _showroomStockRepository = new ShowroomStockRepository();

                Showroom mainShowroom = new Showroom
                {
                    Name = "Autos Garcia Sucursal Managua",
                    Address = "De plaza sesamo, 3 cuadras al Norte",
                    DisplayCapacity = 20,
                    IsActive = true
                };

                Showroom northShowroom = new Showroom
                {
                    Name = "Autos Garcia Sucursal Esteli",
                    Address = "Cueva del duende, gira a la derecha por la cucamonga ",
                    DisplayCapacity = 15,
                    IsActive = true
                };

                _showroomStockRepository.AddShowroom(mainShowroom);
                _showroomStockRepository.AddShowroom(northShowroom);

                List<Vehicle> initialVehicles = Seeds.CreateVehicles();

                if (initialVehicles.Count > 0)
                {
                    _showroomStockRepository.AddStock(new Models.Stock
                    {
                        VehicleId = initialVehicles[0].Id,
                        Vehicle = initialVehicles[0],
                        LocationType = LocationType.Showroom,
                        LocationId = mainShowroom.Id,
                        Quantity = 3
                    });
                }

                if (initialVehicles.Count > 1)
                {
                    _showroomStockRepository.AddStock(new Models.Stock
                    {
                        VehicleId = initialVehicles[1].Id,
                        Vehicle = initialVehicles[1],
                        LocationType = LocationType.Showroom,
                        LocationId = mainShowroom.Id,
                        Quantity = 2
                    });
                }

                if (initialVehicles.Count > 2)
                {
                    _showroomStockRepository.AddStock(new Models.Stock
                    {
                        VehicleId = initialVehicles[2].Id,
                        Vehicle = initialVehicles[2],
                        LocationType = LocationType.Showroom,
                        LocationId = northShowroom.Id,
                        Quantity = 1
                    });
                }
            }

            return _showroomStockRepository;
        }

        public static SalesRepository GetSalesRepository()
        {
            if (_salesRepository == null)
            {
                _salesRepository = new SalesRepository();
            }

            return _salesRepository;
        }

        public static ServiceCenterRepository GetServiceCenterRepository()
        {
            if (_serviceCenterRepository == null)
            {
                _serviceCenterRepository = new ServiceCenterRepository();

                Workshop workshopCentral = new Workshop
                {
                    Name = "Taller Mecanico Central",
                    Address = "Av. Pajuelia casa #450",
                    Capacity = 3,
                    CurrentVehicles = new List<string> { "ABC-123" }
                };

                Workshop workshopExpress = new Workshop
                {
                    Name = "Taller Mantenimiento Express",
                    Address = "Parque Industrial Norte",
                    Capacity = 2,
                    CurrentVehicles = new List<string>()
                };

                _serviceCenterRepository.AddWorkshop(workshopCentral);
                _serviceCenterRepository.AddWorkshop(workshopExpress);

                WorkOrder sampleOrder = new WorkOrder
                {
                    WorkshopId = workshopCentral.Id,
                    VehiclePlate = "ABC-123",
                    CustomerName = "Carlos Gomez",
                    CustomerPhone = "9988-7766",
                    Description = "Cambio de aceite, filtro y revision de frenos",
                    EstimatedCost = 150.00m,
                    Status = WorkOrderStatus.Completed
                };

                _serviceCenterRepository.AddWorkOrder(sampleOrder);
            }

            return _serviceCenterRepository;
        }

        public static StockService InitStockService()
        {
            StockRepository stockRepository = new StockRepository();
            foreach (var vehicle in Seeds.CreateVehicles())
            {
                stockRepository.Add(vehicle);
            }
            return new StockService(stockRepository);
        }

        public static SalesService InitSalesService()
        {
            SalesRepository salesRepo = GetSalesRepository();
            ShowroomStockRepository showroomRepo = GetShowroomStockRepository();
            return new SalesService(salesRepo, showroomRepo);
        }

        public static ServiceCenterService InitServiceCenterService()
        {
            ServiceCenterRepository repo = GetServiceCenterRepository();
            return new ServiceCenterService(repo);
        }

        public static IConsoleWriter InitConsoleWriter()
        {
            return new ConsoleWriter();
        }

        public static StockController InitStockController(IConsoleWriter writer)
        {
            StockService stockService = InitStockService();
            return new StockController(stockService, writer);
        }

        public static SalesController InitSalesController(IConsoleWriter writer)
        {
            SalesService salesService = InitSalesService();
            return new SalesController(salesService, writer);
        }

        public static ServiceCenterController InitServiceCenterController(IConsoleWriter writer)
        {
            ServiceCenterService service = InitServiceCenterService();
            return new ServiceCenterController(service, writer);
        }
    }
}
