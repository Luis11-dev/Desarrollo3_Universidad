using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Data
{
    public static class Seeds
    {
        public static List<Carrera> Carreras { get; } = new List<Carrera>
        {
            new Carrera("C01", "Ingenieria en Sistemas")
        };

        public static List<AnioAcademico> AniosAcademicos { get; } = new List<AnioAcademico>
        {
            new AnioAcademico(1, "Primer Año"),
            new AnioAcademico(2, "Segundo Año"),
            new AnioAcademico(3, "Tercer Año"),
            new AnioAcademico(4, "Cuarto Año"),
            new AnioAcademico(5, "Quinto Año")
        };

        public static List<Semestre> Semestres { get; } = new List<Semestre>
        {
            new Semestre(1, "I Semestre"),
            new Semestre(2, "II Semestre")
        };

        public static void Inicializar(
            ProfesorRepository? profesorRepository = null,
            EstudianteRepository? estudianteRepository = null,
            AdministradorRepository? administradorRepository = null,
            MateriaRepository? materiaRepository = null,
            PlanAcademicoRepository? planRepository = null,
            HorarioClaseRepository? horarioRepository = null,
            ContratoRepository? contratoRepository = null,
            InscripcionRepository? inscripcionRepository = null)
        {
            if (administradorRepository != null && administradorRepository.GetAll().Count == 0)
            {
                Administrador admin = new Administrador(
                    cedula: "001-010180-0000A",
                    nombre: "Administrador del Sistema",
                    correoElectronico: "admin@universidad.edu",
                    numeroCelular: 88889999,
                    direccion: "Oficina Central de Registro",
                    genero: Genero.Masculino,
                    user: "admin",
                    password: "1234"
                );
                administradorRepository.Add(admin);
            }

            if (materiaRepository != null && materiaRepository.ObtenerTodas().Count == 0)
            {
                Materia matDesa = new Materia("SIS301", "Desarrollo de aplicaciones 3");
                Materia matIngSoft = new Materia("SIS302", "Ingenieria de Software 2");
                Materia matEsp = new Materia("HUM301", "Español 4");
                Materia matCivica = new Materia("HUM302", "Moral y civica 2");

                materiaRepository.Agregar(matDesa);
                materiaRepository.Agregar(matIngSoft);
                materiaRepository.Agregar(matEsp);
                materiaRepository.Agregar(matCivica);
            }

            if (planRepository != null && planRepository.GetAll().Count == 0)
            {
                PlanAcademico plan3 = new PlanAcademico("PLAN-SIS-3-2", "C01", 3, 2);
                plan3.AgregarCodigoMateria("SIS301");
                plan3.AgregarCodigoMateria("SIS302");
                plan3.AgregarCodigoMateria("HUM301");
                plan3.AgregarCodigoMateria("HUM302");
                planRepository.Add(plan3);
            }

            if (profesorRepository != null && profesorRepository.GetAllGrupos().Count == 0)
            {
                Grupo grupo1 = new Grupo(
                    idGrupo: "G01",
                    nombreGrupo: "Grupo ISI-3T1",
                    carrera: "Ingenieria en Sistemas",
                    anioAcademico: 3,
                    semestre: 2,
                    modalidad: Modalidad.Regular,
                    cupos: 30
                );

                Grupo grupo2 = new Grupo(
                    idGrupo: "G02",
                    nombreGrupo: "Grupo ISI-3T2",
                    carrera: "Ingenieria en Sistemas",
                    anioAcademico: 3,
                    semestre: 2,
                    modalidad: Modalidad.Regular,
                    cupos: 30
                );

                profesorRepository.AddGrupo(grupo1);
                profesorRepository.AddGrupo(grupo2);
            }
        }
    }
}
