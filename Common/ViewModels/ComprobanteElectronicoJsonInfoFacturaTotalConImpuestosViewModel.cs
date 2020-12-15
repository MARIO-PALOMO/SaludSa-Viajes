using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonInfoFacturaTotalConImpuestosViewModel
    {
        public int Codigo { get; set; }
        public int CodigoPorcentaje { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal Valor { get; set; }
    }
}
