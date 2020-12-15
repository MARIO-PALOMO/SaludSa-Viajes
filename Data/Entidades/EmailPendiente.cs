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
    public class EmailPendiente
    {
        public EmailPendiente()
        {
            EmailsDestino = new HashSet<EmailDestinatario>();
            EmailsCopia = new HashSet<EmailDestinatario>();
        }

        [Key]
        public long Id { get; set; }
        public string Cuerpo { get; set; }
        public string Asunto { get; set; }
        public string Razon { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string UsuarioEnvio { get; set; }

        [ForeignKey("Tarea")]
        public long? TareaId { get; set; }
        public virtual Tarea Tarea { get; set; }

        [ForeignKey("TareaPago")]
        public long? TareaPagoId { get; set; }
        public virtual TareaPago TareaPago { get; set; }

        [InverseProperty("EmailPendienteDestinatario")]
        public virtual ICollection<EmailDestinatario> EmailsDestino { get; set; }
        [InverseProperty("EmailPendienteCopia")]
        public virtual ICollection<EmailDestinatario> EmailsCopia { get; set; }
    }
}
