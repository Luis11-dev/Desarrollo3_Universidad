using System;
using System.Collections.Generic;
using Universidad.Models;
using Universidad.Services;
using Universidad.Util;

namespace Universidad.Commands.Profesor
{
    public class MarcarAsistenciaCommand : ICommand
    {
        private readonly ProfesorService _profesorService;
        private readonly IConsoleWriter _writer;
        private readonly Models.Profesor _profesorActual;

        public MarcarAsistenciaCommand(ProfesorService profesorService, IConsoleWriter writer, Models.Profesor profesorActual)
        {
            _profesorService = profesorService;
            _writer = writer;
            _profesorActual = profesorActual;
        }

        public void Execute()
        {
            _writer.Clear();
            _writer.ShowHeader("REGISTRO DE ASISTENCIA A MATERIA");

            try
            {
                List<HorarioClase> materias = _profesorService.ObtenerMateriasAsignadas(_profesorActual);
                if (materias.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene materias asignadas para marcar asistencia.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesMaterias = new string[materias.Count + 1];
                for (int i = 0; i < materias.Count; i++)
                {
                    var m = materias[i];
                    opcionesMaterias[i] = $"{m.Materia.Nombre} (Grupo: {m.Grupo.NombreGrupo} | {m.Dia} de {m.HoraInicio:hh\\:mm} a {m.HoraFin:hh\\:mm})";
                }
                opcionesMaterias[materias.Count] = "Volver al menu anterior";

                _writer.DisplayMenu("Seleccione la materia para gestionar asistencia:", opcionesMaterias);

                string inputMateria = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, materias.Count + 1)
                    .ReadAndValidate();

                int indiceMateria = int.Parse(inputMateria) - 1;
                if (indiceMateria == materias.Count)
                {
                    return;
                }

                HorarioClase materiaSeleccionada = materias[indiceMateria];
                ProcesarAsistenciaMateria(materiaSeleccionada);
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
                _writer.WaitForKey();
            }
        }

