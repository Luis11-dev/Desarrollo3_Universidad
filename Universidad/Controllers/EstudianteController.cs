using Universidad.Models;
using Universidad.Util;

namespace Universidad.Controllers
{
    public class EstudianteController : IController
    {
        private readonly Estudiante _estudiante;
        private readonly IConsoleWriter _writer;

        public EstudianteController(Estudiante estudiante, IConsoleWriter writer)
        {
            _estudiante = estudiante;
            _writer = writer;
        }

        public void DisplayMenu()
        {
            _writer.Clear();
            _writer.DisplayInitialMessage("MODULO DE ESTUDIANTES");
            _writer.ShowMessage($"Bienvenido(a): {_estudiante.Nombre} | Carnet: {_estudiante.Cedula}\n");
            _writer.ShowMessage("El modulo de presentacion para estudiantes se encuentra en construccion.\n");
            _writer.WaitForKey();
        }
    }
}

