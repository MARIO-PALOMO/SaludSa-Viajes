using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Data.Entidades
{
    public class SolicitudCompraCabecera
    {
        public SolicitudCompraCabecera()
        {
            Detalles = new HashSet<SolicitudCompraDetalle>();
            Tareas = new HashSet<Tarea>();
            OrdenesMadre = new HashSet<OrdenMadre>();
        }

        [Key]
        public long Id { get; set; }
        public long? NumeroSolicitud { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        [Required]
        [StringLength(100)]
        public string SolicitanteUsuario { get; set; }
        [Required]
        [StringLength(300)]
        public string SolicitanteNombreCompleto { get; set; }
        [Required]
        [StringLength(100)]
        public string SolicitanteCiudadCodigo { get; set; }
        [Required]
        [StringLength(100)]
        public string EmpresaCodigo { get; set; }
        [Required]
        [StringLength(300)]
        public string EmpresaNombre { get; set; }
        [StringLength(300)]
        public string ProveedorSugerido { get; set; }
        [Required]
        [StringLength(30)]
        public string Frecuencia { get; set; }
        [Required]
        public decimal MontoEstimado { get; set; }
        [StringLength(100)]
        public string ProductoMercadeoCodigo { get; set; }
        [StringLength(300)]
        public string ProductoMercadeoNombre { get; set; }
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }
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
        public string JsonOriginal { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public virtual ICollection<SolicitudCompraDetalle> Detalles { get; set; }
        public virtual ICollection<Tarea> Tareas { get; set; }
        public virtual ICollection<OrdenMadre> OrdenesMadre { get; set; }
    }
}
