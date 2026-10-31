using Universidad.Controllers;
using Universidad.Data;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Services;

namespace Universidad.Util
{
    public static class Startup
    {
        private static ProfesorRepository? _profesorRepository;
        private static EstudianteRepository? _estudianteRepository;
        private static IConsoleWriter? _consoleWriter;

        public static IConsoleWriter InitConsoleWriter()
        {
            if (_consoleWriter == null)
            {
                _consoleWriter = new ConsoleWriter();
            }
            return _consoleWriter;
        }

        public static ProfesorRepository GetProfesorRepository()
        {
            if (_profesorRepository == null)
            {
                _profesorRepository = new ProfesorRepository();
                if (_estudianteRepository != null)
                {
                    Seeds.Inicializar(_profesorRepository, _estudianteRepository);
                }
            }
            return _profesorRepository;
        }

        public static EstudianteRepository GetEstudianteRepository()
        {
            if (_estudianteRepository == null)
            {
                ProfesorRepository profRepo = GetProfesorRepository();
                _estudianteRepository = new EstudianteRepository(profRepo);
                Seeds.Inicializar(profRepo, _estudianteRepository);
            }
            return _estudianteRepository;
        }

        public static AuthService InitAuthService()
        {
            ProfesorRepository profRepo = GetProfesorRepository();
            EstudianteRepository estRepo = GetEstudianteRepository();
            return new AuthService(profRepo, estRepo);
        }

        public static AuthController InitAuthController(int maxIntentos = 3)
        {
            AuthService authService = InitAuthService();
            IConsoleWriter writer = InitConsoleWriter();
            return new AuthController(authService, writer);
        }

        public static ProfesorService InitProfesorService(ProfesorRepository? profesorRepository = null)
        {
            ProfesorRepository repo = profesorRepository ?? GetProfesorRepository();
            return new ProfesorService(repo);
        }

        public static ProfesorController InitProfesorController(
            Profesor? profesorActual = null,
            ProfesorRepository? profesorRepository = null,
            IConsoleWriter? consoleWriter = null)
        {
            IConsoleWriter writer = consoleWriter ?? InitConsoleWriter();
            ProfesorRepository repository = profesorRepository ?? GetProfesorRepository();
            ProfesorService service = InitProfesorService(repository);
            Profesor profesor = profesorActual ?? new Profesor();

            return new ProfesorController(service, writer, profesor);
        }

        public static EstudianteController InitEstudianteController(
            Estudiante? estudianteActual = null,
            IConsoleWriter? consoleWriter = null)
        {
            IConsoleWriter writer = consoleWriter ?? InitConsoleWriter();
            Estudiante estudiante = estudianteActual ?? new Estudiante();

            return new EstudianteController(estudiante, writer);
        }
    }
}
