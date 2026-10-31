using System;
using System.Collections.Generic;
using Universidad.Commands.Administrador;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Controllers
{
    public class AdministradorController : IController
    {
        private readonly ProfesorService _profesorService;
        private readonly EstudianteService _estudianteService;
        private readonly IConsoleWriter _writer;
        private readonly Administrador _adminActual;
        private readonly Dictionary<int, ICommand> _commands;

        public AdministradorController(
            ProfesorService profesorService,
            EstudianteService estudianteService,
            IConsoleWriter writer,
            Administrador adminActual)
        {
            _profesorService = profesorService;
            _estudianteService = estudianteService;
            _writer = writer;
            _adminActual = adminActual;

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new RegistrarProfesorCommand(_profesorService, _writer) },
                { 2, new RegistrarEstudianteCommand(_estudianteService, _writer) }
            };
        }

        public void DisplayMenu()
        {
            if (_adminActual == null || _adminActual.Rol != Rol.Administrador)
            {
                _writer.ShowError("Acceso denegado: El usuario no tiene permisos de Administrador.");
                _writer.WaitForKey();
                return;
            }

            bool salirMenu = false;

            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("MODULO DE ADMINISTRADOR");
                _writer.ShowMessage($"Bienvenido(a): {_adminActual.Nombre} | Rol: Administrador\n");

                _writer.DisplayMenu(
                    "Seleccione una opcion:",
                    "Registrar Profesor",
                    "Registrar Estudiante",
                    "Cerrar sesion (Volver al menu principal)"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 3)
                    .ReadAndValidate();

                int opcion = int.Parse(input);

                if (opcion == 3)
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

