using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor.Calificaciones
{
    public class RegistrarConvocatoriaCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;
        private readonly Grupo _grupo;

        public RegistrarConvocatoriaCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual, Grupo grupo)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
            _grupo = grupo;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader($"REGISTRAR CONVOCATORIA - GRUPO {_grupo.NombreGrupo}");

            try
            {
                _profesorService.ValidarFechaTope();

                List<Calificacion> candidatos = _profesorService
                    .ObtenerEstudiantesConDerechoAConvocatoria(_profesorActual, _grupo.IdGrupo);

                if (candidatos.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay estudiantes con derecho a convocatoria pendientes en este grupo.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesCandidatos = new string[candidatos.Count + 1];
                for (int i = 0; i < candidatos.Count; i++)
                {
                    var est = candidatos[i].Estudiante;
                    opcionesCandidatos[i] = $"{est?.Nombre} (Cedula: {est?.Cedula} | Nota actual: {candidatos[i].Nota})";
                }
                opcionesCandidatos[candidatos.Count] = "Cancelar y volver";

                _writer.DisplayMenu("Estudiantes con derecho a convocatoria (Nota entre 40 y 59):", opcionesCandidatos);

                string inputCand = new Validator("Seleccione el estudiante: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, candidatos.Count + 1)
                    .ReadAndValidate();

                int indiceCand = int.Parse(inputCand) - 1;
                if (indiceCand == candidatos.Count)
                {
                    return;
                }

                Calificacion calificacionSeleccionada = candidatos[indiceCand];
                PerfilUniversitario estudiante = calificacionSeleccionada.Estudiante!;

                _writer.ShowMessage($"\nRegistrando convocatoria para: {estudiante.Nombre} (Nota ordinaria: {calificacionSeleccionada.Nota})");

                int notaExtraEnEspera = int.Parse(new ValidatorProfesor("Ingrese la nota extraordinaria de convocatoria (0 a 100): ")
                    .NotEmpty()
                    .NotaEnRango(0, 100)
                    .ReadAndValidate());

                bool decisionTomada = false;
                while (!decisionTomada)
                {
                    // tope de 80 segun la guia
                    int notaConTope = Math.Min(notaExtraEnEspera, Calificacion.TopeConvocatoria);
                    EstadoCalificacion estadoFinal = notaConTope >= Calificacion.NotaAprobacion
                        ? EstadoCalificacion.Aprobado
                        : EstadoCalificacion.ReprobadoSinConvocatoria;

                    _writer.Clear();
                    _writer.ShowHeader("VISTA PREVIA Y CONFIRMACION DE CONVOCATORIA");
                    _writer.ShowMessage($"Grupo:                        {_grupo.NombreGrupo} - {_grupo.NombreMateria}");
                    _writer.ShowMessage($"Estudiante:                   {estudiante.Nombre} ({estudiante.Cedula})");
                    _writer.ShowMessage($"Nota Ordinaria Previa:        {calificacionSeleccionada.Nota}");
                    _writer.ShowMessage(new string('-', 65));
                    _writer.ShowMessage($"Nota Extraordinaria Digita:   {notaExtraEnEspera} / 100");
                    _writer.ShowMessage($"Nota Aplicada (Tope max 80):  {notaConTope} / 80");
                    _writer.ShowMessage($"Estado Resultante:            {FormatearEstado(estadoFinal)}");
                    _writer.ShowMessage(new string('-', 65));
                    _writer.ShowMessage("ADVERTENCIA: La nota extraordinaria NO se puede editar una vez guardada y el estudiante solo tiene una convocatoria por clase.\n");

                    _writer.DisplayMenu(
                        "¿Que desea hacer con este registro en espera?",
                        "Confirmar y guardar convocatoria definitivamente",
                        "Editar campo: Nota extraordinaria (0 a 100)",
                        "Anular accion y descartar cambios"
                    );

                    string opcionConfirm = new Validator("Ingrese su opcion: ")
                        .NotEmpty()
                        .OnlyDigits()
                        .InputBetweenXandY(1, 3)
                        .ReadAndValidate();

                    switch (int.Parse(opcionConfirm))
                    {
                        case 1:
                            _profesorService.RegistrarNotaConvocatoria(
                                _profesorActual,
                                _grupo.IdGrupo,
                                estudiante.Cedula,
                                notaExtraEnEspera);

                            _writer.ShowMessage("\nNota de convocatoria registrada exitosamente.");
                            decisionTomada = true;
                            new VerNotasGrupoCommand(_profesorService, _writer, _profesorActual, _grupo).ImprimirTablaNotas();
                            _writer.WaitForKey();
                            break;

                        case 2:
                            notaExtraEnEspera = int.Parse(new ValidatorProfesor("\nIngrese el nuevo valor de la nota extraordinaria (0 a 100): ")
                                .NotEmpty()
                                .NotaEnRango(0, 100)
                                .ReadAndValidate());
                            break;

                        case 3:
                            _writer.ShowMessage("\nAccion anulada. No se registro la convocatoria.");
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
