using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Data.Entidades
{
    public class SolicitudPagoCabecera
    {
        public SolicitudPagoCabecera()
        {
            Facturas = new HashSet<FacturaCabeceraPago>();
            Tareas = new HashSet<TareaPago>();
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
        [Required]
        [StringLength(100)]
        public string NombreCorto { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [StringLength(100)]
        public string BeneficiarioIdentificacion { get; set; }
        [StringLength(20)]
        public string BeneficiarioTipoIdentificacion { get; set; }
        [StringLength(300)]
        public string BeneficiarioNombre { get; set; }
        [Required]
        public decimal MontoTotal { get; set; }
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

        public virtual ICollection<FacturaCabeceraPago> Facturas { get; set; }
        public virtual ICollection<TareaPago> Tareas { get; set; }
    }
}
