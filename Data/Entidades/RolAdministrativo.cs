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
    public class RolAdministrativo
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        [Index("IX_ColaboradorUsuario_EmpresaCodigo_RolId_CiudadId", 1, IsUnique = true)]
        public string ColaboradorUsuario { get; set; }
        [Required]
        [StringLength(300)]
        public string ColaboradorNombreCompleto { get; set; }
        [StringLength(100)]
        [Index("IX_ColaboradorUsuario_EmpresaCodigo_RolId_CiudadId", 2, IsUnique = true)]
        public string EmpresaCodigo { get; set; }
        [StringLength(300)]
        public string EmpresaNombre { get; set; }
        [Required]
        [StringLength(100)]
        public string CreadoPor { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [ForeignKey("Rol")]
        [Index("IX_ColaboradorUsuario_EmpresaCodigo_RolId_CiudadId", 3, IsUnique = true)]
        public long RolId { get; set; }
        public virtual Rol Rol { get; set; }
        
        [ForeignKey("Ciudad")]
        [Index("IX_ColaboradorUsuario_EmpresaCodigo_RolId_CiudadId", 4, IsUnique = true)]
        public long? CiudadId { get; set; }
        public virtual Ciudad Ciudad { get; set; }
    }
}
