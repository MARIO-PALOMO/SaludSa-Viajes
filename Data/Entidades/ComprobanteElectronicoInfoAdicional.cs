using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entidades
{
    public class ComprobanteElectronicoInfoAdicional
    {
        [Key]
        public long Id { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }

        [ForeignKey("ComprobanteElectronico")]
        public long ComprobanteElectronicoId { get; set; }
        public virtual ComprobanteElectronico ComprobanteElectronico { get; set; }
    }
}
