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
            _writer.ShowHeader("CONTRATO LABORAL Y PLANILLA DE PAGO");

            try
            {
                PlanillaProfesor planilla = _profesorService.ConsultarPlanilla(_profesorActual);
                Contrato? contrato = planilla.Contrato ?? _profesorService.ObtenerContratoVigente(_profesorActual);

                _writer.ShowMessage($"Profesor: {_profesorActual.Nombre} | Cedula: {_profesorActual.Cedula} | Profesion: {_profesorActual.Profesion}");

                if (contrato != null)
                {
                    _writer.ShowMessage($"Contrato N°: {contrato.IdContrato} | Periodo: {contrato.PeriodoAcademico} | Estado: {(contrato.Activo ? "Activo" : "Inactivo")}");
                    _writer.ShowMessage($"Tarifa por hora pactada: C$ {contrato.TarifaPorHora:N2}\n");
                }
                else
                {
                    _writer.ShowMessage($"Tarifa por hora estandar: C$ {planilla.TarifaPorHora:N2}\n");
                }

                if (planilla.MateriasAsignadas.Count == 0)
                {
                    _writer.ShowMessage("No tiene materias asignadas ni contratos activos en este momento.");
                    _writer.WaitForKey();
                    return;
                }

                _writer.ShowMessage("--- 1. DETALLE DE MATERIAS ASIGNADAS EN CONTRATO ---");
                _writer.ShowMessage(string.Format(
                    "{0,-10} {1,-26} {2,-12} {3,-10} {4,-22} {5,-14}",
                    "GRUPO", "MATERIA", "MODALIDAD", "HORAS/MES", "HORARIO", "SALARIO BASE"));
                _writer.ShowMessage(new string('-', 98));

                foreach (var m in planilla.MateriasAsignadas)
                {
                    string horario = $"{m.Dia} {m.HoraInicio:hh\\:mm}-{m.HoraFin:hh\\:mm}";
                    decimal salarioMateria = m.HorasMensuales * planilla.TarifaPorHora;
                    _writer.ShowMessage(string.Format(
                        "{0,-10} {1,-26} {2,-12} {3,-10} {4,-22} {5,-14:N2}",
                        m.Grupo.NombreGrupo,
                        m.Materia.Nombre,
                        m.Modalidad,
                        $"{m.HorasMensuales} h",
                        horario,
                        salarioMateria));
                }

                _writer.ShowMessage(new string('-', 98));
                _writer.ShowMessage($"Total Horas Contratadas:          {planilla.HorasContratadasMes} hrs/mes");
                _writer.ShowMessage($"Salario Mensual Pactado:          C$ {planilla.SalarioBruto:N2}");

                _writer.ShowMessage("\n--- 2. LIQUIDACION DE PLANILLA DEL PERIODO ---");
                _writer.ShowMessage($"Salario Base Pactado:             C$ {planilla.SalarioBruto:N2}");
                _writer.ShowMessage($"Ausencias a materias registradas: {planilla.CantidadAusencias} sesion(es)");
                _writer.ShowMessage($"Horas descontadas por inasistencia: {planilla.HorasDescontadas} h (a C$ {planilla.TarifaPorHora:N2}/h)");
                _writer.ShowMessage($"Horas Efectivas Trabajadas:       {planilla.HorasEfectivasTrabajadas} hrs");
                _writer.ShowMessage($"Total Descuentos Aplicados:      -C$ {planilla.DescuentoTotal:N2}");
                _writer.ShowMessage(new string('-', 50));
                _writer.ShowMessage($"SALARIO NETO A PERCIBIR:          C$ {planilla.SalarioNeto:N2}");
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
