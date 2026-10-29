using System;

namespace CarDealership.Util
{
    public class VehiculoNoEncontradoException : Exception
    {
        public string? Placa { get; }

        public VehiculoNoEncontradoException() 
            : base("Vehiculo no encontrado.")
        {
        }

        public VehiculoNoEncontradoException(string message) 
            : base(message)
        {
        }

        public VehiculoNoEncontradoException(string placa, string message) 
            : base(message)
        {
            Placa = placa;
        }

        public VehiculoNoEncontradoException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
