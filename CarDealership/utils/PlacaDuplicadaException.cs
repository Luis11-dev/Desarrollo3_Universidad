using System;

namespace CarDealership.Util
{
    public class PlacaDuplicadaException : Exception
    {
        public string? Placa { get; }

        public PlacaDuplicadaException() 
            : base("La placa ya se encuentra registrada en el sistema.")
        {
        }

        public PlacaDuplicadaException(string message) 
            : base(message)
        {
        }

        public PlacaDuplicadaException(string placa, string message) 
            : base(message)
        {
            Placa = placa;
        }

        public PlacaDuplicadaException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
