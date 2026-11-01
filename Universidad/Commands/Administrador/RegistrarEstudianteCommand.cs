using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Universidad.Data;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Administrador
{
    public class RegistrarEstudianteCommand : ICommand
    {
        private readonly EstudianteService _estudianteService;
        private readonly ProfesorRepository _profesorRepository;
        private readonly IConsoleWriter _writer;

        public RegistrarEstudianteCommand(
            EstudianteService estudianteService,
            ProfesorRepository profesorRepository,
            IConsoleWriter writer)
        {
            _estudianteService = estudianteService ?? throw new ArgumentNullException(nameof(estudianteService));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("REGISTRO DE NUEVO ESTUDIANTE");

            try
            {
                string nombre = new Validator("Ingrese nombre completo: ")
                    .NotEmpty("El nombre no puede estar vacio.")
                    .SoloLetras("El nombre completo debe contener unicamente letras y espacios.")
                    .ReadAndValidate();

                string cedula = new Validator("Ingrese numero de cedula (000-000000-0000X): ")
                    .NotEmpty("La cedula no puede estar vacia.")
                    .FormatoCedula("El formato de la cedula debe ser 000-000000-0000X.")
                    .ReadAndValidate();

                string fechaInput = new Validator("Ingrese fecha de nacimiento (dd/MM/yyyy): ")
                    .NotEmpty("La fecha de nacimiento no puede estar vacia.")
                    .FormatoFecha("dd/MM/yyyy", "Formato de fecha invalido. Use dd/MM/yyyy (ej. 15/05/2002).")
                    .FechaNoFutura("dd/MM/yyyy", "La fecha de nacimiento no puede ser una fecha futura.")
                    .ReadAndValidate();
                DateTime fechaNacimiento = DateTime.ParseExact(fechaInput.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                _writer.DisplayMenu("Seleccione el genero:", "Masculino", "Femenino");
                string generoOpcion = new Validator("Ingrese opcion de genero (1-2): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();
                Genero genero = generoOpcion == "1" ? Genero.Masculino : Genero.Femenino;

                string celularInput = new Validator("Ingrese numero de celular (8 digitos): ")
                    .NotEmpty("El numero de celular no puede estar vacio.")
                    .NumeroCelular(8, "El numero celular debe contener exactamente 8 digitos numericos.")
                    .ReadAndValidate();
                int celular = int.Parse(celularInput);

                string correo = new Validator("Ingrese correo electronico: ")
                    .NotEmpty("El correo electronico no puede estar vacio.")
                    .FormatoCorreo("El formato del correo electronico no es valido (ej. usuario@dominio.com).")
                    .ReadAndValidate();

                string direccion = new Validator("Ingrese direccion: ")
                    .NotEmpty("La direccion no puede estar vacia.")
                    .ReadAndValidate();

                List<Carrera> carreras = Seeds.Carreras;
                string[] opcionesCarreras = carreras.Select(c => c.Nombre).ToArray();
                _writer.DisplayMenu("Seleccione la carrera del estudiante:", opcionesCarreras);

                string inputCarrera = new Validator("Ingrese opcion de carrera: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, carreras.Count)
                    .ReadAndValidate();
                string carrera = carreras[int.Parse(inputCarrera) - 1].Nombre;

                _writer.DisplayMenu("Seleccione el estado del estudiante:", "Activo", "Inactivo");
                string estadoOpcion = new Validator("Ingrese opcion de estado (1-2): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();
                bool activo = estadoOpcion == "1";

                string user = new Validator("Ingrese nombre de usuario para el login: ")
                    .NotEmpty("El usuario no puede estar vacio.")
                    .SinEspacios("El usuario no debe contener espacios en blanco.")
                    .MinLength(4, "El usuario debe tener al menos 4 caracteres.")
                    .ReadAndValidate();

                string password = new Validator("Ingrese contrasenia para el login: ")
                    .NotEmpty("La contrasenia no puede estar vacia.")
                    .SinEspacios("La contraseña no debe contener espacios en blanco.")
                    .PasswordSegura("La contrasenia debe tener al menos 8 caracteres e incluir mayuscula, minuscula, numero y caracter especial.")
                    .ReadAndValidate();

                Models.Estudiante nuevoEstudiante = _estudianteService.RegistrarEstudiante(
                    cedula,
                    nombre,
                    fechaNacimiento,
                    correo,
                    celular,
                    direccion,
                    genero,
                    carrera,
                    activo,
                    user,
                    password
                );

                List<Grupo> gruposDisponibles = _profesorRepository.GetAllGrupos();
                if (gruposDisponibles.Count > 0)
                {
                    string[] opcionesGrupos = gruposDisponibles
                        .Select(g => $"{g.NombreGrupo} ({g.Carrera} - Año {g.AnioAcademico}) [Estudiantes: {g.Estudiantes.Count}/{g.Cupos}]")
                        .ToArray();

                    _writer.DisplayMenu("Seleccione el Grupo cohorte al que pertenecera el estudiante:", opcionesGrupos);

                    string inputGrupo = new Validator("Ingrese opcion de grupo: ")
                        .NotEmpty()
                        .OnlyDigits()
                        .InputBetweenXandY(1, gruposDisponibles.Count)
                        .ReadAndValidate();

                    Grupo grupoSeleccionado = gruposDisponibles[int.Parse(inputGrupo) - 1];
                    grupoSeleccionado.AgregarEstudiante(nuevoEstudiante);

                    _writer.ShowMessage($"\nEstudiante asignado exitosamente al grupo: {grupoSeleccionado.NombreGrupo}");
                }

                FileManager.GuardarEstudiantes(_estudianteService.ObtenerEstudiantes(), _profesorRepository.GetAllGrupos());

                _writer.ShowMessage($"\nEstudiante registrado exitosamente.");
                _writer.ShowMessage($"Nombre: {nuevoEstudiante.Nombre} | Carnet/Cedula: {nuevoEstudiante.Cedula}");
                _writer.ShowMessage($"Carrera: {nuevoEstudiante.Carrera} | Estado: {(nuevoEstudiante.Activo ? "Activo" : "Inactivo")}");
                _writer.ShowMessage($"Usuario de acceso: {nuevoEstudiante.User}\n");
                _writer.WaitForKey();
            }
            catch (Exception ex)
            {
                _writer.ShowError($"\nError al registrar estudiante: {ex.Message}");
                _writer.WaitForKey();
            }
        }

        public void Undo()
        {
            throw new NotSupportedException("La operacion de deshacer no esta soportada para el registro de estudiantes.");
        }
    }
}
