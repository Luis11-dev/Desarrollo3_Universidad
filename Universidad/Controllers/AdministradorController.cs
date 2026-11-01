using System;
using System.Collections.Generic;
using Universidad.Commands.Administrador;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Controllers
{
    public class AdministradorController : IController
    {
        private readonly ProfesorService _profesorService;
        private readonly EstudianteService _estudianteService;
        private readonly ProfesorRepository _profesorRepository;
        private readonly HorarioClaseRepository _horarioRepository;
        private readonly PlanAcademicoRepository _planRepository;
        private readonly MateriaRepository _materiaRepository;
        private readonly PlanillaService _planillaService;
        private readonly IConsoleWriter _writer;
        private readonly Administrador _adminActual;
        private readonly Dictionary<int, ICommand> _commands;

        public AdministradorController(
            ProfesorService profesorService,
            EstudianteService estudianteService,
            ProfesorRepository profesorRepository,
            HorarioClaseRepository horarioRepository,
            PlanAcademicoRepository planRepository,
            MateriaRepository materiaRepository,
            PlanillaService planillaService,
            IConsoleWriter writer,
            Administrador adminActual)
        {
            _profesorService = profesorService ?? throw new ArgumentNullException(nameof(profesorService));
            _estudianteService = estudianteService ?? throw new ArgumentNullException(nameof(estudianteService));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _materiaRepository = materiaRepository ?? throw new ArgumentNullException(nameof(materiaRepository));
            _planillaService = planillaService ?? throw new ArgumentNullException(nameof(planillaService));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _adminActual = adminActual ?? throw new ArgumentNullException(nameof(adminActual));

            _commands = new Dictionary<int, ICommand>
            {
                { 1, new RegistrarProfesorCommand(_profesorService, _writer) },
                { 2, new RegistrarEstudianteCommand(_estudianteService, _profesorRepository, _writer) },
                { 3, new AsignarHorarioClaseCommand(_profesorRepository, _horarioRepository, _planRepository, _materiaRepository, _writer) },
                { 4, new PrepararPlanillaCommand(_planillaService, _writer) }
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
                    "Asignar Materia y Horario a Profesor",
                    "Preparar y Consultar Planilla de Profesores",
                    "Cerrar sesion (Volver al menu principal)"
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
