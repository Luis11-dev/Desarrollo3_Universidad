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
            _writer.ShowHeader("ESTUDIANTES ASIGNADOS POR GRUPO");

            try
            {
                List<Grupo> grupos = _profesorService.ObtenerGruposAsignados(_profesorActual);

                if (grupos.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene grupos asignados en este momento.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesGrupos = new string[grupos.Count];
                for (int i = 0; i < grupos.Count; i++)
                {
                    opcionesGrupos[i] = $"{grupos[i].NombreMateria} (Grupo: {grupos[i].NombreGrupo} - Mod: {grupos[i].Modalidad} - {grupos[i].Dia} {grupos[i].HoraInicio:hh\\:mm} a {grupos[i].HoraFin:hh\\:mm})";
                }

                _writer.DisplayMenu("Seleccione el grupo a consultar:", opcionesGrupos);

                string input = new Validator("Ingrese el numero de grupo: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, grupos.Count)
                    .ReadAndValidate();

                int indiceSeleccionado = int.Parse(input) - 1;
                Grupo grupoSeleccionado = grupos[indiceSeleccionado];

                _writer.Clear();
                _writer.ShowHeader($"ESTUDIANTES DEL GRUPO: {grupoSeleccionado.NombreGrupo} ({grupoSeleccionado.NombreMateria.ToUpper()})");

                IReadOnlyCollection<PerfilUniversitario> estudiantes = _profesorService.VerEstudiantesDeGrupo(_profesorActual, grupoSeleccionado.IdGrupo);

                if (estudiantes.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay estudiantes inscritos en este grupo todavia.");
                }
                else
                {
                    _writer.ShowMessage($"Total inscritos: {estudiantes.Count} | Cupos disponibles: {Math.Max(0, grupoSeleccionado.Cupos - estudiantes.Count)}\n");
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
