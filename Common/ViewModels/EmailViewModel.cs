using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class EmailViewModel
    {
        public string Cuerpo { get; set; }
        public string Asunto { get; set; }
        public string IdAplicacion { get; set; }
        public string IdTransaccion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Contrato { get; set; }
        public string NombreOrigen { get; set; }
        public string EmailOrigen { get; set; }
        public List<EmailDestinatarioViewModel> EmailsDestino { get; set; }
        public List<EmailDestinatarioViewModel> EmailsCopia { get; set; }
        public string TiempoEspera { get; set; }
    }
}
