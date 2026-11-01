using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Universidad.Models;

namespace Universidad.Util
{
    public class EstudianteDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string CorreoElectronico { get; set; } = string.Empty;
        public int NumeroCelular { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public Genero Genero { get; set; }
        public string Carrera { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? IdGrupo { get; set; }
    }

    public class ProfesorDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int NumeroCelular { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public Genero Genero { get; set; }
        public string EstadoCivil { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public string? Especialidad { get; set; }
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class HorarioClaseDto
    {
        public string IdHorario { get; set; } = string.Empty;
        public string IdGrupo { get; set; } = string.Empty;
        public string CodigoMateria { get; set; } = string.Empty;
        public string CedulaProfesor { get; set; } = string.Empty;
        public string Dia { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public Modalidad Modalidad { get; set; }
        public List<string> CedulasEstudiantesInscritos { get; set; } = new();
    }

    public class InscripcionDto
    {
        public string IdInscripcion { get; set; } = string.Empty;
        public string CedulaEstudiante { get; set; } = string.Empty;
        public string IdHorario { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaInscripcion { get; set; }
    }

    public static class FileManager
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private static string GetStorageDirectory()
        {
            string current = Directory.GetCurrentDirectory();

            if (Directory.Exists(Path.Combine(current, "Universidad")))
            {
                string target = Path.Combine(current, "Universidad", "DataFiles");
                Directory.CreateDirectory(target);
                return target;
            }

            if (File.Exists(Path.Combine(current, "Universidad.csproj")))
            {
                string target = Path.Combine(current, "DataFiles");
                Directory.CreateDirectory(target);
                return target;
            }

            string fallback = Path.Combine(AppContext.BaseDirectory, "DataFiles");
            Directory.CreateDirectory(fallback);
            return fallback;
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(GetStorageDirectory(), fileName);
        }

        // ==================== ESTUDIANTES ====================
        public static void GuardarEstudiantes(List<Estudiante> estudiantes, List<Grupo>? grupos = null)
        {
            try
            {
                List<EstudianteDto> dtos = estudiantes.Select(e =>
                {
                    string? idGrupo = grupos?.FirstOrDefault(g => g.Estudiantes.Any(est => est.Cedula.Equals(e.Cedula, StringComparison.OrdinalIgnoreCase)))?.IdGrupo;
                    return new EstudianteDto
                    {
                        Cedula = e.Cedula,
                        Nombre = e.Nombre,
                        FechaNacimiento = e.FechaNacimiento,
                        CorreoElectronico = e.CorreoElectronico,
                        NumeroCelular = e.NumeroCelular,
                        Direccion = e.Direccion,
                        Genero = e.Genero,
                        Carrera = e.Carrera,
                        Activo = e.Activo,
                        User = e.User,
                        Password = e.Password,
                        IdGrupo = idGrupo
                    };
                }).ToList();

                string json = JsonSerializer.Serialize(dtos, _jsonOptions);
                File.WriteAllText(GetFilePath("estudiantes.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al guardar estudiantes: {ex.Message}");
            }
        }

        public static List<EstudianteDto> CargarEstudiantesDto()
        {
            try
            {
                string path = GetFilePath("estudiantes.json");
                if (!File.Exists(path))
                {
                    return new List<EstudianteDto>();
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<EstudianteDto>();
                }

                return JsonSerializer.Deserialize<List<EstudianteDto>>(json, _jsonOptions) ?? new List<EstudianteDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al cargar estudiantes: {ex.Message}");
                return new List<EstudianteDto>();
            }
        }

        // ==================== PROFESORES ====================
        public static void GuardarProfesores(List<Profesor> profesores)
        {
            try
            {
                List<ProfesorDto> dtos = profesores.Select(p => new ProfesorDto
                {
                    Cedula = p.Cedula,
                    Nombre = p.Nombre,
                    CorreoElectronico = p.CorreoElectronico,
                    NumeroCelular = p.NumeroCelular,
                    Direccion = p.Direccion,
                    Genero = p.Genero,
                    EstadoCivil = p.EstadoCivil,
                    Profesion = p.Profesion,
                    Especialidad = p.Especialidad,
                    User = p.User,
                    Password = p.Password
                }).ToList();

                string json = JsonSerializer.Serialize(dtos, _jsonOptions);
                File.WriteAllText(GetFilePath("profesores.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al guardar profesores: {ex.Message}");
            }
        }

        public static List<ProfesorDto> CargarProfesoresDto()
        {
            try
            {
                string path = GetFilePath("profesores.json");
                if (!File.Exists(path))
                {
                    return new List<ProfesorDto>();
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<ProfesorDto>();
                }

                return JsonSerializer.Deserialize<List<ProfesorDto>>(json, _jsonOptions) ?? new List<ProfesorDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al cargar profesores: {ex.Message}");
                return new List<ProfesorDto>();
            }
        }

        // ==================== HORARIOS DE CLASE ====================
        public static void GuardarHorarios(List<HorarioClase> horarios)
        {
            try
            {
                List<HorarioClaseDto> dtos = horarios.Select(h => new HorarioClaseDto
                {
                    IdHorario = h.IdHorario,
                    IdGrupo = h.Grupo?.IdGrupo ?? string.Empty,
                    CodigoMateria = h.Materia?.Codigo ?? string.Empty,
                    CedulaProfesor = h.Profesor?.Cedula ?? string.Empty,
                    Dia = h.Dia,
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin,
                    Modalidad = h.Modalidad,
                    CedulasEstudiantesInscritos = h.EstudiantesInscritos.Select(e => e.Cedula).ToList()
                }).ToList();

                string json = JsonSerializer.Serialize(dtos, _jsonOptions);
                File.WriteAllText(GetFilePath("horarios.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al guardar horarios: {ex.Message}");
            }
        }

        public static List<HorarioClaseDto> CargarHorariosDto()
        {
            try
            {
                string path = GetFilePath("horarios.json");
                if (!File.Exists(path))
                {
                    return new List<HorarioClaseDto>();
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<HorarioClaseDto>();
                }

                return JsonSerializer.Deserialize<List<HorarioClaseDto>>(json, _jsonOptions) ?? new List<HorarioClaseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al cargar horarios: {ex.Message}");
                return new List<HorarioClaseDto>();
            }
        }

        // ==================== INSCRIPCIONES ====================
        public static void GuardarInscripciones(List<Inscripcion> inscripciones)
        {
            try
            {
                List<InscripcionDto> dtos = inscripciones.Select(i => new InscripcionDto
                {
                    IdInscripcion = i.IdInscripcion,
                    CedulaEstudiante = i.Estudiante?.Cedula ?? string.Empty,
                    IdHorario = i.Horario?.IdHorario ?? string.Empty,
                    PeriodoAcademico = i.PeriodoAcademico,
                    FechaInscripcion = i.FechaInscripcion
                }).ToList();

                string json = JsonSerializer.Serialize(dtos, _jsonOptions);
                File.WriteAllText(GetFilePath("inscripciones.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al guardar inscripciones: {ex.Message}");
            }
        }

        public static List<InscripcionDto> CargarInscripcionesDto()
        {
            try
            {
                string path = GetFilePath("inscripciones.json");
                if (!File.Exists(path))
                {
                    return new List<InscripcionDto>();
                }

                string json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<InscripcionDto>();
                }

                return JsonSerializer.Deserialize<List<InscripcionDto>>(json, _jsonOptions) ?? new List<InscripcionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileManager] Error al cargar inscripciones: {ex.Message}");
                return new List<InscripcionDto>();
            }
        }
    }
}

