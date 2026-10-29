using CarDealership.Models;
using CarDealership.Services.Sales;
using CarDealership.Util;
using System.Collections.Generic;

namespace CarDealership.Commands.Sales
{
    public class ListSalesCommand : ICommand
    {
        private readonly SalesService _salesService;
        private readonly IConsoleWriter _writer;

        public ListSalesCommand(SalesService salesService, IConsoleWriter writer)
        {
            _salesService = salesService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("HISTORIAL DE VENTAS");

            IEnumerable<Sale> sales = _salesService.GetAllSales();
            _writer.DisplaySalesTable(sales);

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
