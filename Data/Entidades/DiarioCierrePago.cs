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
    public class DiarioCierrePago
    {
        [Key, ForeignKey("DiarioCabeceraPago")]
        public long Id { get; set; }
        [Required]
        [StringLength(15)]
        public string numeroDiario { get; set; }
        [StringLength(100)]
        public string autorizacion { get; set; }
        [Required]
        public DateTime fechaVigencia { get; set; }
        [StringLength(100)]
        public string autorizacionElectronica { get; set; }
        [Required]
        [StringLength(100)]
        public string codigoCompania { get; set; }
        [Required]
        [StringLength(15)]
        public string cierreDiarioId { get; set; }

        public virtual DiarioCabeceraPago DiarioCabeceraPago { get; set; }
    }
}
