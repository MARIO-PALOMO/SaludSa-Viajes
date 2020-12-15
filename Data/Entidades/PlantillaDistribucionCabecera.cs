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
    public class PlantillaDistribucionCabecera
    {
        public PlantillaDistribucionCabecera()
        {
            Detalles = new HashSet<PlantillaDistribucionDetalle>();
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }
        [Required]
        [StringLength(100)]
        public string UsuarioPropietario { get; set; }
        [Required]
        [StringLength(300)]
        public string DescripcionDepartamentoPropietario { get; set; }
        [Required]
        [StringLength(100)]
        public string EmpresaCodigo { get; set; }

        public virtual ICollection<PlantillaDistribucionDetalle> Detalles { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
    }
}
