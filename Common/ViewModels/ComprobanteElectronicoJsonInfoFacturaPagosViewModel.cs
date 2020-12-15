using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonInfoFacturaPagosViewModel
    {
        public string FormaPago { get; set; }
        public decimal Total { get; set; }
        public decimal Plazo { get; set; }
        public string UnidadTiempo { get; set; }
    }
}
