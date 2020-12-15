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
    public class OrdenMadreLinea
    {
        [Key, ForeignKey("SolicitudCompraDetalle")]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Tipo { get; set; }
        public long Secuencial { get; set; }
        [Required]
        [StringLength(100)]
        public string CodigoLinea { get; set; }
        [Required]
        [StringLength(500)]
        public string Observacion { get; set; }
        [Required]
        [StringLength(300)]
        public string CodigoArticulo { get; set; }
        [Required]
        public decimal Valor { get; set; }
        [Required]
        public decimal Cantidad { get; set; }

        [ForeignKey("OrdenMadre")]
        public long OrdenMadreId { get; set; }
        public virtual OrdenMadre OrdenMadre { get; set; }

        public virtual SolicitudCompraDetalle SolicitudCompraDetalle { get; set; }
    }
}
