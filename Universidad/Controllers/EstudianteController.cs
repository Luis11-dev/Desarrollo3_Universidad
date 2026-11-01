using System;
using System.Collections.Generic;
using Universidad.Commands.Estudiante;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Controllers
{
    public class EstudianteController : IController
    {
        private readonly Estudiante _estudiante;
        private readonly InscripcionService _inscripcionService;
        private readonly ProfesorRepository _profesorRepository;
        private readonly IConsoleWriter _writer;
        private readonly Dictionary<int, ICommand> _commands;

        public EstudianteController(
            Estudiante estudiante,
            InscripcionService inscripcionService,
            ProfesorRepository profesorRepository,
            IConsoleWriter writer)
        {
            _estudiante = estudiante ?? throw new ArgumentNullException(nameof(estudiante));
            _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new VerHorarioClasesCommand(_inscripcionService, _writer, _estudiante) },
                { 2, new InscribirMateriaCommand(_inscripcionService, _writer, _estudiante) },
                { 3, new ConsultarCalificacionesEstudianteCommand(_profesorRepository, _writer, _estudiante) },
                { 4, new ImprimirComprobanteCommand(_inscripcionService, _writer, _estudiante) }
            };
        }

        public void DisplayMenu()
        {
            if (_estudiante == null || _estudiante.Rol != Rol.Estudiante)
            {
                _writer.ShowError("Acceso denegado: El usuario no tiene permisos de Estudiante.");
                _writer.WaitForKey();
                return;
            }

            bool salir = false;

            do
            {
                _writer.Clear();
                _writer.DisplayInitialMessage("PORTAL DEL ESTUDIANTE");
                _writer.ShowMessage($"Estudiante: {_estudiante.Nombre} | Carnet: {_estudiante.Cedula}");
                _writer.ShowMessage($"Carrera:    {_estudiante.Carrera} | Estado: {(_estudiante.Activo ? "Activo" : "Inactivo")}\n");

                _writer.DisplayMenu(
                    "Seleccione una opcion:",
                    "Ver mi horario y materias inscritas",
                    "Inscribir nueva materia",
                    "Consultar mis calificaciones",
                    "Imprimir comprobante de matricula",
                    "Cerrar sesion"
                );

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 5)
                    .ReadAndValidate();

                int opcion = int.Parse(input);

                if (opcion == 5)
                {
                    salir = true;
                }
                else if (_commands.ContainsKey(opcion))
                {
                    _commands[opcion].Execute();
                }

            } while (!salir);
        }
    }
}
