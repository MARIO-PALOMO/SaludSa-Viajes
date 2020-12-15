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
    public class DiarioDetalleLineaPago
    {

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(2)]
        public string tipoDiario { get; set; }
        [Required]
        [StringLength(15)]
        public string numeroDiario { get; set; }
        [Required]
        public decimal valor { get; set; }
        [StringLength(100)]
        public string cuentaContable { get; set; }
        [Required]
        [StringLength(300)]
        public string descripcion { get; set; }
        public string departamento { get; set; }
        public string centroCosto { get; set; }
        public string proposito { get; set; }
        [StringLength(100)]
        public string codigoProyecto { get; set; }
        [StringLength(100)]
        public string codigoCompania { get; set; }        
        public string parametros { get; set; }
        public bool credito { get; set; }
        [Required]
        [StringLength(15)]
        public string detalleLineaId { get; set; }

        [ForeignKey("DiarioCabeceraPago")]
        public long? DiarioCabeceraPagoId { get; set; }
        public virtual DiarioCabeceraPago DiarioCabeceraPago { get; set; }
    }
}
