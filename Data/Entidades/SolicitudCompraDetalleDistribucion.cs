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
    public class SolicitudCompraDetalleDistribucion
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public decimal Porcentaje { get; set; }

        [Required]
        public string DepartamentoCodigo { get; set; }
        [Required]
        public string DepartamentoDescripcion { get; set; }
        [Required]
        public string DepartamentoCodigoDescripcion { get; set; }

        [Required]
        public string CentroCostoCodigo { get; set; }
        [Required]
        public string CentroCostoDescripcion { get; set; }
        [Required]
        public string CentroCostoCodigoDescripcion { get; set; }

        [Required]
        public string PropositoCodigo { get; set; }
        [Required]
        public string PropositoDescripcion { get; set; }
        [Required]
        public string PropositoCodigoDescripcion { get; set; }

        [ForeignKey("SolicitudCompraDetalle")]
        public long? SolicitudCompraDetalleId { get; set; }
        public virtual SolicitudCompraDetalle SolicitudCompraDetalle { get; set; }

        [ForeignKey("RecepcionLinea")]
        public long? RecepcionLineaId { get; set; }
        public virtual RecepcionLinea RecepcionLinea { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
    }
}
