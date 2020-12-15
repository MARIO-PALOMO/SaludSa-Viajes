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
    public class OrdenHija
    {
        public OrdenHija()
        {
            OrdenHijaLineas = new HashSet<OrdenHijaLinea>();
        }

        [Key, ForeignKey("Recepcion")]
        public long Id { get; set; }
        [StringLength(100)]
        public string NumeroOrdenHija { get; set; }


        [ForeignKey("OrdenMadre")]
        public long OrdenMadreId { get; set; }
        public virtual OrdenMadre OrdenMadre { get; set; }

        public virtual Recepcion Recepcion { get; set; }

        public virtual OrdenHijaRemision OrdenHijaRemision { get; set; }

        public virtual ICollection<OrdenHijaLinea> OrdenHijaLineas { get; set; }
    }
}
