using CarDealership.Models;
using System.Collections.Generic;

namespace CarDealership.Util
{
    public interface IConsoleWriter
    {
        void DisplayInitialMessage(string name);
        void DisplayMenu(string headMessage, params string[] options);
        void ShowHeader(string title);
        void ShowMessage(string message);
        void ShowError(string error);
        void DisplayVehiclesTable(IEnumerable<Vehicle> vehicles);
        void DisplayVehicleDetail(Vehicle vehicle);
        void DisplayShowroomVehiclesTable(IEnumerable<Models.Stock> stocks, bool showOptionCodes = false);
        void DisplaySaleSummary(Models.Stock stock);
        void DisplaySaleReceipt(Sale sale);
        void DisplaySalesTable(IEnumerable<Sale> sales);
        void DisplayWorkOrdersTable(IEnumerable<Models.ServiceCenter.WorkOrder> orders);
        void DisplayWorkOrderDetail(Models.ServiceCenter.WorkOrder order);
        void WaitForKey(string prompt = "Presione cualquier tecla para continuar...");
        void Clear();
    }
}
