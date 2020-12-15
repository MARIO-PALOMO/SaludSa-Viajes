using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args != null && args.Length > 0)
            {
                string direccionAbsoluta = args[0] + "ServicioEscalamientoAutomaticoTareas.exe";

                System.Diagnostics.Process newProc;
                newProc = System.Diagnostics.Process.Start(direccionAbsoluta);
            }
        }
    }
}
