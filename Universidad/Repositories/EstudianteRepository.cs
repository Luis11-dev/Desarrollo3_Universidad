
using Universidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Repositories
{
    public class EstudianteRepository
    {
        private readonly List<Estudiante> _estudiantes = new List<Estudiante>();

        private readonly ProfesorRepository _profesorRepository;

        public EstudianteRepository(ProfesorRepository profesorRepository)
        {
            _profesorRepository = profesorRepository
                ?? throw new ArgumentNullException(nameof(profesorRepository));
        }

        // Registrar estudiante
        public void Add(Estudiante estudiante)
        {
            if (estudiante == null)
            {
                throw new ArgumentNullException(nameof(estudiante));
            }

            _estudiantes.Add(estudiante);
        }

        // Obtener todos los estudiantes
        public List<Estudiante> GetAll()
        {
            return _estudiantes.ToList();
        }

        // Buscar estudiante por cédula
        public Estudiante? GetByCedula(string cedula)
        {
            return _estudiantes.FirstOrDefault(e =>
                e.Cedula.Equals(cedula, StringComparison.OrdinalIgnoreCase));
        }

        // Buscar estudiante por nombre de usuario
        public Estudiante? GetByUser(string user)
        {
            return _estudiantes.FirstOrDefault(e =>
                e.User.Equals(user, StringComparison.OrdinalIgnoreCase));
        }

        // Comprobar si la cédula pertenece a un profesor
        public bool ExisteCedulaEnProfesor(string cedula)
        {
            return _profesorRepository.GetByCedula(cedula) != null;
        }

        // Comprobar si el usuario ya pertenece a un profesor
        public bool ExisteUsuarioEnProfesor(string user)
        {
            return _profesorRepository.GetByUser(user) != null;
        }

        // Obtener todos los grupos disponibles en el sistema
        public List<Grupo> GetAllGrupos()
        {
            return _profesorRepository.GetAllGrupos();
        }

        // Buscar un grupo por su código
        public Grupo? GetGrupoById(string idGrupo)
        {
            return _profesorRepository.GetGrupoById(idGrupo);
        }

        // Obtener las calificaciones de un estudiante
        public List<Calificacion> GetCalificacionesByEstudiante(string cedulaEstudiante)
        {
            List<Calificacion> resultado = new List<Calificacion>();

            foreach (Grupo grupo in _profesorRepository.GetAllGrupos())
            {
                List<Calificacion> calificaciones =
                    _profesorRepository.GetCalificacionesByGrupo(grupo.IdGrupo);

                resultado.AddRange(
                    calificaciones.Where(c =>
                        c.Estudiante != null &&
                        c.Estudiante.Cedula.Equals(
                            cedulaEstudiante,
                            StringComparison.OrdinalIgnoreCase))
                );
            }

            return resultado;
        }

        // Registrar la inscripción del estudiante en un grupo
        public void InscribirEnGrupo(Estudiante estudiante, Grupo grupo)
        {
            if (estudiante == null)
            {
                throw new ArgumentNullException(nameof(estudiante));
            }

            if (grupo == null)
            {
                throw new ArgumentNullException(nameof(grupo));
            }

            bool yaInscrito = grupo.Estudiantes.Any(e =>
                e.Cedula.Equals(
                    estudiante.Cedula,
                    StringComparison.OrdinalIgnoreCase));

            if (yaInscrito)
            {
                throw new InvalidOperationException(
                    "El estudiante ya pertenece a este grupo.");
            }

            grupo.AgregarEstudiante(estudiante);

            Calificacion calificacion = new Calificacion(estudiante, grupo);

            _profesorRepository.AddCalificacion(calificacion);
        }
    }
}
