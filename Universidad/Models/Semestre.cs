namespace Universidad.Models
{
    public class Semestre
    {
        public int IdSemestre { get; private set; }
        public string Nombre { get; private set; }

        public Semestre(int idSemestre, string nombre)
        {
            IdSemestre = idSemestre;
            Nombre = nombre;
        }
    }
}

