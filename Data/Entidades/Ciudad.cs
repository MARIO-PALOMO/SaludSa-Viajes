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
    public class Ciudad
    {
        public Ciudad()
        {
            RolesAdministrativo = new HashSet<RolAdministrativo>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(100)]
        public string Codigo { get; set; }
        [Required]
        [StringLength(100)]
        public string Provincia { get; set; }
        [Required]
        [StringLength(500)]
        public string Direccion { get; set; }

        [ForeignKey("Region")]
        public long? RegionId { get; set; }
        public virtual Region Region { get; set; }

        public virtual ICollection<RolAdministrativo> RolesAdministrativo { get; set; }
    }
}
