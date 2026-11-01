using System;
using System.Collections.Generic;
using System.Linq;
using Universidad.Models;
using Universidad.Repositories;

namespace Universidad.Services
{
    public class PlanillaService
    {
        private readonly HorarioClaseRepository _horarioRepository;
        private readonly ProfesorRepository _profesorRepository;
        private readonly ContratoRepository _contratoRepository;
        public const decimal TarifaPorHoraDefault = 400m;
        public const decimal TarifaPorHora = TarifaPorHoraDefault;

        public PlanillaService(
            HorarioClaseRepository horarioRepository,
            ProfesorRepository profesorRepository,
            ContratoRepository contratoRepository)
        {
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
            _contratoRepository = contratoRepository ?? throw new ArgumentNullException(nameof(contratoRepository));
        }

        public PlanillaProfesor CalcularPlanillaProfesor(Profesor profesor)
        {
            if (profesor == null)
            {
                throw new ArgumentNullException(nameof(profesor));
            }

            Contrato? contrato = _contratoRepository.GetContratoActivo(profesor.Cedula);

            decimal tarifa;
            List<HorarioClase> materias;
            int horasContratadas;

            if (contrato != null)
            {
                tarifa = contrato.TarifaPorHora;
                materias = contrato.MateriasAsignadas.Count > 0
                    ? contrato.MateriasAsignadas.ToList()
                    : _horarioRepository.GetByProfesor(profesor.Cedula);
                horasContratadas = materias.Sum(m => m.HorasMensuales);
            }
            else
            {
                tarifa = TarifaPorHoraDefault;
                materias = _horarioRepository.GetByProfesor(profesor.Cedula);
                horasContratadas = materias.Sum(m => m.HorasMensuales);
            }

            List<AsistenciaProfesor> ausencias = _profesorRepository
                .GetAsistenciasByProfesor(profesor.Cedula)
                .Where(a => !a.Asistio)
                .ToList();

            int horasDescontadas = ausencias.Sum(a => a.Horario != null ? a.Horario.HorasPorSesion : 0);
            int horasEfectivas = Math.Max(0, horasContratadas - horasDescontadas);

            decimal salarioBruto = horasContratadas * tarifa;
            decimal descuento = horasDescontadas * tarifa;
            decimal salarioNeto = Math.Max(0m, salarioBruto - descuento);

            return new PlanillaProfesor
            {
                Profesor = profesor,
                Contrato = contrato,
                MateriasAsignadas = materias,
                HorasContratadasMes = horasContratadas,
                CantidadAusencias = ausencias.Count,
                HorasDescontadas = horasDescontadas,
                HorasEfectivasTrabajadas = horasEfectivas,
                TarifaPorHora = tarifa,
                SalarioBruto = salarioBruto,
                DescuentoTotal = descuento,
                SalarioNeto = salarioNeto
            };
        }

        public List<PlanillaProfesor> GenerarPlanillaGeneral()
        {
            return _profesorRepository.GetAll()
                .Select(p => CalcularPlanillaProfesor(p))
                .ToList();
        }
    }
}
