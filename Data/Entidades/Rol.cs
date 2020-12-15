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
    public class Rol
    {
        public Rol()
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
        [StringLength(1)]
        public string Tipo { get; set; }

        public virtual ICollection<RolAdministrativo> RolesAdministrativo { get; set; }
    }
}
