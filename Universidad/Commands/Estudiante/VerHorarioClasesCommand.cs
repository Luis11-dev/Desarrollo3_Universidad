using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Estudiante
{
    public class VerHorarioClasesCommand : ICommand
    {
        private readonly InscripcionService _inscripcionService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Estudiante _estudianteActual;

        public VerHorarioClasesCommand(
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
            _writer.ShowHeader("MI HORARIO DE CLASES Y MATERIAS INSCRITAS");

            try
            {
                List<Inscripcion> inscripciones = _inscripcionService.ObtenerInscripcionesEstudiante(_estudianteActual.Cedula);

                _writer.ShowMessage($"Estudiante: {_estudianteActual.Nombre} | Carnet: {_estudianteActual.Cedula} | Carrera: {_estudianteActual.Carrera}\n");

                if (inscripciones.Count == 0)
                {
                    _writer.ShowMessage("No tiene materias inscritas para este periodo academico.");
                    _writer.WaitForKey();
                    return;
                }

                _writer.ShowMessage(string.Format(
                    "{0,-10} {1,-26} {2,-10} {3,-10} {4,-22} {5,-24}",
                    "CODIGO", "MATERIA", "GRUPO", "MODALIDAD", "HORARIO", "PROFESOR"));
                _writer.ShowMessage(new string('-', 106));

                foreach (var ins in inscripciones)
                {
                    string horario = $"{ins.Horario.Dia} {ins.Horario.HoraInicio:hh\\:mm}-{ins.Horario.HoraFin:hh\\:mm}";
                    _writer.ShowMessage(string.Format(
                        "{0,-10} {1,-26} {2,-10} {3,-10} {4,-22} {5,-24}",
                        ins.Materia.Codigo,
                        ins.Materia.Nombre,
                        ins.Grupo.NombreGrupo,
                        ins.Horario.Modalidad,
                        horario,
                        ins.Horario.Profesor.Nombre));
                }

                _writer.ShowMessage(new string('-', 106));
                _writer.ShowMessage($"Total asignaturas inscritas: {inscripciones.Count}\n");
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

