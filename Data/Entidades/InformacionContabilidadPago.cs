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
    public class InformacionContabilidadPago
    {
        [Key, ForeignKey("TareaPago")]
        public long Id { get; set; }
        [Required]
        public string TipoDiarioCodigo { get; set; }
        [Required]
        public string TipoDiarioDescripcion { get; set; }
        [Required]
        public string DiarioCodigo { get; set; }
        [Required]
        public string DiarioDescripcion { get; set; }
        [Required]
        public string PerfilAsientoContableCodigo { get; set; }
        [Required]
        public string PerfilAsientoContableDescripcion { get; set; }
        [Required]
        public string DepartamentoCodigo { get; set; }
        [Required]
        public string DepartamentoDescripcion { get; set; }
        [Required]
        public string DepartamentoCodigoDescripcion { get; set; }
        [Required]
        public string CuentaContableCodigo { get; set; }
        [Required]
        public string CuentaContableNombre { get; set; }
        [Required]
        public string CuentaContableTipo { get; set; }
        public bool? EsReembolso { get; set; }

        public virtual TareaPago TareaPago { get; set; }
    }
}
