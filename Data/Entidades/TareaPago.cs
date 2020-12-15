using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entidades
{
    public class TareaPago
    {
        public TareaPago()
        {
            HistorialesEmail = new HashSet<HistorialEmail>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Actividad { get; set; }
        [Required]
        [StringLength(100)]
        [Index]
        public string UsuarioResponsable { get; set; }
        [Required]
        [StringLength(300)]
        public string NombreCompletoResponsable { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string EmailResponsable { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        [Required]
        public int TipoTarea { get; set; }
        [StringLength(100)]
        public string Accion { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }


        [ForeignKey("SolicitudPagoCabecera")]
        public long? SolicitudPagoCabeceraId { get; set; }
        public virtual SolicitudPagoCabecera SolicitudPagoCabecera { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        [ForeignKey("TareaPadre")]
        public long? TareaPadreId { get; set; }
        public virtual TareaPago TareaPadre { get; set; }

        [ForeignKey("FacturaCabeceraPago")]
        public long? FacturaCabeceraPagoId { get; set; }
        public virtual FacturaCabeceraPago FacturaCabeceraPago { get; set; }

        public virtual InformacionContabilidadPago InformacionContabilidadPago { get; set; }
        public virtual DiarioCabeceraPago DiarioCabeceraPago { get; set; }

        public virtual ICollection<HistorialEmail> HistorialesEmail { get; set; }
    }
}
