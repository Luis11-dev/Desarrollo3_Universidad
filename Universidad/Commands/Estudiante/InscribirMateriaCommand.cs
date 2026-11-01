using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Estudiante
{
    public class InscribirMateriaCommand : ICommand
    {
        private readonly InscripcionService _inscripcionService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Estudiante _estudianteActual;

        public InscribirMateriaCommand(
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
            _writer.ShowHeader("INSCRIPCION DE ASIGNATURAS");

            try
            {
                string periodo = "2026-I";
                List<HorarioClase> disponibles = _inscripcionService.ObtenerHorariosDisponibles(_estudianteActual, periodo);

                if (disponibles.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay materias disponibles para inscripcion en este momento (o ya inscribio todas las asignaturas ofertadas).");
                    _writer.WaitForKey();
                    return;
                }

                string[] opciones = new string[disponibles.Count + 1];
                for (int i = 0; i < disponibles.Count; i++)
                {
                    var h = disponibles[i];
                    opciones[i] = $"[{h.Materia.Codigo}] {h.Materia.Nombre} | Grupo: {h.Grupo.NombreGrupo} | {h.Dia} {h.HoraInicio:hh\\:mm}-{h.HoraFin:hh\\:mm} | Prof: {h.Profesor.Nombre} [Cupos: {h.Grupo.Cupos - h.EstudiantesInscritos.Count}]";
                }
                opciones[disponibles.Count] = "Volver al menu anterior";

                _writer.DisplayMenu("Seleccione la materia que desea inscribir:", opciones);

                string input = new Validator("Ingrese opcion de materia: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, disponibles.Count + 1)
                    .ReadAndValidate();

                int indice = int.Parse(input) - 1;
                if (indice == disponibles.Count)
                {
                    return;
                }

                HorarioClase materiaSeleccionada = disponibles[indice];

                _writer.Clear();
                _writer.ShowHeader("CONFIRMAR MATRICULA DE ASIGNATURA");
                _writer.ShowMessage($"Asignatura:          [{materiaSeleccionada.Materia.Codigo}] {materiaSeleccionada.Materia.Nombre}");
                _writer.ShowMessage($"Grupo / Cohorte:     {materiaSeleccionada.Grupo.NombreGrupo} ({materiaSeleccionada.Grupo.Carrera})");
                _writer.ShowMessage($"Horario de clases:   {materiaSeleccionada.Dia} de {materiaSeleccionada.HoraInicio:hh\\:mm} a {materiaSeleccionada.HoraFin:hh\\:mm} ({materiaSeleccionada.Modalidad})");
                _writer.ShowMessage($"Profesor:            {materiaSeleccionada.Profesor.Nombre}");
                _writer.ShowMessage(new string('-', 60));

                _writer.DisplayMenu(
                    "¿Desea formalizar la inscripcion?",
                    "Confirmar e inscribir materia",
                    "Cancelar"
                );

                string confirm = new Validator("Ingrese opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 2)
                    .ReadAndValidate();

                if (int.Parse(confirm) == 1)
                {
                    string mensaje;
                    bool exitoso = _inscripcionService.InscribirEstudiante(_estudianteActual, materiaSeleccionada, periodo, out mensaje);

                    if (exitoso)
                    {
                        _writer.ShowMessage($"\n{mensaje}");
                    }
                    else
                    {
                        _writer.ShowError($"\nError al inscribir: {mensaje}");
                    }
                }
                else
                {
                    _writer.ShowMessage("\nInscripcion cancelada.");
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

