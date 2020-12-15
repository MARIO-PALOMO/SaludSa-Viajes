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
    public class RecepcionLinea
    {
        public RecepcionLinea()
        {
            Distribuciones = new HashSet<SolicitudCompraDetalleDistribucion>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        [Required]
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }

        [ForeignKey("Recepcion")]
        public long RecepcionId { get; set; }
        public virtual Recepcion Recepcion { get; set; }

        [ForeignKey("SolicitudCompraDetalle")]
        public long SolicitudCompraDetalleId { get; set; }
        public virtual SolicitudCompraDetalle SolicitudCompraDetalle { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public virtual ICollection<SolicitudCompraDetalleDistribucion> Distribuciones { get; set; }
    }
}
