using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class FacturaDetallePagoViewModel
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Valor { get; set; }
        public decimal Subtotal { get; set; }

        public string GrupoImpuestoCodigo { get; set; }
        public string GrupoImpuestoDescripcion { get; set; }

        public string GrupoImpuestoArticuloCodigo { get; set; }
        public string GrupoImpuestoArticuloDescripcion { get; set; }
        public string GrupoImpuestoArticuloCodigoDescripcion { get; set; }

        public string SustentoTributarioCodigo { get; set; }
        public string SustentoTributarioDescripcion { get; set; }
        public string SustentoTributarioCodigoDescripcion { get; set; }

        public string ImpuestoRentaGrupoImpuestoArticuloCodigo { get; set; }
        public string ImpuestoRentaGrupoImpuestoArticuloDescripcion { get; set; }
        public string ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion { get; set; }

        public string IvaGrupoImpuestoArticuloCodigo { get; set; }
        public string IvaGrupoImpuestoArticuloDescripcion { get; set; }
        public string IvaGrupoImpuestoArticuloCodigoDescripcion { get; set; }

        public long ImpuestoPagoId { get; set; }
        public ImpuestoPagoViewModel ImpuestoPagoObj { get; set; }

        public List<PlantillasDistribucionDetalleViewModel> PlantillaDistribucionDetalle { get; set; }
    }
}
