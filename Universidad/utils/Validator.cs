using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Universidad.Util
{
    public class Validator
    {
        private readonly string _promptMessage;
        protected readonly List<Func<string, (bool IsValid, string ErrorMessage)>> _rules = new List<Func<string, (bool IsValid, string ErrorMessage)>>();

        public Validator(string promptMessage)
        {
            _promptMessage = promptMessage;
        }

        public Validator NotEmpty(string errorMessage = "Error: No se ingreso ningun valor")
        {
            _rules.Add(input => (!string.IsNullOrWhiteSpace(input), errorMessage));
            return this;
        }

        public Validator OnlyDigits(string errorMessage = "Error: Solo se permiten digitos numericos")
        {
            _rules.Add(input => (input.All(char.IsDigit), errorMessage));
            return this;
        }

        public Validator InputBetweenXandY(int x, int y, string errorMessage = "Error: Solo se permiten valores dentro del rango indicado")
        {
            _rules.Add(input => (CheckRange(input, x, y), errorMessage));
            return this;
        }

        private bool CheckRange(string input, int x, int y)
        {
            if (!int.TryParse(input, out int selectedOption))
            {
                return false;
            }
            return selectedOption >= x && selectedOption <= y;
        }

        public Validator FormatoCedula(string errorMessage = "Error: El formato de la cedula debe ser 000-000000-0000X")
        {
            string patron = @"^\d{3}-\d{6}-\d{4}[A-Za-z]$";
            _rules.Add(input => (Regex.IsMatch(input.Trim(), patron), errorMessage));
            return this;
        }

        public Validator FormatoCorreo(string errorMessage = "Error: El correo electronico no tiene un formato valido")
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            _rules.Add(input => (Regex.IsMatch(input.Trim(), patron), errorMessage));
            return this;
        }

        public Validator NumeroCelular(int longitud = 8, string errorMessage = "Error: El numero celular debe contener exactamente 8 digitos numericos")
        {
            _rules.Add(input =>
            {
                string trimmed = input.Trim();
                bool esValido = trimmed.Length == longitud && trimmed.All(char.IsDigit) && int.TryParse(trimmed, out int num) && num > 0;
                return (esValido, errorMessage);
            });
            return this;
        }

        public Validator FormatoFecha(string formato = "dd/MM/yyyy", string errorMessage = "Error: Formato de fecha invalido. Use dd/MM/yyyy")
        {
            _rules.Add(input =>
            {
                bool esValida = DateTime.TryParseExact(input.Trim(), formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
                return (esValida, errorMessage);
            });
            return this;
        }

        public Validator FechaNoFutura(string formato = "dd/MM/yyyy", string errorMessage = "Error: La fecha no puede ser futura")
        {
            _rules.Add(input =>
            {
                if (DateTime.TryParseExact(input.Trim(), formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                {
                    return (fecha.Date <= DateTime.Today, errorMessage);
                }
                return (false, "Error: Formato de fecha invalido para verificar.");
            });
            return this;
        }

        public Validator SinEspacios(string errorMessage = "Error: El valor no debe contener espacios en blanco")
        {
            _rules.Add(input => (!input.Any(char.IsWhiteSpace), errorMessage));
            return this;
        }

        public Validator MinLength(int minLength, string errorMessage = "")
        {
            string msg = string.IsNullOrWhiteSpace(errorMessage)
                ? $"Error: Debe contener al menos {minLength} caracteres"
                : errorMessage;

            _rules.Add(input => (input.Trim().Length >= minLength, msg));
            return this;
        }

        public Validator SoloLetras(string errorMessage = "Error: Solo se permiten letras y espacios")
        {
            _rules.Add(input =>
            {
                string trimmed = input.Trim();
                bool esValido = !string.IsNullOrEmpty(trimmed) && trimmed.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
                return (esValido, errorMessage);
            });
            return this;
        }

        public Validator PasswordSegura(string errorMessage = "Error: La contrasenia debe tener al menos 8 caracteres e incluir mayuscula, minuscula, numero y caracter especial")
        {
            _rules.Add(input =>
            {
                bool esValida = input.Length >= 8 &&
                                input.Any(char.IsUpper) &&
                                input.Any(char.IsLower) &&
                                input.Any(char.IsDigit) &&
                                input.Any(c => !char.IsLetterOrDigit(c));
                return (esValida, errorMessage);
            });
            return this;
        }

        public string ReadAndValidate()
        {
            while (true)
            {
                Console.Write(_promptMessage);
                string input = Console.ReadLine() ?? string.Empty;
                bool allValid = true;

                foreach (var rule in _rules)
                {
                    var result = rule(input);
                    if (!result.IsValid)
                    {
                        Console.WriteLine(result.ErrorMessage);
                        allValid = false;
                        break;
                    }
                }

                if (allValid) return input;
            }
        }
    }
}
