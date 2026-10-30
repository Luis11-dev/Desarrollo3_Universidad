using System;
using System.Collections.Generic;
using System.Globalization;
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
            _writer.ShowHeader("REGISTRO DE ASISTENCIA A CLASE");

            try
            {
                List<Grupo> grupos = _profesorService.ObtenerGruposAsignados(_profesorActual);
                if (grupos.Count == 0)
                {
                    _writer.ShowMessage("\nNo tiene grupos asignados para marcar asistencia.");
                    _writer.WaitForKey();
                    return;
                }

                string[] opcionesGrupos = new string[grupos.Count + 1];
                for (int i = 0; i < grupos.Count; i++)
                {
                    var g = grupos[i];
                    opcionesGrupos[i] = $"{g.NombreMateria} (Grupo: {g.NombreGrupo} | {g.Dia} de {g.HoraInicio:hh\\:mm} a {g.HoraFin:hh\\:mm})";
                }
                opcionesGrupos[grupos.Count] = "Volver al menu anterior";

                _writer.DisplayMenu("Seleccione el grupo de la clase:", opcionesGrupos);

                string inputGrupo = new Validator("Ingrese su opcion: ")
                    .NotEmpty()
                    .OnlyDigits()
                    .InputBetweenXandY(1, grupos.Count + 1)
                    .ReadAndValidate();

                int indiceGrupo = int.Parse(inputGrupo) - 1;
                if (indiceGrupo == grupos.Count)
                {
                    return;
                }

                Grupo grupoSeleccionado = grupos[indiceGrupo];
                ProcesarAsistenciaGrupo(grupoSeleccionado);
            }
            catch (Exception ex)
            {
                _writer.ShowError(ex.Message);
                _writer.WaitForKey();
            }
        }

        private void ProcesarAsistenciaGrupo(Grupo grupo)
        {
            TimeSpan horaLimite = grupo.HoraInicio.Add(TimeSpan.FromMinutes(ProfesorService.MinutosToleranciaAsistencia));

            _writer.Clear();
            _writer.ShowHeader($"ASISTENCIA - {grupo.NombreMateria.ToUpper()} (GRUPO {grupo.NombreGrupo})");
            _writer.ShowMessage($"Dia de clase:              {grupo.Dia}");
            _writer.ShowMessage($"Hora de inicio:            {grupo.HoraInicio:hh\\:mm}");
            _writer.ShowMessage($"Hora limite (15 min):      {horaLimite:hh\\:mm}");
            _writer.ShowMessage($"Hora de finalizacion:      {grupo.HoraFin:hh\\:mm} ({grupo.ObtenerHorasPorSesion()} h por sesion)\n");

            _writer.DisplayMenu(
                "Seleccione la accion a realizar:",
                "Simular hora de entrada y marcar asistencia",
                "Registrar como ausencia (No asistio a la clase)",
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
                FlujoSimularHoraEntrada(grupo, horaLimite);
            }
            else if (accion == 2)
            {
                FlujoRegistrarAusencia(grupo);
            }
        }

        private void FlujoSimularHoraEntrada(Grupo grupo, TimeSpan horaLimite)
        {
            TimeSpan horaEnEspera = PedirHoraConExcepcion($"\nIngrese la hora de entrada (formato HH:mm, ej. {grupo.HoraInicio:hh\\:mm}): ");

            bool decisionTomada = false;
            while (!decisionTomada)
            {
                _writer.Clear();
                _writer.ShowHeader("CONFIRMACION DE MARCAJE DE ASISTENCIA");
                _writer.ShowMessage($"Grupo:                  {grupo.NombreGrupo} - {grupo.NombreMateria}");
                _writer.ShowMessage($"Horario oficial:        {grupo.HoraInicio:hh\\:mm} (Limite permitido: {horaLimite:hh\\:mm})");
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
                            _profesorService.MarcarAsistencia(_profesorActual, grupo.IdGrupo, DateTime.Today, horaEnEspera);
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
                        horaEnEspera = PedirHoraConExcepcion("\nIngrese la nueva hora de entrada (formato HH:mm): ");
                        break;

                    case 3:
                        _writer.ShowMessage("\nAccion anulada. No se registro la asistencia.");
                        decisionTomada = true;
                        _writer.WaitForKey();
                        break;
                }
            }
        }

        private void FlujoRegistrarAusencia(Grupo grupo)
        {
            int horasClase = grupo.ObtenerHorasPorSesion();
            decimal descuentoClase = horasClase * ProfesorService.TarifaPorHora;

            _writer.Clear();
            _writer.ShowHeader("CONFIRMACION DE REGISTRO DE AUSENCIA");
            _writer.ShowMessage($"Grupo:                  {grupo.NombreGrupo} - {grupo.NombreMateria}");
            _writer.ShowMessage($"Horas de la sesion:     {horasClase} hora(s)");
            _writer.ShowMessage($"Descuento en planilla:  {descuentoClase:N2} ({ProfesorService.TarifaPorHora} por hora)");
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
                    _profesorService.RegistrarAusencia(_profesorActual, grupo.IdGrupo, DateTime.Today);
                    _writer.ShowMessage("\nSe registro la ausencia a la clase.");
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

        // validador propio para leer la hora en formato HH:mm sin que crashee si escriben mal
        private TimeSpan PedirHoraConExcepcion(string mensaje)
        {
            string[] formatosValidos = { @"hh\:mm", @"h\:mm" };

            while (true)
            {
                try
                {
                    Console.Write(mensaje);
                    string entrada = (Console.ReadLine() ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(entrada))
                    {
                        throw new FormatException("Debe ingresar una hora en formato HH:mm (por ejemplo 07:35).");
                    }

                    if (!TimeSpan.TryParseExact(entrada, formatosValidos, CultureInfo.InvariantCulture, out TimeSpan hora) ||
                        hora.TotalHours < 0 || hora.TotalHours >= 24)
                    {
                        throw new FormatException("Formato de hora invalido. Use el formato de 24 horas HH:mm (por ejemplo 07:30 o 14:15).");
                    }

                    return hora;
                }
                catch (Exception ex)
                {
                    _writer.ShowError(ex.Message);
                }
            }
        }

        public void Undo()
        {
        }
    }
}

