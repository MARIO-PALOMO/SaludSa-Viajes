using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoDocumentoViewModel
    {
        public string ruc { get; set; }
        public string establecimiento { get; set; }
        public string puntoEmision { get; set; }
        public string secuencial { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }

        public string tipoDocumento { get; set; }
        public int estadoDocumento { get; set; }
        public int codigoSistema { get; set; }
    }
}
