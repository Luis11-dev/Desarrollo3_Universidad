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
        private static AdministradorRepository? _administradorRepository;
        private static MateriaRepository? _materiaRepository;
        private static PlanAcademicoRepository? _planAcademicoRepository;
        private static HorarioClaseRepository? _horarioClaseRepository;
        private static IConsoleWriter? _consoleWriter;
        private static bool _seedsLoaded = false;

        public static IConsoleWriter InitConsoleWriter()
        {
            if (_consoleWriter == null)
            {
                _consoleWriter = new ConsoleWriter();
            }
            return _consoleWriter;
        }

        private static void EnsureSeeds()
        {
            if (!_seedsLoaded &&
                _profesorRepository != null &&
                _estudianteRepository != null &&
                _administradorRepository != null &&
                _materiaRepository != null &&
                _planAcademicoRepository != null &&
                _horarioClaseRepository != null)
            {
                _seedsLoaded = true;
                Seeds.Inicializar(
                    _profesorRepository,
                    _estudianteRepository,
                    _administradorRepository,
                    _materiaRepository,
                    _planAcademicoRepository,
                    _horarioClaseRepository
                );
            }
        }

        public static ProfesorRepository GetProfesorRepository()
        {
            if (_profesorRepository == null)
            {
                _profesorRepository = new ProfesorRepository();
            }
            EnsureAllRepositories();
            return _profesorRepository;
        }

        public static EstudianteRepository GetEstudianteRepository()
        {
            if (_estudianteRepository == null)
            {
                _estudianteRepository = new EstudianteRepository(GetProfesorRepository());
            }
            EnsureAllRepositories();
            return _estudianteRepository;
        }

        public static AdministradorRepository GetAdministradorRepository()
        {
            if (_administradorRepository == null)
            {
                _administradorRepository = new AdministradorRepository();
            }
            EnsureAllRepositories();
            return _administradorRepository;
        }

        public static MateriaRepository GetMateriaRepository()
        {
            if (_materiaRepository == null)
            {
                _materiaRepository = new MateriaRepository();
            }
            EnsureAllRepositories();
            return _materiaRepository;
        }

        public static PlanAcademicoRepository GetPlanAcademicoRepository()
        {
            if (_planAcademicoRepository == null)
            {
                _planAcademicoRepository = new PlanAcademicoRepository();
            }
            EnsureAllRepositories();
            return _planAcademicoRepository;
        }

        public static HorarioClaseRepository GetHorarioClaseRepository()
        {
            if (_horarioClaseRepository == null)
            {
                _horarioClaseRepository = new HorarioClaseRepository();
            }
            EnsureAllRepositories();
            return _horarioClaseRepository;
        }

        private static void EnsureAllRepositories()
        {
            if (_profesorRepository == null) _profesorRepository = new ProfesorRepository();
            if (_administradorRepository == null) _administradorRepository = new AdministradorRepository();
            if (_materiaRepository == null) _materiaRepository = new MateriaRepository();
            if (_planAcademicoRepository == null) _planAcademicoRepository = new PlanAcademicoRepository();
            if (_horarioClaseRepository == null) _horarioClaseRepository = new HorarioClaseRepository();
            if (_estudianteRepository == null) _estudianteRepository = new EstudianteRepository(_profesorRepository);

            EnsureSeeds();
        }

        public static AuthService InitAuthService()
        {
            EnsureAllRepositories();
            return new AuthService(_profesorRepository!, _estudianteRepository!, _administradorRepository!);
        }

        public static AuthController InitAuthController(int maxIntentos = 3)
        {
            AuthService authService = InitAuthService();
            IConsoleWriter writer = InitConsoleWriter();
            return new AuthController(authService, writer, maxIntentos);
        }

        public static ProfesorService InitProfesorService(ProfesorRepository? profesorRepository = null)
        {
            ProfesorRepository repo = profesorRepository ?? GetProfesorRepository();
            return new ProfesorService(repo);
        }

        public static EstudianteService InitEstudianteService(EstudianteRepository? estudianteRepository = null)
        {
            EstudianteRepository repo = estudianteRepository ?? GetEstudianteRepository();
            return new EstudianteService(repo);
        }

        public static PlanillaService InitPlanillaService()
        {
            HorarioClaseRepository horarioRepo = GetHorarioClaseRepository();
            ProfesorRepository profesorRepo = GetProfesorRepository();
            return new PlanillaService(horarioRepo, profesorRepo);
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

        public static AdministradorController InitAdministradorController(
            Administrador? adminActual = null,
            IConsoleWriter? consoleWriter = null)
        {
            IConsoleWriter writer = consoleWriter ?? InitConsoleWriter();
            ProfesorRepository profRepo = GetProfesorRepository();
            ProfesorService profService = InitProfesorService(profRepo);
            EstudianteService estService = InitEstudianteService();
            HorarioClaseRepository horarioRepo = GetHorarioClaseRepository();
            PlanAcademicoRepository planRepo = GetPlanAcademicoRepository();
            MateriaRepository materiaRepo = GetMateriaRepository();
            PlanillaService planillaService = InitPlanillaService();
            Administrador admin = adminActual ?? new Administrador();

            return new AdministradorController(
                profService,
                estService,
                profRepo,
                horarioRepo,
                planRepo,
                materiaRepo,
                planillaService,
                writer,
                admin
            );
        }
    }
}
