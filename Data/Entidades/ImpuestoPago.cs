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
    public class ImpuestoPago
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }
        [Required]
        public decimal Porcentaje { get; set; }
        [Required]
        public decimal Compensacion { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
    }
}
