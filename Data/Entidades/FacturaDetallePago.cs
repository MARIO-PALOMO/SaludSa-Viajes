using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entidades
{
    public class FacturaDetallePago
    {
        public FacturaDetallePago()
        {
            Distribuciones = new HashSet<FacturaDetallePagoDistribucion>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Descripcion { get; set; }
        [Required]
        public decimal Valor { get; set; }
        [Required]
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

        [Required]
        [ForeignKey("ImpuestoPago")]
        public long ImpuestoPagoId { get; set; }
        public virtual ImpuestoPago ImpuestoPago { get; set; }

        [Required]
        [ForeignKey("FacturaCabeceraPago")]
        public long FacturaCabeceraPagoId { get; set; }
        public virtual FacturaCabeceraPago FacturaCabeceraPago { get; set; }

        public virtual ICollection<FacturaDetallePagoDistribucion> Distribuciones { get; set; }
    }
}
