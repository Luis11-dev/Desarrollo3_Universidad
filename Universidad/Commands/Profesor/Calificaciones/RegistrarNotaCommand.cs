using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor.Calificaciones
{
    public class RegistrarNotaCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;
        private readonly Grupo _grupo;

        public RegistrarNotaCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual, Grupo grupo)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
            _grupo = grupo;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader($"REGISTRAR NOTAS - GRUPO {_grupo.NombreGrupo}");

            try
            {
                // cargamos los que todavia no tienen nota puesta
                List<Calificacion> pendientes = _profesorService.ObtenerEstudiantesSinNota(_profesorActual, _grupo.IdGrupo);

                if (pendientes.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay estudiantes pendientes de calificar en este grupo (o todos ya tienen su nota registrada).");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesEstudiantes = new string[pendientes.Count + 1];
                for (int i = 0; i < pendientes.Count; i++)
                {
                    var est = pendientes[i].Estudiante;
                    opcionesEstudiantes[i] = $"{est?.Nombre} (Cedula: {est?.Cedula})";
                }
                opcionesEstudiantes[pendientes.Count] = "Cancelar y volver";

                _writer.DisplayMenu("Estudiantes inscritos sin nota registrada:", opcionesEstudiantes);

                string inputEst = new Validator("Seleccione el estudiante a calificar: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, pendientes.Count + 1)
                    .ReadAndValidate();

                int indiceEst = int.Parse(inputEst) - 1;
                if (indiceEst == pendientes.Count)
                {
                    return;
                }

                Calificacion calificacionSeleccionada = pendientes[indiceEst];
                PerfilUniversitario estudiante = calificacionSeleccionada.Estudiante!;

                _writer.ShowMessage($"\nIngresando calificaciones para: {estudiante.Nombre} ({estudiante.Cedula})");

                int acumuladoEnEspera = PedirNotaConExcepcion("Ingrese el Acumulado (0 a 60): ", 0, 60);
                int examenEnEspera = PedirNotaConExcepcion("Ingrese el Examen (0 a 40): ", 0, 40);

                // confirmacion antes de guardar para que pueda corregir o cancelar
                bool decisionTomada = false;
                while (!decisionTomada)
                {
                    var precalculo = _profesorService.PrecalcularNota(acumuladoEnEspera, examenEnEspera);

                    _writer.Clear();
                    _writer.ShowHeader("VISTA PREVIA Y CONFIRMACION DE NOTA");
                    _writer.ShowMessage($"Grupo:           {_grupo.NombreGrupo} - {_grupo.NombreMateria}");
                    _writer.ShowMessage($"Estudiante:      {estudiante.Nombre} ({estudiante.Cedula})");
                    _writer.ShowMessage(new string('-', 65));
                    _writer.ShowMessage($"Acumulado (60):  {acumuladoEnEspera}");
                    _writer.ShowMessage($"Examen (40):     {examenEnEspera}");
                    _writer.ShowMessage($"Nota Final:      {acumuladoEnEspera} + {examenEnEspera} = {precalculo.NotaFinal} / 100");
                    _writer.ShowMessage($"Estado:          {FormatearEstado(precalculo.Estado)}");
                    _writer.ShowMessage(new string('-', 65));
                    _writer.ShowMessage("ADVERTENCIA: Una vez guardadas, las notas NO se podran editar.\n");

                    _writer.DisplayMenu(
                        "¿Que desea hacer con este registro en espera?",
                        "Confirmar y guardar calificacion definitivamente",
                        "Editar campo: Acumulado (0 a 60)",
                        "Editar campo: Examen (0 a 40)",
                        "Anular accion y descartar cambios"
                    );

                    string opcionConfirm = new Validator("Ingrese su opcion: ")
                        .NotEmpty()
                        .OnlyDigits()
                        .InputBetweenXandY(1, 4)
                        .ReadAndValidate();

                    switch (int.Parse(opcionConfirm))
                    {
                        case 1:
                            _profesorService.RegistrarNotaEstudiante(
                                _profesorActual,
                                _grupo.IdGrupo,
                                estudiante.Cedula,
                                acumuladoEnEspera,
                                examenEnEspera);

                            _writer.ShowMessage("\nCalificacion guardada exitosamente.");
                            decisionTomada = true;

                            // mostramos la lista completa para verificar que se guardo
                            new VerNotasGrupoCommand(_profesorService, _writer, _profesorActual, _grupo).ImprimirTablaNotas();
                            _writer.WaitForKey();
                            break;

                        case 2:
                            acumuladoEnEspera = PedirNotaConExcepcion("\nIngrese el nuevo valor de Acumulado (0 a 60): ", 0, 60);
                            break;

                        case 3:
                            examenEnEspera = PedirNotaConExcepcion("\nIngrese el nuevo valor de Examen (0 a 40): ", 0, 40);
                            break;

                        case 4:
                            _writer.ShowMessage("\nAccion anulada. No se guardo ninguna calificacion.");
                            decisionTomada = true;
                            _writer.WaitForKey();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
                _writer.WaitForKey();
            }
        }

        private int PedirNotaConExcepcion(string mensaje, int min, int max)
        {
            while (true)
            {
                try
                {
                    Console.Write(mensaje);
                    string entrada = (Console.ReadLine() ?? string.Empty).Trim();

                    if (!int.TryParse(entrada, out int valor))
                    {
                        throw new FormatException("El valor ingresado no es numerico. Debe ingresar un numero entero.");
                    }

                    if (valor < min || valor > max)
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(valor),
                            $"El valor ingresado ({valor}) esta fuera del rango permitido ({min} a {max}).");
                    }

                    return valor;
                }
                catch (Exception ex)
                {
                    _writer.ShowError(ex.Message);
                }
            }
        }

        private string FormatearEstado(EstadoCalificacion estado)
        {
            return estado switch
            {
                EstadoCalificacion.Pendiente => "Nota pendiente",
                EstadoCalificacion.Aprobado => "Aprobado",
                EstadoCalificacion.ReprobadoConConvocatoria => "Reprobado (Derecho a Convoc.)",
                EstadoCalificacion.ReprobadoSinConvocatoria => "Reprobado sin derecho",
                _ => estado.ToString()
            };
        }

        public void Undo()
        {
        }
    }
}

