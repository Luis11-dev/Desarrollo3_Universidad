using System;
using System.Linq;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor.Calificaciones
{
    public class VerNotasGrupoCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;
        private readonly Grupo _grupo;

        public VerNotasGrupoCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual, Grupo grupo)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
            _grupo = grupo;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader($"LISTADO DE NOTAS - GRUPO {_grupo.NombreGrupo} ({_grupo.NombreMateria.ToUpper()})");

            try
            {
                ImprimirTablaNotas();
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
            }

            _writer.WaitForKey();
        }

        public void ImprimirTablaNotas()
        {
            var resultado = _profesorService.VerNotasDeGrupo(_profesorActual, _grupo.IdGrupo);

            if (resultado.Calificaciones.Count == 0)
            {
                _writer.ShowMessage("\nNo hay estudiantes inscritos en este grupo.");
                return;
            }

            _writer.ShowMessage($"\n--- LISTA DE NOTAS DEL GRUPO {_grupo.NombreGrupo} ({_grupo.NombreMateria}) ---");
            _writer.ShowMessage(string.Format(
                "{0,-14} {1,-26} {2,-10} {3,-8} {4,-11} {5,-13} {6,-28}",
                "CEDULA", "ESTUDIANTE", "ACUM(60)", "EXAM(40)", "FINAL(100)", "CONVOC(80)", "ESTADO"));
            _writer.ShowMessage(new string('-', 115));

            foreach (var cal in resultado.Calificaciones)
            {
                string cedula = cal.Estudiante?.Cedula ?? "-";
                string nombre = cal.Estudiante?.Nombre ?? "-";

                if (cal.Estado == EstadoCalificacion.Pendiente || cal.Nota == null)
                {
                    _writer.ShowMessage(string.Format(
                        "{0,-14} {1,-26} {2,-10} {3,-8} {4,-11} {5,-13} {6,-28}",
                        cedula, nombre, "-", "-", "-", "-", "Nota pendiente"));
                }
                else
                {
                    string textoConvocatoria = cal.NotaConvocatoria.HasValue
                        ? $"{cal.NotaConvocatoria} -> {cal.NotaFinalConvocatoria}"
                        : "-";

                    _writer.ShowMessage(string.Format(
                        "{0,-14} {1,-26} {2,-10} {3,-8} {4,-11} {5,-13} {6,-28}",
                        cedula,
                        nombre,
                        cal.Acumulado,
                        cal.Examen,
                        $"{cal.Acumulado}+{cal.Examen}={cal.Nota}",
                        textoConvocatoria,
                        FormatearEstado(cal.Estado)));
                }
            }

            int pendientesCount = resultado.Calificaciones.Count(c => c.Estado == EstadoCalificacion.Pendiente);

            _writer.ShowMessage(new string('-', 115));
            _writer.ShowMessage(
                $"RESUMEN DEL GRUPO -> Aprobados: {resultado.Aprobados} | " +
                $"Reprobados con derecho a convocatoria: {resultado.ReprobadosConConvocatoria} | " +
                $"Reprobados sin derecho: {resultado.ReprobadosSinDerecho} | " +
                $"Pendientes: {pendientesCount}");
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

