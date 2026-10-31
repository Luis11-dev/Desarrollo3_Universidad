using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Services
{
    public class AuthService
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly EstudianteRepository _estudianteRepository;

        public AuthService(ProfesorRepository profesorRepository, EstudianteRepository estudianteRepository)
        {
            _profesorRepository = profesorRepository;
            _estudianteRepository = estudianteRepository;
        }

        public IAutentication? Autenticar(string username, string password)
        {
            Profesor? profesor = _profesorRepository.GetByUser(username);
            if (profesor != null && profesor.Password == password)
            {
                return profesor;
            }

            Estudiante? estudiante = _estudianteRepository.GetByUser(username);
            if (estudiante != null && estudiante.Password == password)
            {
                return estudiante;
            }

            return null;
        }
    }
}

