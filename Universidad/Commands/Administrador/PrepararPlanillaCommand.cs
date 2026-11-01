using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Administrador
{
    public class PrepararPlanillaCommand : ICommand
    {
        private readonly PlanillaService _planillaService;
        private readonly IConsoleWriter _writer;

        public PrepararPlanillaCommand(PlanillaService planillaService, IConsoleWriter writer)
        {
            _planillaService = planillaService ?? throw new ArgumentNullException(nameof(planillaService));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("PLANILLA GENERAL DE PAGO A PROFESORES");

            try
            {
                List<PlanillaProfesor> planillas = _planillaService.GenerarPlanillaGeneral();

                if (planillas.Count == 0)
                {
                    _writer.ShowMessage("\nNo hay profesores registrados en el sistema para liquidar planilla.\n");
                    _writer.WaitForKey();
                    return;
                }

                _writer.ShowMessage($"Tarifa oficial por hora: C$ {PlanillaService.TarifaPorHora:N2}\n");

                foreach (PlanillaProfesor p in planillas)
                {
                    _writer.ShowMessage("--------------------------------------------------------------------------------");
                    _writer.ShowMessage($"Profesor: {p.Profesor.Nombre} | Cedula: {p.Profesor.Cedula} | Profesion: {p.Profesor.Profesion}");
                    _writer.ShowMessage($"Clases Asignadas: {p.HorariosAsignados.Count}");

                    if (p.HorariosAsignados.Count > 0)
                    {
                        foreach (HorarioClase h in p.HorariosAsignados)
                        {
                            _writer.ShowMessage($"  • [{h.Materia.Codigo}] {h.Materia.Nombre} | Grupo: {h.Grupo.NombreGrupo} | {h.Dia} {h.HoraInicio:hh\\:mm}-{h.HoraFin:hh\\:mm} ({h.Modalidad}) -> {h.HorasMensuales}h/mes");
                        }
                    }
                    else
                    {
                        _writer.ShowMessage("  (Sin clases asignadas este periodo)");
                    }

                    _writer.ShowMessage($"Horas Contratadas al Mes: {p.HorasContratadasMes} hrs");
                    _writer.ShowMessage($"Inasistencias Registradas: {p.CantidadAusencias} (Horas descontadas: {p.HorasDescontadas} hrs)");
                    _writer.ShowMessage($"Horas Efectivas Trabajadas: {p.HorasEfectivasTrabajadas} hrs");
                    _writer.ShowMessage($"Salario Bruto:             C$ {p.SalarioBruto,10:N2}");
                    _writer.ShowMessage($"Deducciones por Ausencias: C$ {p.DescuentoTotal,10:N2}");
                    _writer.ShowMessage($"SALARIO NETO A PAGAR:      C$ {p.SalarioNeto,10:N2}");
                }

                _writer.ShowMessage("================================================================================");
                decimal totalBruto = planillas.Sum(p => p.SalarioBruto);
                decimal totalDescuentos = planillas.Sum(p => p.DescuentoTotal);
                decimal totalNeto = planillas.Sum(p => p.SalarioNeto);
                int totalHoras = planillas.Sum(p => p.HorasEfectivasTrabajadas);

                _writer.ShowMessage($"TOTAL GENERAL DE HORAS TRABAJADAS: {totalHoras} hrs");
                _writer.ShowMessage($"TOTAL SALARIOS BRUTOS:             C$ {totalBruto,10:N2}");
                _writer.ShowMessage($"TOTAL DEDUCCIONES:                 C$ {totalDescuentos,10:N2}");
                _writer.ShowMessage($"TOTAL NETO A DESEMBOLSAR:          C$ {totalNeto,10:N2}");
                _writer.ShowMessage("================================================================================\n");

                _writer.WaitForKey();
            }
            catch (Exception ex)
            {
                _writer.ShowError($"\nError al preparar la planilla: {ex.Message}");
                _writer.WaitForKey();
            }
        }

        public void Undo()
        {
            throw new NotSupportedException("Deshacer no soportado para la preparacion de planilla.");
        }
    }
}

