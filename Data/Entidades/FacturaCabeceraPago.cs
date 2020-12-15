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
    public class FacturaCabeceraPago
    {
        public FacturaCabeceraPago()
        {
            FacturaDetallesPago = new HashSet<FacturaDetallePago>();
        }

        [NotMapped]
        public string AdjuntoName { get; set; }
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(15)]
        public string NoFactura { get; set; }
        [StringLength(15)]
        public string NoLiquidacion { get; set; }
        [Required]
        [StringLength(100)]
        public string NoAutorizacion { get; set; }
        [Required]
        [StringLength(300)]
        public string Concepto { get; set; }
        [Required]
        public DateTime FechaEmision { get; set; }
        [Required]
        public DateTime FechaVencimiento { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        [StringLength(100)]
        public string AprobacionJefeArea { get; set; }
        [StringLength(100)]
        public string AprobacionSubgerenteArea { get; set; }
        [StringLength(100)]
        public string AprobacionGerenteArea { get; set; }
        [StringLength(100)]
        public string AprobacionVicePresidenteFinanciero { get; set; }
        [StringLength(100)]
        public string AprobacionGerenteGeneral { get; set; }

        [Required]
        [ForeignKey("TipoPago")]
        public long TipoPagoId { get; set; }
        public virtual TipoPago TipoPago { get; set; }

        [ForeignKey("ComprobanteElectronico")]
        public long? ComprobanteElectronicoId { get; set; }
        public virtual ComprobanteElectronico ComprobanteElectronico { get; set; }

        [ForeignKey("SolicitudPagoCabecera")]
        public long SolicitudPagoCabeceraId { get; set; }
        public virtual SolicitudPagoCabecera SolicitudPagoCabecera { get; set; }

        public virtual ICollection<FacturaDetallePago> FacturaDetallesPago { get; set; }
    }
}
