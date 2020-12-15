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
    public class Usuario
    {
        [Key]
        public string NombreUsuario { get; set; }
        public bool EnLaOficina { get; set; }
        [StringLength(100)]
        public string UsuarioReasignar { get; set; }

        [ForeignKey("Estado")]
        public long EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
    }
}
