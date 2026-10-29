using Universidad.Data;
using Universidad.Models;
using System.Collections.Generic;

namespace Universidad.Util
{
    public static class Startup
    {
        public static IConsoleWriter InitConsoleWriter()
        {
            return new ConsoleWriter();
        }
    }
}
