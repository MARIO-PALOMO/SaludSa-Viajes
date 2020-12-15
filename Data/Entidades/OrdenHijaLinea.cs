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
    public class OrdenHijaLinea
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public decimal Cantidad { get; set; }
        [Required]
        [StringLength(100)]
        public string Departamento { get; set; }
        [Required]
        [StringLength(100)]
        public string CentroCosto { get; set; }
        [Required]
        [StringLength(100)]
        public string Proposito { get; set; }
        [Required]
        public decimal PrecioUnitario { get; set; }
        [Required]
        public decimal PrecioTotal { get; set; }

        [ForeignKey("OrdenMadreLinea")]
        public long OrdenMadreLineaId { get; set; }
        public virtual OrdenMadreLinea OrdenMadreLinea { get; set; }

        [ForeignKey("OrdenHija")]
        public long OrdenHijaId { get; set; }
        public virtual OrdenHija OrdenHija { get; set; }
    }
}
