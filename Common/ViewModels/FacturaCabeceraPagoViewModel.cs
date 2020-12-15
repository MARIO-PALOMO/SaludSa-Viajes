using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class FacturaCabeceraPagoViewModel
    {
        public long Id { get; set; }
        public string NoFactura { get; set; }
        public string NoAutorizacion { get; set; }
        public string NoLiquidacion { get; set; }
        public string Concepto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Total { get; set; }
        public string Tipo { get; set; }

        public string AprobacionJefeArea { get; set; }
        public string AprobacionSubgerenteArea { get; set; }
        public string AprobacionGerenteArea { get; set; }
        public string AprobacionVicePresidenteFinanciero { get; set; }
        public string AprobacionGerenteGeneral { get; set; }

        public long TipoPagoId { get; set; }
        public TipoPagoViewModel TipoPagoObj { get; set; }

        public long? ComprobanteElectronicoId { get; set; }
        public ComprobanteElectronicoViewModel FacturaElectronica { get; set; }

        public List<FacturaDetallePagoViewModel> FacturaDetallesPago { get; set; }
    }
}
