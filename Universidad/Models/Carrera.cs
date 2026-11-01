namespace Universidad.Models
{
    public class Carrera
    {
        public string IdCarrera { get; private set; }
        public string Nombre { get; private set; }

        public Carrera(string idCarrera, string nombre)
        {
            IdCarrera = idCarrera;
            Nombre = nombre;
        }
    }
}

