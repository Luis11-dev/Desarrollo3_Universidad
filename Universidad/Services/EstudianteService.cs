
using Universidad.Models;
using Universidad.Repositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Universidad.Services
{
    public class EstudianteService
    {
        private readonly EstudianteRepository _estudianteRepository;

        public EstudianteService(EstudianteRepository estudianteRepository)
        {
            _estudianteRepository = estudianteRepository
                ?? throw new ArgumentNullException(nameof(estudianteRepository));
        }

        // Validar que los datos obligatorios estén completos
        private void ValidarDatosObligatorios(
            string cedula,
            string nombre,
            string correo,
            int numeroCelular,
            string direccion,
            string carrera,
            string user,
            string password)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                throw new ArgumentException("La cédula es obligatoria.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre completo es obligatorio.");

            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo electrónico es obligatorio.");

            if (numeroCelular <= 0)
                throw new ArgumentException("El número celular no es válido.");

            if (string.IsNullOrWhiteSpace(direccion))
                throw new ArgumentException("La dirección es obligatoria.");

            if (string.IsNullOrWhiteSpace(carrera))
                throw new ArgumentException("La carrera es obligatoria.");

            if (string.IsNullOrWhiteSpace(user))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es obligatoria.");
        }

        // Validar formato de cédula nicaragüense
        private void ValidarCedula(string cedula)
        {
            string patron = @"^\d{3}-\d{6}-\d{4}[A-Za-z]$";

            if (!Regex.IsMatch(cedula.Trim(), patron))
            {
                throw new ArgumentException(
                    "La cédula debe tener el formato 000-000000-0000A.");
            }
        }

        // Validar correo electrónico
        private void ValidarCorreo(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(correo.Trim(), patron))
            {
                throw new ArgumentException(
                    "El correo electrónico no tiene un formato válido.");
            }
        }

        // Validar nombre de usuario
        private void ValidarUsuario(string user)
        {
            if (user.Contains(' ') || user.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException(
                    "El nombre de usuario no debe contener espacios.");
            }
        }

        // Validar contraseña según los requisitos del proyecto
        private void ValidarPassword(string password)
        {
            if (password.Length < 8)
                throw new ArgumentException(
                    "La contraseña debe contener al menos 8 caracteres.");

            if (!password.Any(char.IsUpper))
                throw new ArgumentException(
                    "La contraseña debe contener al menos una letra mayúscula.");

            if (!password.Any(char.IsLower))
                throw new ArgumentException(
                    "La contraseña debe contener al menos una letra minúscula.");

            if (!password.Any(char.IsDigit))
                throw new ArgumentException(
                    "La contraseña debe contener al menos un número.");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                throw new ArgumentException(
                    "La contraseña debe contener al menos un carácter especial.");
        }

        // Registrar un estudiante
        public Estudiante RegistrarEstudiante(
            string cedula,
            string nombre,
            DateTime fechaNacimiento,
            string correo,
            int numeroCelular,
            string direccion,
            Genero genero,
            string carrera,
            bool activo,
            string user,
            string password)
        {
            ValidarDatosObligatorios(
                cedula,
                nombre,
                correo,
                numeroCelular,
                direccion,
                carrera,
                user,
                password);

            cedula = cedula.Trim().ToUpperInvariant();
            nombre = nombre.Trim();
            correo = correo.Trim();
            direccion = direccion.Trim();
            carrera = carrera.Trim();
            user = user.Trim();

            ValidarCedula(cedula);
            ValidarCorreo(correo);
            ValidarUsuario(user);
            ValidarPassword(password);

            if (fechaNacimiento == DateTime.MinValue ||
                fechaNacimiento.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "La fecha de nacimiento no es válida.");
            }

            if (_estudianteRepository.GetByCedula(cedula) != null)
            {
                throw new InvalidOperationException(
                    "Ya existe un estudiante registrado con esa cédula.");
            }

            if (_estudianteRepository.ExisteCedulaEnProfesor(cedula))
            {
                throw new InvalidOperationException(
                    "Esta cédula ya pertenece a un profesor.");
            }

            if (_estudianteRepository.GetByUser(user) != null ||
                _estudianteRepository.ExisteUsuarioEnProfesor(user))
            {
                throw new InvalidOperationException(
                    "El nombre de usuario ya está registrado.");
            }

            bool correoExiste = _estudianteRepository
                .GetAll()
                .Any(e => e.CorreoElectronico.Equals(
                    correo,
                    StringComparison.OrdinalIgnoreCase));

            if (correoExiste)
            {
                throw new InvalidOperationException(
                    "El correo electrónico ya está registrado.");
            }

            Estudiante estudiante = new Estudiante(
                cedula,
                nombre,
                fechaNacimiento,
                correo,
                numeroCelular,
                direccion,
                genero,
                carrera,
                activo,
                user,
                password);

            _estudianteRepository.Add(estudiante);

            return estudiante;
        }

        // Consultar todos los estudiantes
        public List<Estudiante> ObtenerEstudiantes()
        {
            return _estudianteRepository.GetAll();
        }

        // Consultar un estudiante por cédula
        public Estudiante ObtenerPorCedula(string cedula)
        {
            Estudiante? estudiante =
                _estudianteRepository.GetByCedula(cedula);

            if (estudiante == null)
            {
                throw new KeyNotFoundException(
                    "No se encontró un estudiante con esa cédula.");
            }

            return estudiante;
        }

        // Activar un estudiante
        public void ActivarEstudiante(string cedula)
        {
            Estudiante estudiante = ObtenerPorCedula(cedula);
            estudiante.Activar();
        }

        // Desactivar un estudiante
        public void DesactivarEstudiante(string cedula)
        {
            Estudiante estudiante = ObtenerPorCedula(cedula);
            estudiante.Desactivar();
        }
    }
}
