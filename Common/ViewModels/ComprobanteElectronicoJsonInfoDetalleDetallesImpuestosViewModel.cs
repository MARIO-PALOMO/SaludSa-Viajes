using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonInfoDetalleDetallesImpuestosViewModel
    {
        public int Codigo { get; set; }
        public string CodigoNombre { get; set; }
        public int CodigoPorcentaje { get; set; }
        public string CodigoPorcentajeNombre { get; set; }
        public decimal Tarifa { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal Valor { get; set; }
    }
}
