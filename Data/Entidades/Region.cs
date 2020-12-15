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
    public class Region
    {
        public Region()
        {
            Ciudades = new HashSet<Ciudad>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }
        [Required]
        [StringLength(100)]
        public string CodigoAuxiliar { get; set; }

        public virtual ICollection<Ciudad> Ciudades { get; set; }
    }
}
