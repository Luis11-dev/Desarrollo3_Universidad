using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor
{
    public class VerEstudiantesGrupoCommand : ICommand
    {
        private readonly IConsoleWriter _writer;
        private readonly ProfesorService _profesorService;
        private readonly Models.Profesor _profesorActual;

        public VerEstudiantesGrupoCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("ESTUDIANTES POR MATERIA Y GRUPO ASIGNADO");

            try
            {
                List<HorarioClase> materias = _profesorService.ObtenerMateriasAsignadas(_profesorActual);

                if (materias.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene materias ni grupos asignados en este momento.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesMaterias = new string[materias.Count];
                for (int i = 0; i < materias.Count; i++)
                {
                    var m = materias[i];
                    opcionesMaterias[i] = $"{m.Materia.Nombre} (Grupo: {m.Grupo.NombreGrupo} - Mod: {m.Modalidad} - {m.Dia} {m.HoraInicio:hh\\:mm} a {m.HoraFin:hh\\:mm})";
                }

                _writer.DisplayMenu("Seleccione la materia / grupo a consultar:", opcionesMaterias);

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, materias.Count)
                    .ReadAndValidate();

                int indiceSeleccionado = int.Parse(input) - 1;
                HorarioClase materiaSeleccionada = materias[indiceSeleccionado];

                _writer.Clear();
                _writer.ShowHeader($"ESTUDIANTES DEL GRUPO: {materiaSeleccionada.Grupo.NombreGrupo} ({materiaSeleccionada.Materia.Nombre.ToUpper()})");

                IReadOnlyCollection<PerfilUniversitario> estudiantes = _profesorService.VerEstudiantesDeGrupo(_profesorActual, materiaSeleccionada.Grupo.IdGrupo);

                if (estudiantes.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay estudiantes inscritos en este grupo todavia.");
                }
                else
                {
                    _writer.ShowMessage($"Total inscritos en cohorte: {estudiantes.Count} | Cupos disponibles: {Math.Max(0, materiaSeleccionada.Grupo.Cupos - estudiantes.Count)}\n");
                    _writer.ShowMessage(string.Format("{0,-15} {1,-35} {2,-30}", "CEDULA", "NOMBRE COMPLETO", "CORREO ELECTRONICO"));
                    _writer.ShowMessage(new string('-', 82));

                    foreach (var estudiante in estudiantes)
                    {
                        _writer.ShowMessage(string.Format("{0,-15} {1,-35} {2,-30}", estudiante.Cedula, estudiante.Nombre, estudiante.CorreoElectronico));
                    }
                }
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
            }

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
