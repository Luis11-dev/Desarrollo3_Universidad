namespace Universidad.Models
{
    public class AnioAcademico
    {
        public int IdAnioAcademico { get; private set; }
        public string Nombre { get; private set; }

        public AnioAcademico(int idAnioAcademico, string nombre)
        {
            IdAnioAcademico = idAnioAcademico;
            Nombre = nombre;
        }
    }
}

