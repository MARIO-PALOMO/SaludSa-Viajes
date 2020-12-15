using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonInfoFacturaViewModel
    {
        public DateTime FechaEmision { get; set; }
        public string DireccionEstablecimiento { get; set; }
        public string ContribuyenteEspecial { get; set; }
        public bool ObligadoContabilidad { get; set; }
        public string TipoIdentificacionComprador { get; set; }
        public string IdentificacionCompradorNombre { get; set; }
        public string RazonSocialComprador { get; set; }
        public string IdentificacionComprador { get; set; }
        public decimal TotalSinImpuestos { get; set; }
        public decimal TotalDescuento { get; set; }
        public List<ComprobanteElectronicoJsonInfoFacturaTotalConImpuestosViewModel> TotalConImpuestos { get; set; }
        public decimal Propina { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Moneda { get; set; }
        public List<ComprobanteElectronicoJsonInfoFacturaPagosViewModel> Pagos { get; set; }
    }
}
