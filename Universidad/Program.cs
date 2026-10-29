using Universidad.Util;
using System;

namespace Universidad
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConsoleWriter consoleWriter = Startup.InitConsoleWriter();

            bool endProgram = false;
            string input = string.Empty;
            int option = -1;

            do
            {
                consoleWriter.Clear();
                consoleWriter.DisplayInitialMessage("Sistema Universitario");
                consoleWriter.DisplayMenu(
                    "Escoja una de las siguientes opciones: ",
                    "Modulo de Profesores",
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
                        consoleWriter.ShowMessage("\nModulo de Profesores en construccion...");
                        consoleWriter.WaitForKey();
                        break;
                    case 2:
                        endProgram = true;
                        break;
                }

            } while (!endProgram);
        }
    }
}
