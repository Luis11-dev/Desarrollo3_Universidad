using CarDealership.Controllers;
using CarDealership.Util;
using System;

namespace CarDealership
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConsoleWriter consoleWriter = Startup.InitConsoleWriter();
            StockController stockController = Startup.InitStockController(consoleWriter);
            SalesController salesController = Startup.InitSalesController(consoleWriter);
            ServiceCenterController serviceCenterController = Startup.InitServiceCenterController(consoleWriter);

            bool endProgram = false;
            string input = string.Empty;
            int option = -1;

            do
            {
                consoleWriter.Clear();
                consoleWriter.DisplayInitialMessage("Car Dealership");
                consoleWriter.DisplayMenu(
                    "Escoja una de las siguientes opciones: ",
                    "Modulo de ventas",
                    "Modulo de inventario",
                    "Modulo de autoservicio",
                    "Salir"
                );

                input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 4)
                    .ReadAndValidate();

                option = int.Parse(input);

                switch (option)
                {
                    case 1:
                        salesController.DisplayMenu();
                        break;
                    case 2:
                        stockController.DisplayMenu();
                        break;
                    case 3:
                        serviceCenterController.DisplayMenu();
                        break;
                    case 4:
                        endProgram = true;
                        break;
                }

            } while (!endProgram);
        }
    }
}
