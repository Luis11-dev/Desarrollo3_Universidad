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
        public const decimal TarifaPorHora = 400m;

        public PlanillaService(HorarioClaseRepository horarioRepository, ProfesorRepository profesorRepository)
        {
            _horarioRepository = horarioRepository ?? throw new ArgumentNullException(nameof(horarioRepository));
            _profesorRepository = profesorRepository ?? throw new ArgumentNullException(nameof(profesorRepository));
        }

        public PlanillaProfesor CalcularPlanillaProfesor(Profesor profesor)
        {
            if (profesor == null)
            {
                throw new ArgumentNullException(nameof(profesor));
            }

            List<HorarioClase> horarios = _horarioRepository.GetByProfesor(profesor.Cedula);
            int horasContratadas = horarios.Sum(h => h.HorasMensuales);

            List<AsistenciaProfesor> ausencias = _profesorRepository
                .GetAsistenciasByProfesor(profesor.Cedula)
                .Where(a => !a.Asistio)
                .ToList();

            int horasDescontadas = ausencias.Sum(a => a.Grupo.ObtenerHorasPorSesion());
            int horasEfectivas = Math.Max(0, horasContratadas - horasDescontadas);

            decimal salarioBruto = horasContratadas * TarifaPorHora;
            decimal descuento = horasDescontadas * TarifaPorHora;
            decimal salarioNeto = Math.Max(0m, salarioBruto - descuento);

            return new PlanillaProfesor
            {
                Profesor = profesor,
                HorariosAsignados = horarios,
                HorasContratadasMes = horasContratadas,
                CantidadAusencias = ausencias.Count,
                HorasDescontadas = horasDescontadas,
                HorasEfectivasTrabajadas = horasEfectivas,
                TarifaPorHora = TarifaPorHora,
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

