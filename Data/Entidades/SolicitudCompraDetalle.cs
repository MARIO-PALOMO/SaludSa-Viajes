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
    public class SolicitudCompraDetalle
    {
        public SolicitudCompraDetalle()
        {
            Distribuciones = new HashSet<SolicitudCompraDetalleDistribucion>();
            RecepcionLineas = new HashSet<RecepcionLinea>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        public bool CompraInternacional { get; set; }
        [Required]
        [StringLength(30)]
        public string Tipo { get; set; }
        [StringLength(300)]
        public string Producto { get; set; }
        [StringLength(300)]
        public string ProductoNombre { get; set; }
        [StringLength(300)]
        public string GrupoProducto { get; set; }
        [StringLength(300)]
        public string GrupoProductoNombre { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        [Url]
        [StringLength(300)]
        public string Url { get; set; }

        
        public decimal Valor { get; set; }
        public decimal Total { get; set; }

        [StringLength(100)]
        public string CodigoImpuestoVigente { get; set; }
        public decimal? PorcentajeImpuestoVigente { get; set; }
        [StringLength(100)]
        public string DescripcionImpuestoVigente { get; set; }

        [StringLength(20)]
        public string IdentificacionProveedor { get; set; }
        [StringLength(100)]
        public string NombreProveedor { get; set; }
        [StringLength(100)]
        public string RazonSocialProveedor { get; set; }
        [StringLength(10)]
        public string TipoIdentificacionProveedor { get; set; }
        [StringLength(10)]
        public string BloqueadoProveedor { get; set; }
        [StringLength(100)]
        public string CorreoProveedor { get; set; }
        [StringLength(100)]
        public string TelefonoProveedor { get; set; }
        [StringLength(300)]
        public string DireccionProveedor { get; set; }


        [ForeignKey("SolicitudCompraCabecera")]
        public long? SolicitudCompraCabeceraId { get; set; }
        public virtual SolicitudCompraCabecera SolicitudCompraCabecera { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public virtual OrdenMadreLinea OrdenMadreLinea { get; set; }

        public virtual ICollection<SolicitudCompraDetalleDistribucion> Distribuciones { get; set; }
        public virtual ICollection<RecepcionLinea> RecepcionLineas { get; set; }
    }
}
