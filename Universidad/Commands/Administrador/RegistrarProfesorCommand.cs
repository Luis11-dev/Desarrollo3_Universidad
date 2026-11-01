using System;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Administrador
{
    public class RegistrarProfesorCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;

        public RegistrarProfesorCommand(ProfesorService profesorService, IConsoleWriter writer)
        {
            _profesorService = profesorService;
            _writer = writer;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("REGISTRO DE NUEVO PROFESOR");

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

                string celularInput = new Validator("Ingrese numero de celular (8 digitos): ")
                    .NotEmpty("El numero de celular no puede estar vacio.")
                    .NumeroCelular(8, "El numero celular debe contener exactamente 8 digitos numericos.")
                    .ReadAndValidate();
                int celular = int.Parse(celularInput);

                string correo = new Validator("Ingrese correo electronico: ")
                    .NotEmpty("El correo electronico no puede estar vacio.")
                    .FormatoCorreo("El formato del correo electronico no es valido (ej. usuario@dominio.com).")
                    .ReadAndValidate();

                string estadoCivil = new Validator("Ingrese estado civil: ")
                    .NotEmpty("El estado civil no puede estar vacio.")
                    .SoloLetras("El estado civil debe contener unicamente letras.")
                    .ReadAndValidate();

                string profesion = new Validator("Ingrese profesion: ")
                    .NotEmpty("La profesion no puede estar vacia.")
                    .SoloLetras("La profesion debe contener unicamente letras y espacios.")
                    .ReadAndValidate();

                Console.Write("Ingrese especialidad (opcional, presione Enter para omitir): ");
                string? especialidadInput = Console.ReadLine();
                string? especialidad = string.IsNullOrWhiteSpace(especialidadInput) ? null : especialidadInput.Trim();

                string direccion = new Validator("Ingrese direccion: ")
                    .NotEmpty("La direccion no puede estar vacia.")
                    .ReadAndValidate();

                _writer.DisplayMenu("Seleccione el genero:", "Masculino", "Femenino");
                string generoOpcion = new Validator("Ingrese opcion de genero (1-2): ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();
                Genero genero = generoOpcion == "1" ? Genero.Masculino : Genero.Femenino;

                string user = new Validator("Ingrese nombre de usuario para el login: ")
                    .NotEmpty("El usuario no puede estar vacio.")
                    .SinEspacios("El usuario no debe contener espacios en blanco.")
                    .MinLength(4, "El usuario debe tener al menos 4 caracteres.")
                    .ReadAndValidate();

                string password = new Validator("Ingrese contrasenia para el login: ")
                    .NotEmpty("La contrasenia no puede estar vacia.")
                    .SinEspacios("La contraseña no debe contener espacios en blanco.")
                    .PasswordSegura()
                    .ReadAndValidate();

                Models.Profesor nuevoProfesor = _profesorService.RegistrarProfesor(
                    cedula,
                    nombre,
                    correo,
                    celular,
                    direccion,
                    genero,
                    estadoCivil,
                    profesion,
                    especialidad,
                    user,
                    password
                );

                FileManager.GuardarProfesores(_profesorService.ObtenerProfesores());

                _writer.ShowMessage($"\nProfesor registrado exitosamente.");
                _writer.ShowMessage($"Nombre: {nuevoProfesor.Nombre} | Cedula: {nuevoProfesor.Cedula}");
                _writer.ShowMessage($"Profesion: {nuevoProfesor.Profesion} | Especialidad: {nuevoProfesor.Especialidad ?? "Ninguna"}");
                _writer.ShowMessage($"Usuario de acceso: {nuevoProfesor.User}\n");
                _writer.WaitForKey();
            }
            catch (Exception ex)
            {
                _writer.ShowError($"\nError al registrar profesor: {ex.Message}");
                _writer.WaitForKey();
            }
        }

        public void Undo()
        {
            throw new NotSupportedException("La operacion de deshacer no esta soportada para el registro de profesores.");
        }
    }
}
