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
    public class EmailDestinatario
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(300)]
        public string Direccion { get; set; }

        [ForeignKey("EmailPendienteDestinatario")]
        public long? EmailPendienteDestinatarioId { get; set; }
        public virtual EmailPendiente EmailPendienteDestinatario { get; set; }

        [ForeignKey("EmailPendienteCopia")]
        public long? EmailPendienteCopiaId { get; set; }
        public virtual EmailPendiente EmailPendienteCopia { get; set; }
    }
}
