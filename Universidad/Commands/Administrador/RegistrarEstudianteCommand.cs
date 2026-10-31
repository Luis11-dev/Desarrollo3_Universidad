using System;
using System.Globalization;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Administrador
{
    public class RegistrarEstudianteCommand : ICommand
    {
        private readonly EstudianteService _estudianteService;
        private readonly IConsoleWriter _writer;

        public RegistrarEstudianteCommand(EstudianteService estudianteService, IConsoleWriter writer)
        {
            _estudianteService = estudianteService;
            _writer = writer;
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

                string carrera = new Validator("Ingrese carrera o programa de estudio: ")
                    .NotEmpty("La carrera no puede estar vacia.")
                    .SoloLetras("La carrera o programa de estudio debe contener unicamente letras y espacios.")
                    .ReadAndValidate();

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
                    .PasswordSegura("La contrasenia debe tener al menos 8 caracteres e incluir mayuscula, minuscula, numero y caracter especial.")
                    .ReadAndValidate();

                Estudiante nuevoEstudiante = _estudianteService.RegistrarEstudiante(
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
