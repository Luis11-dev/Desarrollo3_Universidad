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
            new Carrera("C01", "Ingenieria en Sistemas"),
            new Carrera("C02", "Ingenieria Industrial")
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
            ProfesorRepository profesorRepository,
            EstudianteRepository estudianteRepository,
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

            // Semillas de Materias
            if (materiaRepository != null && materiaRepository.ObtenerTodas().Count == 0)
            {
                Materia mat1 = new Materia("MAT101", "Matematica I");
                Materia prog1 = new Materia("SIS101", "Introduccion a la Programacion");
                Materia prog2 = new Materia("SIS201", "Programacion Orientada a Objetos");
                prog2.AgregarPrerrequisito("SIS101");

                Materia desa3 = new Materia("SIS301", "Desarrollo de Aplicaciones III");
                desa3.AgregarPrerrequisito("SIS201");

                Materia bd1 = new Materia("SIS302", "Bases de Datos I");

                materiaRepository.Agregar(mat1);
                materiaRepository.Agregar(prog1);
                materiaRepository.Agregar(prog2);
                materiaRepository.Agregar(desa3);
                materiaRepository.Agregar(bd1);
            }

            // Semillas de Planes Académicos
            if (planRepository != null && planRepository.GetAll().Count == 0)
            {
                PlanAcademico plan1 = new PlanAcademico("PLAN-SIS-1-1", "C01", 1, 1);
                plan1.AgregarCodigoMateria("MAT101");
                plan1.AgregarCodigoMateria("SIS101");
                planRepository.Add(plan1);

                PlanAcademico plan3 = new PlanAcademico("PLAN-SIS-3-1", "C01", 3, 1);
                plan3.AgregarCodigoMateria("SIS301");
                plan3.AgregarCodigoMateria("SIS302");
                planRepository.Add(plan3);
            }

            Profesor? profesorActual = null;
            Grupo? grupoActual = null;

            if (profesorRepository.GetAll().Count == 0)
            {
                Profesor profesor = new Profesor(
                    cedula: "001-010190-0001A",
                    nombre: "Carlos Martinez",
                    correoElectronico: "carlos.martinez@universidad.edu",
                    numeroCelular: 88887777,
                    direccion: "Managua, Nicaragua",
                    genero: Genero.Masculino,
                    estadoCivil: "Casado",
                    profesion: "Ingeniero en Sistemas",
                    especialidad: "Bases de Datos",
                    user: "profesor1",
                    password: "1234"
                );

                profesorRepository.Add(profesor);
                profesorActual = profesor;

                Grupo grupo = new Grupo(
                    idGrupo: "G01",
                    nombreGrupo: "Grupo ISI-IIIB",
                    carrera: "Ingenieria en Sistemas",
                    anioAcademico: 3,
                    semestre: 1,
                    modalidad: Modalidad.Regular,
                    cupos: 30
                );

                profesorRepository.AddGrupo(grupo);
                grupoActual = grupo;

                if (estudianteRepository.GetAll().Count == 0)
                {
                    Estudiante estudiante = new Estudiante(
                        cedula: "001-020202-0002B",
                        nombre: "Ana Lopez",
                        fechaNacimiento: new DateTime(2002, 5, 10),
                        correoElectronico: "ana.lopez@universidad.edu",
                        numeroCelular: 87654321,
                        direccion: "Managua, Nicaragua",
                        genero: Genero.Femenino,
                        carrera: "Ingenieria en Sistemas",
                        activo: true,
                        user: "estudiante1",
                        password: "1234"
                    );

                    estudianteRepository.Add(estudiante);
                    grupo.AgregarEstudiante(estudiante);

                    // Materia previa aprobada (Prerrequisito SIS201 - POO)
                    Materia materiaPOO = materiaRepository?.ObtenerPorCodigo("SIS201")
                        ?? new Materia("SIS201", "Programacion Orientada a Objetos");

                    Grupo grupoAnterior = new Grupo(
                        idGrupo: "G00",
                        nombreGrupo: "Grupo ISI-IIA",
                        carrera: "Ingenieria en Sistemas",
                        anioAcademico: 2,
                        semestre: 1,
                        modalidad: Modalidad.Regular,
                        cupos: 30
                    );
                    grupoAnterior.AgregarEstudiante(estudiante);

                    Calificacion calificacionPrevia = new Calificacion(estudiante, grupoAnterior, materiaPOO);
                    calificacionPrevia.RegistrarNotas(50, 35); // 85 / 100 -> Aprobado
                    profesorRepository.AddCalificacion(calificacionPrevia);

                    Materia materiaDesaInicial = materiaRepository?.ObtenerPorCodigo("SIS301") 
                        ?? new Materia("SIS301", "Desarrollo de Aplicaciones III");

                    Calificacion calificacion = new Calificacion(estudiante, grupo, materiaDesaInicial);
                    profesorRepository.AddCalificacion(calificacion);
                }
            }
            else
            {
                profesorActual = profesorRepository.GetAll()[0];
                grupoActual = profesorRepository.GetAllGrupos()[0];
            }

            // Semilla de HorarioClase
            if (horarioRepository != null && horarioRepository.GetAll().Count == 0 && profesorActual != null && grupoActual != null)
            {
                Materia materiaDesa = materiaRepository?.ObtenerPorCodigo("SIS301") 
                    ?? new Materia("SIS301", "Desarrollo de Aplicaciones III");

                HorarioClase horario = new HorarioClase(
                    idHorario: "H01",
                    grupo: grupoActual,
                    materia: materiaDesa,
                    profesor: profesorActual,
                    dia: "Lunes",
                    horaInicio: new TimeSpan(8, 0, 0),
                    horaFin: new TimeSpan(11, 0, 0),
                    modalidad: Modalidad.Regular
                );

                if (estudianteRepository.GetAll().Count > 0)
                {
                    horario.InscribirEstudiante(estudianteRepository.GetAll()[0]);
                }

                horarioRepository.Add(horario);

                if (inscripcionRepository != null && inscripcionRepository.ObtenerTodas().Count == 0 && estudianteRepository.GetAll().Count > 0)
                {
                    try
                    {
                        inscripcionRepository.Inscribir(estudianteRepository.GetAll()[0], horario, "2026-I");
                    }
                    catch
                    {
                        // En caso de que ya estuviese matriculado
                    }
                }
            }

            // Semilla de Contrato
            if (contratoRepository != null && contratoRepository.GetAll().Count == 0 && profesorActual != null)
            {
                Contrato contrato = new Contrato(
                    idContrato: "CTR-2026-001",
                    profesor: profesorActual,
                    periodoAcademico: "2026-I",
                    tarifaPorHora: 400m
                );

                if (horarioRepository != null)
                {
                    foreach (HorarioClase h in horarioRepository.GetByProfesor(profesorActual.Cedula))
                    {
                        contrato.AsignarMateria(h);
                    }
                }

                contratoRepository.Add(contrato);
            }
        }
    }
}
