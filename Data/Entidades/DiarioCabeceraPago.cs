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
    public class DiarioCabeceraPago
    {
        public DiarioCabeceraPago()
        {
            DiarioDetallesLineaPago = new HashSet<DiarioDetalleLineaPago>();
        }

        [Key, ForeignKey("TareaPago")]
        public long Id { get; set; }
        [Required]
        [StringLength(300)]
        public string nombreDiario { get; set; }
        [Required]
        public string descripcionDiario { get; set; }
        [Required]
        [StringLength(100)]
        public string codigoCompania { get; set; }
        [Required]
        [StringLength(15)]
        public string numeroDiario { get; set; }

        public virtual TareaPago TareaPago { get; set; }
        public virtual DiarioLineaProveedorPago DiarioLineaProveedorPago { get; set; }
        public virtual DiarioCierrePago DiarioCierrePago { get; set; }

        public virtual ICollection<DiarioDetalleLineaPago> DiarioDetallesLineaPago { get; set; }
    }
}
