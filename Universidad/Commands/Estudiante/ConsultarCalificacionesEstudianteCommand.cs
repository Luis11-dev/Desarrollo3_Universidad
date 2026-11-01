using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Repositories;
using Universidad.Util;

namespace Universidad.Commands.Estudiante
{
    public class ConsultarCalificacionesEstudianteCommand : ICommand
    {
        private readonly ProfesorRepository _profesorRepository;
        private readonly IConsoleWriter _writer;
        private readonly Models.Estudiante _estudianteActual;

        public ConsultarCalificacionesEstudianteCommand(
            ProfesorRepository profesorRepository,
            IConsoleWriter writer,
            Models.Estudiante estudianteActual)
        {
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _estudianteActual = estudianteActual ?? throw new ArgumentNullException(nameof(estudianteActual));
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("BOLETA DE CALIFICACIONES DEL ESTUDIANTE");

            try
            {
                List<Calificacion> calificaciones = _profesorRepository.GetCalificacionesByEstudiante(_estudianteActual.Cedula);

                _writer.ShowMessage($"Estudiante: {_estudianteActual.Nombre} | Carnet: {_estudianteActual.Cedula} | Carrera: {_estudianteActual.Carrera}\n");

                if (calificaciones.Count == 0)
                {
                    _writer.ShowMessage("No tiene notas registradas en el sistema para este periodo.");
                    _writer.WaitForKey();
                    return;
                }

                _writer.ShowMessage(string.Format(
                    "{0,-22} {1,-10} {2,-8} {3,-11} {4,-12} {5,-24}",
                    "GRUPO", "ACUM(60)", "EXAM(40)", "FINAL(100)", "CONVOC(80)", "ESTADO"));
                _writer.ShowMessage(new string('-', 92));

                foreach (var cal in calificaciones)
                {
                    string nombreGrupo = cal.Grupo != null ? cal.Grupo.NombreGrupo : "Sin grupo";

                    if (cal.Estado == EstadoCalificacion.Pendiente || cal.Nota == null)
                    {
                        _writer.ShowMessage(string.Format(
                            "{0,-22} {1,-10} {2,-8} {3,-11} {4,-12} {5,-24}",
                            nombreGrupo, "-", "-", "-", "-", "Nota Pendiente"));
                    }
                    else
                    {
                        string textoConvocatoria = cal.NotaConvocatoria.HasValue
                            ? $"{cal.NotaConvocatoria.Value} (Final: {cal.NotaFinalConvocatoria})"
                            : "-";

                        _writer.ShowMessage(string.Format(
                            "{0,-22} {1,-10} {2,-8} {3,-11} {4,-12} {5,-24}",
                            nombreGrupo,
                            cal.Acumulado,
                            cal.Examen,
                            cal.Nota,
                            textoConvocatoria,
                            FormatearEstado(cal.Estado)));
                    }
                }

                _writer.ShowMessage(new string('-', 92));
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
            }

            _writer.WaitForKey();
        }

        private string FormatearEstado(EstadoCalificacion estado)
        {
            switch (estado)
            {
                case EstadoCalificacion.Aprobado:
                    return "APROBADO";
                case EstadoCalificacion.ReprobadoConConvocatoria:
                    return "DERECHO A CONVOCATORIA";
                case EstadoCalificacion.ReprobadoSinConvocatoria:
                    return "REPROBADO";
                default:
                    return "PENDIENTE";
            }
        }

        public void Undo()
        {
        }
    }
}

