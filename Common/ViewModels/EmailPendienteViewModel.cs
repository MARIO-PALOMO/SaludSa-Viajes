using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class EmailPendienteViewModel
    {
        public long Id { get; set; }
        public string Cuerpo { get; set; }
        public string Asunto { get; set; }
        public string Razon { get; set; }
        public DateTime FechaRegistro { get; set; }

        public TareaViewModel Tarea { get; set; }

        public List<EmailDestinatarioViewModel> EmailsDestino { get; set; }
        public List<EmailDestinatarioViewModel> EmailsCopia { get; set; }
    }
}
