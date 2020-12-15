using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class EmailRespuestaViewModel
    {
        public EmailRespuestaViewModel()
        {
            Mensajes = new List<string>();
        }

        public string IdRequerimiento { get; set; }
        public bool Enviado { get; set; }
        public List<string> Mensajes { get; set; }
    }
}
