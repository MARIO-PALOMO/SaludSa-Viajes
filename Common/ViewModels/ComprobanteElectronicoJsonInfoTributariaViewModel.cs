using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonInfoTributariaViewModel
    {
        public int Ambiente { get; set;  }
        public string AmbienteNombre { get; set; }
        public int TipoEmision { get; set; }
        public string TipoEmisionNombre { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string Ruc { get; set; }
        public string ClaveAcceso { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDocumentoNombre { get; set; }
        public string Establecimiento { get; set; }
        public string PuntoEmision { get; set; }
        public string Secuencial { get; set; }
        public string NumeroDocumento { get; set; }
        public string DirccionMatriz { get; set; }
    }
}
