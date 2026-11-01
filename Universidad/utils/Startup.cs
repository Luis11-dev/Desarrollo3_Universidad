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
        private static ContratoRepository? _contratoRepository;
        private static InscripcionRepository? _inscripcionRepository;
        private static IConsoleWriter? _consoleWriter;
        private static bool _seedsLoaded = false;
        private static bool _archivosCargados = false;

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
                _horarioClaseRepository != null &&
                _contratoRepository != null &&
                _inscripcionRepository != null)
            {
                _seedsLoaded = true;
                Seeds.Inicializar(
                    _profesorRepository,
                    _estudianteRepository,
                    _administradorRepository,
                    _materiaRepository,
                    _planAcademicoRepository,
                    _horarioClaseRepository,
                    _contratoRepository,
                    _inscripcionRepository
                );
            }
        }

        public static ProfesorRepository GetProfesorRepository()
        {
            EnsureAllRepositories();
            return _profesorRepository!;
        }

        public static EstudianteRepository GetEstudianteRepository()
        {
            EnsureAllRepositories();
            return _estudianteRepository!;
        }

        public static AdministradorRepository GetAdministradorRepository()
        {
            EnsureAllRepositories();
            return _administradorRepository!;
        }

        public static MateriaRepository GetMateriaRepository()
        {
            EnsureAllRepositories();
            return _materiaRepository!;
        }

        public static PlanAcademicoRepository GetPlanAcademicoRepository()
        {
            EnsureAllRepositories();
            return _planAcademicoRepository!;
        }

        public static HorarioClaseRepository GetHorarioClaseRepository()
        {
            EnsureAllRepositories();
            return _horarioClaseRepository!;
        }

        public static ContratoRepository GetContratoRepository()
        {
            EnsureAllRepositories();
            return _contratoRepository!;
        }

        public static InscripcionRepository GetInscripcionRepository()
        {
            EnsureAllRepositories();
            return _inscripcionRepository!;
        }

        private static void EnsureAllRepositories()
        {
            if (_profesorRepository == null) _profesorRepository = new ProfesorRepository();
            if (_administradorRepository == null) _administradorRepository = new AdministradorRepository();
            if (_materiaRepository == null) _materiaRepository = new MateriaRepository();
            if (_planAcademicoRepository == null) _planAcademicoRepository = new PlanAcademicoRepository();
            if (_horarioClaseRepository == null) _horarioClaseRepository = new HorarioClaseRepository();
            if (_contratoRepository == null) _contratoRepository = new ContratoRepository();
            if (_inscripcionRepository == null) _inscripcionRepository = new InscripcionRepository(_materiaRepository, _profesorRepository);
            if (_estudianteRepository == null) _estudianteRepository = new EstudianteRepository(_profesorRepository);

            EnsureSeeds();
            InicializarDatos();
        }

        public static void InicializarDatos()
        {
            if (_archivosCargados)
            {
                return;
            }

            if (_profesorRepository == null ||
                _estudianteRepository == null ||
                _materiaRepository == null ||
                _horarioClaseRepository == null)
            {
                return;
            }

            _archivosCargados = true;

            // 1. Cargar profesores
            List<ProfesorDto> profesoresDto = FileManager.CargarProfesoresDto();
            foreach (ProfesorDto pDto in profesoresDto)
            {
                if (_profesorRepository.GetByCedula(pDto.Cedula) == null)
                {
                    Profesor profesor = new Profesor(
                        pDto.Cedula,
                        pDto.Nombre,
                        pDto.CorreoElectronico,
                        pDto.NumeroCelular,
                        pDto.Direccion,
                        pDto.Genero,
                        pDto.EstadoCivil,
                        pDto.Profesion,
                        pDto.Especialidad,
                        pDto.User,
                        pDto.Password
                    );
                    _profesorRepository.Add(profesor);
                }
            }

            // 2. Cargar estudiantes
            List<EstudianteDto> estudiantesDto = FileManager.CargarEstudiantesDto();
            foreach (EstudianteDto eDto in estudiantesDto)
            {
                Estudiante? existente = _estudianteRepository.GetByCedula(eDto.Cedula);
                Estudiante estudiante;
                if (existente == null)
                {
                    estudiante = new Estudiante(
                        eDto.Cedula,
                        eDto.Nombre,
                        eDto.FechaNacimiento,
                        eDto.CorreoElectronico,
                        eDto.NumeroCelular,
                        eDto.Direccion,
                        eDto.Genero,
                        eDto.Carrera,
                        eDto.Activo,
                        eDto.User,
                        eDto.Password
                    );
                    _estudianteRepository.Add(estudiante);
                }
                else
                {
                    estudiante = existente;
                }

                if (!string.IsNullOrWhiteSpace(eDto.IdGrupo))
                {
                    Grupo? grupo = _profesorRepository.GetGrupoById(eDto.IdGrupo);
                    if (grupo != null && !grupo.Estudiantes.Any(est => est.Cedula.Equals(estudiante.Cedula, StringComparison.OrdinalIgnoreCase)))
                    {
                        grupo.AgregarEstudiante(estudiante);
                    }
                }
            }

            // 3. Cargar horarios
            List<HorarioClaseDto> horariosDto = FileManager.CargarHorariosDto();
            foreach (HorarioClaseDto hDto in horariosDto)
            {
                if (_horarioClaseRepository.GetAll().Any(h => h.IdHorario.Equals(hDto.IdHorario, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                Grupo? grupo = _profesorRepository.GetGrupoById(hDto.IdGrupo);
                Materia? materia = _materiaRepository.ObtenerPorCodigo(hDto.CodigoMateria);
                Profesor? profesor = _profesorRepository.GetByCedula(hDto.CedulaProfesor);

                if (grupo != null && materia != null && profesor != null)
                {
                    HorarioClase horario = new HorarioClase(
                        hDto.IdHorario,
                        grupo,
                        materia,
                        profesor,
                        hDto.Dia,
                        hDto.HoraInicio,
                        hDto.HoraFin,
                        hDto.Modalidad
                    );

                    foreach (string cedula in hDto.CedulasEstudiantesInscritos)
                    {
                        Estudiante? est = _estudianteRepository.GetByCedula(cedula);
                        if (est != null)
                        {
                            horario.InscribirEstudiante(est);
                        }
                    }

                    _horarioClaseRepository.Add(horario);

                    Contrato? contrato = _contratoRepository?.GetContratoActivo(profesor.Cedula);
                    if (contrato != null)
                    {
                        contrato.AsignarMateria(horario);
                    }
                }
            }

            // 4. Cargar inscripciones
            if (_inscripcionRepository != null)
            {
                List<InscripcionDto> inscripcionesDto = FileManager.CargarInscripcionesDto();
                foreach (InscripcionDto iDto in inscripcionesDto)
                {
                    if (_inscripcionRepository.ObtenerTodas().Any(i => i.IdInscripcion.Equals(iDto.IdInscripcion, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    Estudiante? estudiante = _estudianteRepository.GetByCedula(iDto.CedulaEstudiante);
                    HorarioClase? horario = _horarioClaseRepository.GetAll().FirstOrDefault(h => h.IdHorario.Equals(iDto.IdHorario, StringComparison.OrdinalIgnoreCase));

                    if (estudiante != null && horario != null)
                    {
                        Inscripcion inscripcion = new Inscripcion(
                            iDto.IdInscripcion,
                            estudiante,
                            horario,
                            iDto.PeriodoAcademico
                        );

                        _inscripcionRepository.Agregar(inscripcion);

                        if (!horario.EstudiantesInscritos.Any(e => e.Cedula.Equals(estudiante.Cedula, StringComparison.OrdinalIgnoreCase)))
                        {
                            horario.InscribirEstudiante(estudiante);
                        }
                    }
                }
            }
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

        public static ProfesorService InitProfesorService(
            ProfesorRepository? profesorRepository = null,
            HorarioClaseRepository? horarioClaseRepository = null,
            ContratoRepository? contratoRepository = null,
            PlanillaService? planillaService = null)
        {
            ProfesorRepository repo = profesorRepository ?? GetProfesorRepository();
            HorarioClaseRepository horarioRepo = horarioClaseRepository ?? GetHorarioClaseRepository();
            ContratoRepository contratoRepo = contratoRepository ?? GetContratoRepository();
            PlanillaService planilla = planillaService ?? InitPlanillaService();
            return new ProfesorService(repo, horarioRepo, contratoRepo, planilla);
        }

        public static EstudianteService InitEstudianteService(EstudianteRepository? estudianteRepository = null)
        {
            EstudianteRepository repo = estudianteRepository ?? GetEstudianteRepository();
            return new EstudianteService(repo);
        }

        public static InscripcionService InitInscripcionService()
        {
            HorarioClaseRepository horarioRepo = GetHorarioClaseRepository();
            MateriaRepository materiaRepo = GetMateriaRepository();
            InscripcionRepository inscripcionRepo = GetInscripcionRepository();
            ProfesorRepository profesorRepo = GetProfesorRepository();
            return new InscripcionService(horarioRepo, materiaRepo, inscripcionRepo, profesorRepo);
        }

        public static PlanillaService InitPlanillaService()
        {
            HorarioClaseRepository horarioRepo = GetHorarioClaseRepository();
            ProfesorRepository profesorRepo = GetProfesorRepository();
            ContratoRepository contratoRepo = GetContratoRepository();
            return new PlanillaService(horarioRepo, profesorRepo, contratoRepo);
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
            InscripcionService inscripcionService = InitInscripcionService();
            ProfesorRepository profesorRepo = GetProfesorRepository();

            return new EstudianteController(estudiante, inscripcionService, profesorRepo, writer);
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
                admin,
                GetContratoRepository()
            );
        }
    }
}
