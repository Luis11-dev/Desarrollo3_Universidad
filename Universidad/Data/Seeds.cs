using Universidad.Models;
using System.Collections.Generic;

namespace Universidad.Data
{
    public static class Seeds
    {
        public static List<Vehicle> CreateVehicles()
        {
            return new List<Vehicle>
            {
                new Automobile
                {
                    LicensePlate = "ABC-123",
                    Vin = "1HGCR2F83HA123456",
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2022,
                    Color = "Gris",
                    BasePrice = 21000m,
                    Mileage = 15000,
                    NumberOfDoors = 4,
                    PassengerCapacity = 5,
                    Transmission = "Automatica"
                },
                new Motorcycle
                {
                    LicensePlate = "XYZ-789",
                    Vin = "JH2PC37038M123456",
                    Make = "Yamaha",
                    Model = "MT-07",
                    Year = 2023,
                    Color = "Negro",
                    BasePrice = 8500m,
                    Mileage = 3200,
                    EngineDisplacementCc = 689,
                    HasSidecar = false
                },
                new Truck
                {
                    LicensePlate = "TRK-456",
                    Vin = "1FTFW1ED4MFA12345",
                    Make = "Ford",
                    Model = "F-150",
                    Year = 2021,
                    Color = "Blanco",
                    BasePrice = 36000m,
                    Mileage = 42000,
                    LoadCapacityTons = 1.5,
                    TowingCapacityTons = 5.0,
                    AxleCount = 2
                }
            };
        }
    }
}
