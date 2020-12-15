using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class RespuestaRestViewModel
    {
        public string Estado { get; set; }
        public object Datos { get; set; }
        public List<string> Mensajes { get; set; }
    }
}
