using System;
using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Data
{
    public static class Seeds
    {
        public static void Inicializar(ProfesorRepository profesorRepository, EstudianteRepository estudianteRepository)
        {
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

                Grupo grupo = new Grupo(
                    idGrupo: "G01",
                    nombreGrupo: "Grupo 3T1-SIS",
                    materia: "Desarrollo de Aplicaciones III",
                    cupos: 30,
                    semestre: 1,
                    modalidad: Modalidad.Regular,
                    dia: "Lunes",
                    horaInicio: new TimeSpan(8, 0, 0),
                    horaFin: new TimeSpan(11, 0, 0),
                    profesorAsignado: profesor
                );

                profesorRepository.AddGrupo(grupo);

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

                    Calificacion calificacion = new Calificacion(estudiante, grupo);
                    profesorRepository.AddCalificacion(calificacion);
                }
            }
        }
    }
}
