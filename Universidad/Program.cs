using Universidad.Controllers;
using Universidad.Util;
using System;

namespace Universidad
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConsoleWriter consoleWriter = Startup.InitConsoleWriter();
            SalesController salesController = Startup.InitSalesController(consoleWriter);

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
                    "Salir"
                );

                input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();

                option = int.Parse(input);

                switch (option)
                {
                    case 1:
                        salesController.DisplayMenu();
                        break;
                    case 2:
                        endProgram = true;
                        break;
                }

            } while (!endProgram);
        }
    }
}
