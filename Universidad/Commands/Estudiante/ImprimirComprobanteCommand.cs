using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Estudiante
{
    public class ImprimirComprobanteCommand : ICommand
    {
        private readonly InscripcionService _inscripcionService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Estudiante _estudianteActual;

        public ImprimirComprobanteCommand(
            InscripcionService inscripcionService,
            IConsoleWriter writer,
            Models.Estudiante estudianteActual)
        {
            _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _estudianteActual = estudianteActual ?? throw new ArgumentNullException(nameof(estudianteActual));
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("COMPROBANTE OFICIAL DE MATRICULA");

            try
            {
                List<Inscripcion> inscripciones = _inscripcionService.ObtenerInscripcionesEstudiante(_estudianteActual.Cedula);

                if (inscripciones.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene matriculas registradas para generar comprobante.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opciones = new string[inscripciones.Count + 1];
                for (int i = 0; i < inscripciones.Count; i++)
                {
                    var ins = inscripciones[i];
                    opciones[i] = $"{ins.Materia.Nombre} (No: {ins.IdInscripcion} | Grupo: {ins.Grupo.NombreGrupo})";
                }
                opciones[inscripciones.Count] = "Volver al menu anterior";

                _writer.DisplayMenu("Seleccione la matricula para generar comprobante:", opciones);

                string input = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, inscripciones.Count + 1)
                    .ReadAndValidate();

                int indice = int.Parse(input) - 1;
                if (indice == inscripciones.Count)
                {
                    return;
                }

                Inscripcion inscripcionSeleccionada = inscripciones[indice];
                string comprobante = _inscripcionService.GenerarComprobante(inscripcionSeleccionada);

                _writer.Clear();
                _writer.ShowMessage(comprobante + "\n");
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

