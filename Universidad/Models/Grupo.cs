using System;
using System.Collections.Generic;

namespace Universidad.Models
{
    public class Grupo
    {
        public string IdGrupo { get; private set; } = string.Empty;
        public string NombreGrupo { get; private set; } = string.Empty;
        public string Carrera { get; private set; } = string.Empty;
        public int AnioAcademico { get; private set; }
        public int Semestre { get; private set; }
        public Modalidad Modalidad { get; private set; }
        public int Cupos { get; private set; }

        private readonly List<PerfilUniversitario> _estudiantes = new();
        public IReadOnlyCollection<PerfilUniversitario> Estudiantes
        {
            get
            {
                return _estudiantes.AsReadOnly();
            }
        }

        public Grupo() { }

        public Grupo(
            string idGrupo,
            string nombreGrupo,
            string carrera,
            int anioAcademico,
            int semestre,
            Modalidad modalidad,
            int cupos)
        {
            IdGrupo = idGrupo;
            NombreGrupo = nombreGrupo;
            Carrera = carrera;
            AnioAcademico = anioAcademico;
            Semestre = semestre;
            Modalidad = modalidad;
            Cupos = cupos;
        }

        public void AgregarEstudiante(PerfilUniversitario estudiante)
        {
            if (estudiante != null && !_estudiantes.Contains(estudiante))
            {
                _estudiantes.Add(estudiante);
            }
        }
    }
}
