using Universidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Universidad.Repositories
{
    public class ProfesorRepository
    {
        private readonly List<Profesor> _profesores = new List<Profesor>();
        private readonly List<Grupo> _grupos = new List<Grupo>();
        private readonly List<Calificacion> _calificaciones = new List<Calificacion>();
        private readonly List<AsistenciaProfesor> _asistencias = new List<AsistenciaProfesor>();

        public DateTime? FechaTopeNotas { get; set; }

        // --- Profesores ---
        public void Add(Profesor profesor)
        {
            _profesores.Add(profesor);
        }

        public List<Profesor> GetAll()
        {
            return _profesores.ToList();
        }

        public Profesor? GetByCedula(string cedula)
        {
            return _profesores.FirstOrDefault(p => p.Cedula == cedula);
        }

        public Profesor? GetByUser(string user)
        {
            return _profesores.FirstOrDefault(p => p.User == user);
        }

        // --- Grupos ---
        public void AddGrupo(Grupo grupo)
        {
            _grupos.Add(grupo);
        }

        public List<Grupo> GetAllGrupos()
        {
            return _grupos.ToList();
        }

        public Grupo? GetGrupoById(string idGrupo)
        {
            return _grupos.FirstOrDefault(g => g.IdGrupo == idGrupo);
        }

        public List<Grupo> GetGruposByProfesor(string cedulaProfesor)
        {
            return _grupos
                .Where(g => g.ProfesorAsignado != null && g.ProfesorAsignado.Cedula == cedulaProfesor)
                .ToList();
        }

        // --- Calificaciones ---
        public void AddCalificacion(Calificacion calificacion)
        {
            _calificaciones.Add(calificacion);
        }

        public List<Calificacion> GetCalificacionesByGrupo(string idGrupo)
        {
            return _calificaciones
                .Where(c => c.Grupo != null && c.Grupo.IdGrupo == idGrupo)
                .ToList();
        }

        public Calificacion? GetCalificacion(string idGrupo, string cedulaEstudiante)
        {
            return _calificaciones.FirstOrDefault(c =>
                c.Grupo != null && c.Grupo.IdGrupo == idGrupo &&
                c.Estudiante != null && c.Estudiante.Cedula == cedulaEstudiante);
        }

        // --- Asistencias ---
        public void AddAsistencia(AsistenciaProfesor asistencia)
        {
            _asistencias.Add(asistencia);
        }

        public List<AsistenciaProfesor> GetAsistenciasByProfesor(string cedulaProfesor)
        {
            return _asistencias
                .Where(a => a.Profesor.Cedula == cedulaProfesor)
                .ToList();
        }
    }
}
