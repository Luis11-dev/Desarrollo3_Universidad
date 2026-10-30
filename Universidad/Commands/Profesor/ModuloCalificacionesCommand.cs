using System;
using System.Collections.Generic;
using Universidad.Commands.Profesor.Calificaciones;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor
{
    public class ModuloCalificacionesCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;

        public ModuloCalificacionesCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("MODULO DE CALIFICACIONES");

            try
            {
                List<Grupo> grupos = _profesorService.ObtenerGruposAsignados(_profesorActual);
                if (grupos.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene grupos asignados para gestionar calificaciones.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesGrupos = new string[grupos.Count + 1];
                for (int i = 0; i < grupos.Count; i++)
                {
                    opcionesGrupos[i] = $"{grupos[i].NombreMateria} (Grupo: {grupos[i].NombreGrupo} - {grupos[i].Modalidad})";
                }
                opcionesGrupos[grupos.Count] = "Volver al menu anterior";

                _writer.DisplayMenu("1. Seleccione uno de sus grupos asignados:", opcionesGrupos);

                string inputGrupo = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, grupos.Count + 1)
                    .ReadAndValidate();

                int indiceGrupo = int.Parse(inputGrupo) - 1;
                if (indiceGrupo == grupos.Count)
                {
                    return;
                }

                Grupo grupoSeleccionado = grupos[indiceGrupo];
                MostrarSubmenuCalificaciones(grupoSeleccionado);
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
                _writer.WaitForKey();
            }
        }

        private void MostrarSubmenuCalificaciones(Grupo grupo)
        {
            bool salirSubmenu = false;

            do
            {
                _writer.Clear();
                _writer.ShowHeader($"CALIFICACIONES - {grupo.NombreMateria.ToUpper()} (GRUPO {grupo.NombreGrupo})");

                // solo habilitamos convocatoria si hay alumnos que quedaron entre 40 y 59
                List<Calificacion> candidatosConvocatoria = _profesorService
                    .ObtenerEstudiantesConDerechoAConvocatoria(_profesorActual, grupo.IdGrupo);

                bool habilitarConvocatoria = candidatosConvocatoria.Count > 0;

                if (habilitarConvocatoria)
                {
                    _writer.DisplayMenu(
                        "Seleccione una accion:",
                        "Registrar notas",
                        "Ver notas",
                        $"Registrar convocatoria ({candidatosConvocatoria.Count} estudiante(s) con derecho)",
                        "Volver"
                    );

                    string input = new Validator("Ingrese su opcion: ")
                        .NotEmpty()
                        .OnlyDigits()
                        .InputBetweenXandY(1, 4)
                        .ReadAndValidate();

                    int opcion = int.Parse(input);
                    switch (opcion)
                    {
                        case 1:
                            new RegistrarNotaCommand(_profesorService, _writer, _profesorActual, grupo).Execute();
                            break;
                        case 2:
                            new VerNotasGrupoCommand(_profesorService, _writer, _profesorActual, grupo).Execute();
                            break;
                        case 3:
                            new RegistrarConvocatoriaCommand(_profesorService, _writer, _profesorActual, grupo).Execute();
                            break;
                        case 4:
                            salirSubmenu = true;
                            break;
                    }
                }
                else
                {
                    _writer.DisplayMenu(
                        "Seleccione una accion:",
                        "Registrar notas",
                        "Ver notas",
                        "Volver"
                    );

                    string input = new Validator("Ingrese su opcion: ")
                        .NotEmpty()
                        .OnlyDigits()
                        .InputBetweenXandY(1, 3)
                        .ReadAndValidate();

                    int opcion = int.Parse(input);
                    switch (opcion)
                    {
                        case 1:
                            new RegistrarNotaCommand(_profesorService, _writer, _profesorActual, grupo).Execute();
                            break;
                        case 2:
                            new VerNotasGrupoCommand(_profesorService, _writer, _profesorActual, grupo).Execute();
                            break;
                        case 3:
                            salirSubmenu = true;
                            break;
                    }
                }

            } while (!salirSubmenu);
        }

        public void Undo()
        {
        }
    }
}
