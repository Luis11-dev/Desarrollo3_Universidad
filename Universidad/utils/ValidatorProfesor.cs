using System;
using System.Globalization;

namespace Universidad.Util
{
    public class ValidatorProfesor : Validator
    {
        public ValidatorProfesor(string promptMessage) : base(promptMessage) { }

        public new ValidatorProfesor NotEmpty(string errorMessage = "Error: No se ingreso ningun valor")
        {
            base.NotEmpty(errorMessage);
            return this;
        }

        public new ValidatorProfesor OnlyDigits(string errorMessage = "Error: Solo se permiten digitos numericos")
        {
            base.OnlyDigits(errorMessage);
            return this;
        }

        public new ValidatorProfesor InputBetweenXandY(int x, int y, string errorMessage = "Error: Solo se permiten valores dentro del rango indicado")
        {
            base.InputBetweenXandY(x, y, errorMessage);
            return this;
        }

        // Nueva regla solo para tu módulo: Rango numérico con mensaje personalizado
        public ValidatorProfesor NotaEnRango(int min, int max)
        {
            _rules.Add(input =>
            {
                if (!int.TryParse(input, out int valor))
                    return (false, "Error: Debe ingresar un valor numerico entero.");
                if (valor < min || valor > max)
                    return (false, $"Error: La nota debe estar entre {min} y {max} puntos.");
                return (true, string.Empty);
            });
            return this;
        }

        // Nueva regla solo para asistencia: Formato HH:mm
        public ValidatorProfesor FormatoHora24h()
        {
            string[] formatos = { @"hh\:mm", @"h\:mm" };
            _rules.Add(input =>
            {
                bool esValida = TimeSpan.TryParseExact(input, formatos, CultureInfo.InvariantCulture, out _);
                return (esValida, "Error: Formato de hora invalido. Use HH:mm (ej. 07:30).");
            });
            return this;
        }
    }
}
