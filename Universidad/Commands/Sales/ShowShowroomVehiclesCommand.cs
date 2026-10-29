using Universidad.Models;
using Universidad.Services.Sales;
using Universidad.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Commands.Sales
{
    public class ShowShowroomVehiclesCommand : ICommand
    {
        private readonly SalesService _salesService;
        private readonly IConsoleWriter _writer;

        public ShowShowroomVehiclesCommand(SalesService salesService, IConsoleWriter writer)
        {
            _salesService = salesService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("VEHICULOS DE UN SHOWROOM");

            var showrooms = _salesService.GetAllShowrooms().ToList();
            if (showrooms.Count == 0)
            {
                _writer.ShowMessage("No existen showrooms registrados en el sistema.");
                _writer.WaitForKey();
                return;
            }

            _writer.ShowMessage("Seleccione el Showroom a consultar:");
            string[] showroomNames = new string[showrooms.Count];
            for (int i = 0; i < showrooms.Count; i++)
            {
                showroomNames[i] = $"{showrooms[i].Name} ({showrooms[i].Address})";
            }
            _writer.DisplayMenu("Showrooms disponibles:", showroomNames);

            string optionInput = new Validator("Ingrese el numero de Showroom: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, showrooms.Count)
                .ReadAndValidate();

            int selectedIndex = int.Parse(optionInput) - 1;
            var selectedShowroom = showrooms[selectedIndex];

            _writer.Clear();
            _writer.ShowHeader($"VEHICULOS EN SHOWROOM: {selectedShowroom.Name.ToUpper()}");

            var stocks = _salesService.GetShowroomVehicles(selectedShowroom.Id);
            _writer.DisplayShowroomVehiclesTable(stocks, showOptionCodes: false);

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
