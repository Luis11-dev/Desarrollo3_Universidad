namespace Universidad.Models
{
    public class Profesor : PerfilUniversitario, IAutentication
    {
        public string EstadoCivil { get; private set; } = string.Empty;
        public string Profesion { get; private set; } = string.Empty;
        public string? Especialidad { get; private set; }

        public string User { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public Rol Rol { get; private set; } = Rol.Profesor;

        public Profesor() : base() { }

        public Profesor(
            string cedula,
            string nombre,
            string correoElectronico,
            int numeroCelular,
            string direccion,
            Genero genero,
            string estadoCivil,
            string profesion,
            string? especialidad,
            string user,
            string password)
            : base(cedula, nombre, correoElectronico, numeroCelular, direccion, genero)
        {
            EstadoCivil = estadoCivil;
            Profesion = profesion;
            Especialidad = especialidad;
            User = user;
            Password = password;
            Rol = Rol.Profesor;
        }
    }
}
