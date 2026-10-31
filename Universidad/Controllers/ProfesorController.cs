using System;
using System.Collections.Generic;
using Universidad.Commands.Profesor;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Controllers
{
    public class ProfesorController : IController
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Profesor _profesorActual;
        private readonly Dictionary<int, ICommand> _commands;

        public ProfesorController(ProfesorService profesorService, IConsoleWriter writer, Profesor profesorActual)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new VerEstudiantesGrupoCommand(_profesorService, _writer, _profesorActual) },
                { 2, new ModuloCalificacionesCommand(_profesorService, _writer, _profesorActual) },
                { 3, new MarcarAsistenciaCommand(_profesorService, _writer, _profesorActual) },
                { 4, new ConsultarContratoPlanillaCommand(_profesorService, _writer, _profesorActual) }
            };
        }

        public void DisplayMenu()
        {
            try
            {
                // verificamos que el usuario tenga el rol de Profesor antes de dejarlo entrar al modulo
                _profesorService.ValidarRolProfesor(_profesorActual);
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
                _writer.WaitForKey();
                return;
            }

            bool salirMenu = false;

            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("MODULO DE PROFESOR");
                _writer.ShowMessage($"Bienvenido(a): {_profesorActual.Nombre} | Cedula: {_profesorActual.Cedula}\n");

                _writer.DisplayMenu(
                    "Seleccione una opcion:",
                    "Ver estudiantes de mi grupo",
                    "Modulo de calificaciones (Registrar notas, Ver notas, Convocatoria)",
                    "Marcar asistencia a clase",
                    "Consultar mi contrato y planilla de pago",
                    "Volver al menu principal"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 5)
                    .ReadAndValidate();

                int opcion = int.Parse(input);

                if (opcion == 5)
                {
                    salirMenu = true;
                }
                else if (_commands.TryGetValue(opcion, out ICommand? command))
                {
                    command.Execute();
                }

            } while (!salirMenu);
        }
    }
}
