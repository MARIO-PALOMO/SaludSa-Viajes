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
    public class Recepcion
    {
        public Recepcion()
        {
            RecepcionLineas = new HashSet<RecepcionLinea>();
            ComprobantesElectronicos = new HashSet<ComprobanteElectronico>();
        }

        [Key]
        public long Id { get; set; }
        public int NumeroRecepcion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public bool Aprobada { get; set; }
        public bool Contabilizada { get; set; }
        public bool EsperandoAutorizacionAnulacion { get; set; }
        public string UsuarioCreador { get; set; }

        [ForeignKey("OrdenMadre")]
        public long OrdenMadreId { get; set; }
        public virtual OrdenMadre OrdenMadre { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        public virtual OrdenHija OrdenHija { get; set; }

        public virtual ICollection<RecepcionLinea> RecepcionLineas { get; set; }
        public virtual ICollection<ComprobanteElectronico> ComprobantesElectronicos { get; set; }
    }
}
