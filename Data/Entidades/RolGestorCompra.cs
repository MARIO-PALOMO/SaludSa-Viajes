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
    public class RolGestorCompra
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(100)]
        public string CodigoEmpresa { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreEmpresa { get; set; }
        [Required]
        [StringLength(100)]
        public string CodigoTipoCompra { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreTipoCompra { get; set; }
        [Required]
        [StringLength(100)]
        public string UsuarioGestorSierra { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreGestorSierra { get; set; }
        [Required]
        [StringLength(100)]
        public string UsuarioGestorCosta { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreGestorCosta { get; set; }
        [Required]
        [StringLength(100)]
        public string UsuarioGestorAustro { get; set; }
        [Required]
        [StringLength(255)]
        public string NombreGestorAustro { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
    }
}
