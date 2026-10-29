using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealership.Util
{
    public class Validator
    {
        private readonly string _promptMessage;
        private readonly List<Func<string, (bool IsValid, string ErrorMessage)>> _rules = new List<Func<string, (bool IsValid, string ErrorMessage)>>();

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
