using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Services
{
    public class AuthService
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly EstudianteRepository _estudianteRepository;
        private readonly AdministradorRepository _administradorRepository;

        public AuthService(
            ProfesorRepository profesorRepository,
            EstudianteRepository estudianteRepository,
            AdministradorRepository administradorRepository)
        {
            _profesorRepository = profesorRepository;
            _estudianteRepository = estudianteRepository;
            _administradorRepository = administradorRepository;
        }

        public IAutentication? Autenticar(string username, string password)
        {
            Administrador? admin = _administradorRepository.GetByUser(username);
            if (admin != null && admin.Password == password)
            {
                return admin;
            }

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
