using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entidades
{
    public class Tarea
    {
        public Tarea()
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
        public int TiempoColor { get; set; }
        [Required]
        public int TipoTarea { get; set; }
        [StringLength(100)]
        public string Accion { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [Required]
        public bool Recordatorio1 { get; set; }
        [Required]
        public bool Recordatorio2 { get; set; }
        [Required]
        public bool Recordatorio3 { get; set; }
        [Required]
        public bool TiempoAviso { get; set; }
        public bool? RetornaAJefeInmediato { get; set; }
        [StringLength(100)]
        public string UsuarioGerenteArea { get; set; }
        [StringLength(100)]
        public string UsuarioVicepresidenteFinanciero { get; set; }
        [StringLength(100)]
        public string UsuarioAprobadorDesembolso { get; set; }
        public int CantIteraciones10Minutos { get; set; }


        [ForeignKey("SolicitudCompraCabecera")]
        public long? SolicitudCompraCabeceraId { get; set; }
        public virtual SolicitudCompraCabecera SolicitudCompraCabecera { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        [ForeignKey("TareaPadre")]
        public long? TareaPadreId { get; set; }
        public virtual Tarea TareaPadre { get; set; }

        [ForeignKey("OrdenMadre")]
        public long? OrdenMadreId { get; set; }
        public virtual OrdenMadre OrdenMadre { get; set; }

        [ForeignKey("Recepcion")]
        public long? RecepcionId { get; set; }
        public virtual Recepcion Recepcion { get; set; }

        public virtual ICollection<HistorialEmail> HistorialesEmail { get; set; }
    }
}
