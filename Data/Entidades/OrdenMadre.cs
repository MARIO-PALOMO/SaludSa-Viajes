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
    public class OrdenMadre
    {
        public OrdenMadre()
        {
            LineasOrdenMadre = new HashSet<OrdenMadreLinea>();
            Recepciones = new HashSet<Recepcion>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(20)]
        public string Ruc { get; set; }
        [Required]
        [StringLength(100)]
        public string NumeroOrdenMadre { get; set; }

        [ForeignKey("SolicitudCompraCabecera")]
        public long SolicitudCompraCabeceraId { get; set; }
        public virtual SolicitudCompraCabecera SolicitudCompraCabecera { get; set; }

        public virtual ICollection<OrdenMadreLinea> LineasOrdenMadre { get; set; }
        public virtual ICollection<Recepcion> Recepciones { get; set; }
    }
}
