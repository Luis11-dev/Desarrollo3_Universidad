using System;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor
{
    public class ConsultarContratoPlanillaCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;

        public ConsultarContratoPlanillaCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("CONTRATO Y PLANILLA DE PAGO DEL PROFESOR");

            try
            {
                var datos = _profesorService.ConsultarContratoYPlanilla(_profesorActual);

                _writer.ShowMessage($"Profesor: {_profesorActual.Nombre} | Cedula: {_profesorActual.Cedula} | Profesion: {_profesorActual.Profesion}");
                _writer.ShowMessage($"Tarifa por hora contratada: {ProfesorService.TarifaPorHora:N2}\n");

                if (datos.GruposContratados.Count == 0)
                {
                    _writer.ShowMessage("No tiene grupos asignados ni contratos activos en este momento.");
                    _writer.WaitForKey();
                    return;
                }

                // 1. Detalle del contrato por cada clase asignada
                _writer.ShowMessage("--- 1. DETALLE DE CONTRATO POR GRUPO ASIGNADO ---");
                _writer.ShowMessage(string.Format(
                    "{0,-10} {1,-26} {2,-12} {3,-10} {4,-22} {5,-14}",
                    "GRUPO", "MATERIA", "MODALIDAD", "HORAS", "HORARIO", "SALARIO GRUPO"));
                _writer.ShowMessage(new string('-', 98));

                foreach (var g in datos.GruposContratados)
                {
                    string horario = $"{g.Dia} {g.HoraInicio:hh\\:mm}-{g.HoraFin:hh\\:mm}";
                    _writer.ShowMessage(string.Format(
                        "{0,-10} {1,-26} {2,-12} {3,-10} {4,-22} {5,-14:N2}",
                        g.NombreGrupo,
                        g.NombreMateria,
                        g.Modalidad,
                        $"{g.ObtenerHorasContrato()} h",
                        horario,
                        g.ObtenerSalarioContrato()));
                }

                _writer.ShowMessage(new string('-', 98));

                // 2. Resumen de planilla de pago con descuentos por inasistencia
                _writer.ShowMessage("\n--- 2. RESUMEN DE PLANILLA DE PAGO ---");
                _writer.ShowMessage($"Salario Base Total:                {datos.SalarioBaseTotal:N2}");
                _writer.ShowMessage($"Ausencias registradas:             {datos.CantidadAusencias} clase(s)");
                _writer.ShowMessage($"Horas descontadas por ausencias:   {datos.HorasDescontadas} h (a {ProfesorService.TarifaPorHora:N2} por hora)");
                _writer.ShowMessage($"Total Descuentos Aplicados:       -{datos.DescuentoTotal:N2}");
                _writer.ShowMessage(new string('-', 50));
                _writer.ShowMessage($"SALARIO NETO EN PLANILLA:          {datos.SalarioNeto:N2}");
                _writer.ShowMessage(new string('-', 50));
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

