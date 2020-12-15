using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoJsonViewModel
    {
        public ComprobanteElectronicoJsonInfoTributariaViewModel InfoTributaria { get; set; }
        public ComprobanteElectronicoJsonInfoFacturaViewModel InfoFactura { get; set; }
        public ComprobanteElectronicoJsonInfoDetalleViewModel InfoDetalle { get; set; }
        public ComprobanteElectronicoJsonInfoAdicionalViewModel InfoAdicional { get; set; }
    }
}