        private void ProcesarAsistenciaMateria(HorarioClase horario)
        {
            TimeSpan horaLimite = horario.HoraInicio.Add(TimeSpan.FromMinutes(ProfesorService.MinutosToleranciaAsistencia));

            _writer.Clear();
            _writer.ShowHeader($"ASISTENCIA - {horario.Materia.Nombre.ToUpper()} (GRUPO {horario.Grupo.NombreGrupo})");
            _writer.ShowMessage($"Dia de materia:            {horario.Dia}");
            _writer.ShowMessage($"Hora de inicio:            {horario.HoraInicio:hh\\:mm}");
            _writer.ShowMessage($"Hora limite (15 min):      {horaLimite:hh\\:mm}");
            _writer.ShowMessage($"Hora de finalizacion:      {horario.HoraFin:hh\\:mm} ({horario.HorasPorSesion} h por sesion)\n");

            _writer.DisplayMenu(
                "Seleccione la accion a realizar:",
                "Simular hora de entrada y marcar asistencia",
                "Registrar como ausencia (No asistio a la materia)",
                "Cancelar y volver"
            );

            string opcionAccion = new Validator("Ingrese su opcion: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 3)
                .ReadAndValidate();

            int accion = int.Parse(opcionAccion);
            if (accion == 3)
            {
                return;
            }

            if (accion == 1)
            {
                FlujoSimularHoraEntrada(horario, horaLimite);
            }
            else if (accion == 2)
            {
                FlujoRegistrarAusencia(horario);
            }
        }

        private void FlujoSimularHoraEntrada(HorarioClase horario, TimeSpan horaLimite)
        {
            string inputHora = new ValidatorProfesor($"\nIngrese la hora de entrada (formato HH:mm, ej. {horario.HoraInicio:hh\\:mm}): ")
                .NotEmpty()
                .FormatoHora24h()
                .ReadAndValidate();

            TimeSpan horaEnEspera = TimeSpan.Parse(inputHora);

            bool decisionTomada = false;
            while (!decisionTomada)
            {
                _writer.Clear();
                _writer.ShowHeader("CONFIRMACION DE MARCAJE DE ASISTENCIA");
                _writer.ShowMessage($"Materia:                {horario.Materia.Nombre} (Grupo: {horario.Grupo.NombreGrupo})");
                _writer.ShowMessage($"Horario oficial:        {horario.HoraInicio:hh\\:mm} (Limite permitido: {horaLimite:hh\\:mm})");
                _writer.ShowMessage($"Hora de entrada:        {horaEnEspera:hh\\:mm}");
                _writer.ShowMessage(new string('-', 60));

                _writer.DisplayMenu(
                    "¿Que desea hacer con este registro?",
                    "Confirmar y guardar asistencia",
                    "Editar campo: Hora de entrada",
                    "Anular accion"
                );

                string inputConfirm = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, 3)
                    .ReadAndValidate();

                switch (int.Parse(inputConfirm))
                {
                    case 1:
                        try
                        {
                            _profesorService.MarcarAsistencia(_profesorActual, horario.IdHorario, DateTime.Today, horaEnEspera);
                            _writer.ShowMessage($"\nAsistencia registrada correctamente a las {horaEnEspera:hh\\:mm}.");
                        }
                        catch (Exception ex)
                        {
                            _writer.ShowError(ex.Message);
                        }
                        decisionTomada = true;
                        _writer.WaitForKey();
                        break;

                    case 2:
                        string nuevaHora = new ValidatorProfesor("\nIngrese la nueva hora de entrada (formato HH:mm): ")
                            .NotEmpty()
                            .FormatoHora24h()
                            .ReadAndValidate();
                        horaEnEspera = TimeSpan.Parse(nuevaHora);
                        break;

                    case 3:
                        _writer.ShowMessage("\nAccion anulada. No se registro la asistencia.");
                        decisionTomada = true;
                        _writer.WaitForKey();
                        break;
                }
            }
        }

        private void FlujoRegistrarAusencia(HorarioClase horario)
        {
            int horasSesion = horario.HorasPorSesion;
            decimal tarifaHora = _profesorService.ObtenerContratoVigente(_profesorActual)?.TarifaPorHora 
                ?? ProfesorService.TarifaPorHoraDefault;
            decimal descuentoMateria = horasSesion * tarifaHora;

            _writer.Clear();
            _writer.ShowHeader("CONFIRMACION DE REGISTRO DE AUSENCIA");
            _writer.ShowMessage($"Materia:                {horario.Materia.Nombre} (Grupo: {horario.Grupo.NombreGrupo})");
            _writer.ShowMessage($"Horas de la sesion:     {horasSesion} hora(s)");
            _writer.ShowMessage($"Descuento estimado:     C$ {descuentoMateria:N2} (a C$ {tarifaHora:N2}/h)");
            _writer.ShowMessage(new string('-', 60));

            _writer.DisplayMenu(
                "¿Desea confirmar el registro de ausencia?",
                "Confirmar y registrar ausencia",
                "Anular accion"
            );

            string inputConfirm = new Validator("Ingrese su opcion: ")
                .NotEmpty()
                .OnlyDigits()
                .InputBetweenXandY(1, 2)
                .ReadAndValidate();

            if (int.Parse(inputConfirm) == 1)
            {
                try
                {
                    _profesorService.RegistrarAusencia(_profesorActual, horario.IdHorario, DateTime.Today);
                    _writer.ShowMessage("\nSe registro la ausencia a la materia.");
                }
                catch (Exception ex)
                {
                    _writer.ShowError(ex.Message);
                }
            }
            else
            {
                _writer.ShowMessage("\nAccion anulada.");
            }

            _writer.WaitForKey();
        }

        public void Undo()
        {
        }
    }
}
