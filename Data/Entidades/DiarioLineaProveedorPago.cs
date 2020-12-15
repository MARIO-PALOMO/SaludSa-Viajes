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
    public class DiarioLineaProveedorPago
    {
        [Key, ForeignKey("DiarioCabeceraPago")]
        public long Id { get; set; }
        [Required]
        [StringLength(2)]
        public string tipoDiario { get; set; }
        [Required]
        [StringLength(15)]
        public string numeroDiario { get; set; }
        [Required]
        public decimal valor { get; set; }
        [Required]
        public DateTime fecha { get; set; }
        [Required]
        [StringLength(100)]
        public string proveedor { get; set; }
        [Required]
        public string descripcion { get; set; }
        [StringLength(100)]
        public string referencia { get; set; }
        [Required]
        public string departameto { get; set; }
        [Required]
        public string perfilAsiento { get; set; }
        [Required]
        [StringLength(100)]
        public string codigoCompania { get; set; }
        [StringLength(15)]
        public string numeroFactura { get; set; }
        [Required]
        [StringLength(15)]
        public string lineaProveedorId { get; set; }

        public virtual DiarioCabeceraPago DiarioCabeceraPago { get; set; }
    }
}
