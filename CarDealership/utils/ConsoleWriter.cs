using CarDealership.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Util
{
    public class ConsoleWriter : IConsoleWriter
    {
        public void DisplayInitialMessage(string name)
        {
            int totalWidth = 35;
            char filler = '*';
            char subLine = '-';

            string centeredMessage = name
                .PadLeft(((totalWidth - name.Length) / 2) + name.Length, filler)
                .PadRight(totalWidth, filler);

            string bottomLine = new string(subLine, totalWidth);

            Console.WriteLine(centeredMessage);
            Console.WriteLine(bottomLine);
        }

        public void DisplayMenu(string headMessage, params string[] options)
        {
            int i = 1;
            Console.WriteLine(headMessage);
            foreach (var item in options)
            {
                Console.WriteLine($"{i}. {item}");
                i++;
            }
            Console.WriteLine(new string('-', 35));
        }

        public void ShowHeader(string title)
        {
            Console.WriteLine("\n=======================================================================================================");
            int totalWidth = 103;
            int padding = (totalWidth - title.Length) / 2;
            string centered = title.PadLeft(padding + title.Length).PadRight(totalWidth);
            Console.WriteLine(centered);
            Console.WriteLine("=======================================================================================================");
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void ShowError(string error)
        {
            Console.WriteLine($"\nError: {error}");
        }

        public void DisplayVehiclesTable(IEnumerable<Vehicle> vehicles)
        {
            var list = vehicles.ToList();
            if (!list.Any())
            {
                Console.WriteLine("No hay vehiculos registrados en el inventario.");
                return;
            }

            string format = "| {0,-10} | {1,-12} | {2,-14} | {3,-6} | {4,-10} | {5,-10} | {6,-12} | {7,-12} |";
            Console.WriteLine(format, "Placa", "Tipo", "Marca/Modelo", "Anio", "Color", "Km", "Precio Base", "Extra");
            Console.WriteLine(new string('-', 103));

            foreach (var v in list)
            {
                string vehicleType = "Vehiculo";
                string extra = string.Empty;

                if (v is Automobile auto)
                {
                    vehicleType = "Automovil";
                    extra = $"{auto.NumberOfDoors}p, {auto.Transmission}";
                }
                else if (v is Motorcycle moto)
                {
                    vehicleType = "Moto";
                    string sidecarText = moto.HasSidecar ? "Sidecar" : "";
                    extra = $"{moto.EngineDisplacementCc}cc {sidecarText}".Trim();
                }
                else if (v is Truck truck)
                {
                    vehicleType = "Camion";
                    extra = $"{truck.LoadCapacityTons}t carga";
                }

                string makeModel = $"{v.Make} {v.Model}";
                if (makeModel.Length > 14)
                {
                    makeModel = makeModel.Substring(0, 11) + "...";
                }

                Console.WriteLine(
                    format,
                    v.LicensePlate,
                    vehicleType,
                    makeModel,
                    v.Year,
                    v.Color,
                    $"{v.Mileage} km",
                    $"${v.BasePrice:N0}",
                    extra
                );
            }

            Console.WriteLine(new string('-', 103));
            Console.WriteLine($"Total de vehiculos: {list.Count}");
        }

        public void DisplayShowroomVehiclesTable(IEnumerable<Models.Stock> stocks, bool showOptionCodes = false)
        {
            var list = stocks.ToList();
            if (!list.Any())
            {
                Console.WriteLine("No hay vehiculos disponibles en este Showroom.");
                return;
            }

            if (showOptionCodes)
            {
                string formatWithCode = "| {0,-4} | {1,-16} | {2,-16} | {3,-14} | {4,-6} | {5,-10} | {6,-12} | {7,-6} |";
                Console.WriteLine(formatWithCode, "Cod", "Modelo", "Marca", "Precio Base", "Anio", "Color", "Millaje", "Disp");
                Console.WriteLine(new string('-', 103));

                int code = 1;
                foreach (var item in list)
                {
                    if (item.Vehicle == null)
                    {
                        continue;
                    }

                    var v = item.Vehicle;
                    Console.WriteLine(
                        formatWithCode,
                        code,
                        v.Model,
                        v.Make,
                        $"${v.BasePrice:N2}",
                        v.Year,
                        v.Color,
                        $"{v.Mileage} mi",
                        item.Quantity
                    );
                    code++;
                }

                Console.WriteLine(new string('-', 103));
                Console.WriteLine($"Total de opciones disponibles: {list.Count}");
            }
            else
            {
                string format = "| {0,-18} | {1,-18} | {2,-16} | {3,-6} | {4,-12} | {5,-14} | {6,-6} |";
                Console.WriteLine(format, "Modelo", "Marca", "Precio Base", "Anio", "Color", "Millaje", "Stock");
                Console.WriteLine(new string('-', 103));

                foreach (var item in list)
                {
                    if (item.Vehicle == null)
                    {
                        continue;
                    }

                    var v = item.Vehicle;
                    Console.WriteLine(
                        format,
                        v.Model,
                        v.Make,
                        $"${v.BasePrice:N2}",
                        v.Year,
                        v.Color,
                        $"{v.Mileage} mi",
                        item.Quantity
                    );
                }

                Console.WriteLine(new string('-', 103));
                Console.WriteLine($"Total de vehiculos en exhibicion: {list.Count}");
            }
        }

        public void DisplaySaleSummary(Models.Stock stock)
        {
            if (stock.Vehicle == null)
            {
                return;
            }

            var vehicle = stock.Vehicle;
            Console.WriteLine("\n" + new string('-', 50));
            Console.WriteLine("RESUMEN DEL VEHICULO A COMPRAR");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Modelo:          {vehicle.Model}");
            Console.WriteLine($"Marca:           {vehicle.Make}");
            Console.WriteLine($"Anio:            {vehicle.Year}");
            Console.WriteLine($"Color:           {vehicle.Color}");
            Console.WriteLine($"Millaje:         {vehicle.Mileage} mi");
            Console.WriteLine($"Placa:           {vehicle.LicensePlate}");
            Console.WriteLine($"Precio Base:     ${vehicle.BasePrice:N2}");
            Console.WriteLine($"Precio Venta:    ${vehicle.CalculateSalePrice():N2} (IVA + Impuestos)");
            Console.WriteLine($"Disponibles:     {stock.Quantity} unidad(es)");
            Console.WriteLine(new string('-', 50));
        }

        public void DisplaySaleReceipt(Sale sale)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("COMPROBANTE DE VENTA");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Numero de Venta: {sale.Id}");
            Console.WriteLine($"Fecha y Hora:    {sale.SaleDate.ToLocalTime()}");
            Console.WriteLine($"Cliente:         {sale.CustomerName}");
            Console.WriteLine($"Correo:          {sale.CustomerEmail}");
            Console.WriteLine(new string('-', 50));
            if (sale.Vehicle != null)
            {
                Console.WriteLine($"Vehiculo:        {sale.Vehicle.Make} {sale.Vehicle.Model} ({sale.Vehicle.Year})");
                Console.WriteLine($"Placa:           {sale.Vehicle.LicensePlate}");
                Console.WriteLine($"Color:           {sale.Vehicle.Color}");
            }
            Console.WriteLine($"Total Pagado:    ${sale.SoldPrice:N2}");
            Console.WriteLine($"Estado:          {sale.Status}");
            Console.WriteLine(new string('=', 50));
        }

        public void DisplaySalesTable(IEnumerable<Sale> sales)
        {
            var list = sales.ToList();
            if (!list.Any())
            {
                Console.WriteLine("No se han registrado ventas en el sistema.");
                return;
            }

            string format = "| {0,-36} | {1,-16} | {2,-18} | {3,-12} | {4,-12} |";
            Console.WriteLine(format, "Id Venta", "Cliente", "Vehiculo", "Precio", "Fecha");
            Console.WriteLine(new string('-', 106));

            foreach (var sale in list)
            {
                string vehicleDesc = "N/A";
                if (sale.Vehicle != null)
                {
                    vehicleDesc = $"{sale.Vehicle.Make} {sale.Vehicle.Model}";
                    if (vehicleDesc.Length > 18)
                    {
                        vehicleDesc = vehicleDesc.Substring(0, 15) + "...";
                    }
                }

                string clientName = sale.CustomerName;
                if (clientName.Length > 16)
                {
                    clientName = clientName.Substring(0, 13) + "...";
                }

                Console.WriteLine(
                    format,
                    sale.Id.ToString(),
                    clientName,
                    vehicleDesc,
                    $"${sale.SoldPrice:N2}",
                    sale.SaleDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                );
            }

            Console.WriteLine(new string('-', 106));
            Console.WriteLine($"Total de ventas registradas: {list.Count}");
        }

        public void DisplayWorkOrdersTable(IEnumerable<CarDealership.Models.ServiceCenter.WorkOrder> orders)
        {
            var list = orders.ToList();
            if (!list.Any())
            {
                Console.WriteLine("No hay ordenes de trabajo registradas.");
                return;
            }

            string format = "| {0,-4} | {1,-10} | {2,-16} | {3,-20} | {4,-10} | {5,-12} |";
            Console.WriteLine(format, "Num", "Placa", "Cliente", "Descripcion", "Costo Est", "Estado");
            Console.WriteLine(new string('-', 88));

            int num = 1;
            foreach (var order in list)
            {
                string desc = order.Description;
                if (desc.Length > 20)
                {
                    desc = desc.Substring(0, 17) + "...";
                }

                string customer = order.CustomerName;
                if (customer.Length > 16)
                {
                    customer = customer.Substring(0, 13) + "...";
                }

                Console.WriteLine(
                    format,
                    num,
                    order.VehiclePlate,
                    customer,
                    desc,
                    $"${order.EstimatedCost:N2}",
                    order.Status.ToString()
                );
                num++;
            }

            Console.WriteLine(new string('-', 88));
            Console.WriteLine($"Total de ordenes: {list.Count}");
        }

        public void DisplayWorkOrderDetail(CarDealership.Models.ServiceCenter.WorkOrder order)
        {
            Console.WriteLine("\n" + new string('-', 50));
            Console.WriteLine("DETALLE DE ORDEN DE TRABAJO");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Numero de Orden: {order.Id}");
            Console.WriteLine($"Placa Vehiculo:  {order.VehiclePlate}");
            Console.WriteLine($"Cliente:         {order.CustomerName}");
            Console.WriteLine($"Telefono:        {order.CustomerPhone}");
            Console.WriteLine($"Descripcion:     {order.Description}");
            Console.WriteLine($"Costo Estimado:  ${order.EstimatedCost:N2}");
            Console.WriteLine($"Estado:          {order.Status}");
            Console.WriteLine($"Fecha Creacion:  {order.CreatedAt.ToLocalTime()}");
            if (order.CompletedAt != null)
            {
                Console.WriteLine($"Fecha Terminada: {order.CompletedAt.Value.ToLocalTime()}");
            }
            if (order.PaidAt != null)
            {
                Console.WriteLine($"Fecha Pago:      {order.PaidAt.Value.ToLocalTime()}");
            }
            if (order.DeliveredAt != null)
            {
                Console.WriteLine($"Fecha Entrega:   {order.DeliveredAt.Value.ToLocalTime()}");
            }
            Console.WriteLine(new string('-', 50));
        }

        public void DisplayVehicleDetail(Vehicle vehicle)
        {
            Console.WriteLine("\n" + new string('-', 45));
            Console.WriteLine("DETALLE DEL VEHICULO");
            Console.WriteLine(new string('-', 45));
            Console.WriteLine($"Placa:         {vehicle.LicensePlate}");
            Console.WriteLine($"VIN:           {vehicle.Vin}");
            Console.WriteLine($"Marca / Mod:   {vehicle.Make} {vehicle.Model}");
            Console.WriteLine($"Anio:          {vehicle.Year}");
            Console.WriteLine($"Color:         {vehicle.Color}");
            Console.WriteLine($"Kilometraje:   {vehicle.Mileage} km");
            Console.WriteLine($"Precio Base:   ${vehicle.BasePrice:N2}");

            if (vehicle is Automobile auto)
            {
                Console.WriteLine("Tipo:          Automovil");
                Console.WriteLine($"Puertas:       {auto.NumberOfDoors}");
                Console.WriteLine($"Pasajeros:     {auto.PassengerCapacity}");
                Console.WriteLine($"Transmision:   {auto.Transmission}");
            }
            else if (vehicle is Motorcycle moto)
            {
                Console.WriteLine("Tipo:          Motocicleta");
                Console.WriteLine($"Cilindraje:    {moto.EngineDisplacementCc} cc");
                Console.WriteLine($"Tiene Sidecar: {(moto.HasSidecar ? "Si" : "No")}");
            }
            else if (vehicle is Truck truck)
            {
                Console.WriteLine("Tipo:          Camion");
                Console.WriteLine($"Carga util:    {truck.LoadCapacityTons} ton");
                Console.WriteLine($"Remolque:      {truck.TowingCapacityTons} ton");
                Console.WriteLine($"Ejes:          {truck.AxleCount}");
            }

            Console.WriteLine(new string('-', 45));
        }

        public void WaitForKey(string prompt = "Presione cualquier tecla para continuar...")
        {
            Console.WriteLine($"\n{prompt}");
            Console.ReadKey();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
