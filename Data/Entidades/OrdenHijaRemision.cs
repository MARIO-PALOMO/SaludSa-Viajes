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
    public class OrdenHijaRemision
    {
        [Key, ForeignKey("OrdenHija")]
        public long Id { get; set; }
        [StringLength(100)]
        public string NumeroRemisionOrdenHija { get; set; }
        public string RespuestaRemisionOrdenHija { get; set; }

        public virtual OrdenHija OrdenHija { get; set; }
    }
}
