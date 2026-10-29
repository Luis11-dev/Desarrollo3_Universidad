using CarDealership.Models;
using CarDealership.Services.Sales;
using CarDealership.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Commands.Sales
{
    public class SellVehicleCommand : ICommand
    {
        private readonly SalesService _salesService;
        private readonly IConsoleWriter _writer;

        public SellVehicleCommand(SalesService salesService, IConsoleWriter writer)
        {
            _salesService = salesService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("VENTA DE VEHICULO");

            var showrooms = _salesService.GetAllShowrooms().ToList();
            if (showrooms.Count == 0)
            {
                _writer.ShowMessage("No existen showrooms registrados en el sistema.");
                _writer.WaitForKey();
                return;
            }

            string[] showroomNames = new string[showrooms.Count];
            for (int i = 0; i < showrooms.Count; i++)
            {
                showroomNames[i] = $"{showrooms[i].Name} ({showrooms[i].Address})";
            }
            _writer.DisplayMenu("Seleccione el Showroom donde se realizara la compra:", showroomNames);

            string showroomInput = new Validator("Ingrese el numero del Showroom: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, showrooms.Count)
                .ReadAndValidate();

            int showroomIndex = int.Parse(showroomInput) - 1;
            var selectedShowroom = showrooms[showroomIndex];

            var availableStocks = _salesService.GetShowroomVehicles(selectedShowroom.Id).ToList();
            if (availableStocks.Count == 0)
            {
                _writer.ShowMessage($"\nNo hay vehiculos con stock disponible en el Showroom '{selectedShowroom.Name}'.");
                _writer.WaitForKey();
                return;
            }

            _writer.Clear();
            _writer.ShowHeader($"VEHICULOS DISPONIBLES EN {selectedShowroom.Name.ToUpper()}");
            _writer.DisplayShowroomVehiclesTable(availableStocks, showOptionCodes: true);

            string optionInput = new Validator("\nIngrese el codigo de la opcion de compra (Cod): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, availableStocks.Count)
                .ReadAndValidate();

            int selectedOptionIndex = int.Parse(optionInput) - 1;
            var chosenStock = availableStocks[selectedOptionIndex];

            _writer.DisplaySaleSummary(chosenStock);

            string confirmPayment = new Validator("Desea pagar el vehiculo? (1: Si / 2: No): ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 2)
                .ReadAndValidate();

            if (confirmPayment != "1")
            {
                _writer.ShowMessage("\nOperacion de compra finalizada sin cambios.");
                _writer.WaitForKey();
                return;
            }

            string customerName = new Validator("Ingrese el nombre completo del cliente: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            string customerEmail = new Validator("Ingrese el correo electronico del cliente: ")
                .NotEmpty()
                .ReadAndValidate()
                .Trim();

            try
            {
                Sale completedSale = _salesService.ProcessSaleTransaction(chosenStock.Id, customerName, customerEmail);
                _writer.ShowMessage("\nEl pago ha sido procesado y el inventario actualizado exitosamente.");
                _writer.DisplaySaleReceipt(completedSale);
            }
            catch (Exception ex)
            {
                _writer.ShowError($"La transaccion fue abortada debido a un error: {ex.Message}");
            }

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
