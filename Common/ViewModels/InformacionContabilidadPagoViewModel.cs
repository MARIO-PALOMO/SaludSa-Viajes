using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class InformacionContabilidadPagoViewModel
    {
        public long Id { get; set; }
        public string TipoDiarioCodigo { get; set; }
        public string TipoDiarioDescripcion { get; set; }
        public string DiarioCodigo { get; set; }
        public string DiarioDescripcion { get; set; }
        public string PerfilAsientoContableCodigo { get; set; }
        public string PerfilAsientoContableDescripcion { get; set; }
        public string DepartamentoCodigo { get; set; }
        public string DepartamentoDescripcion { get; set; }
        public string DepartamentoCodigoDescripcion { get; set; }
        public string CuentaContableCodigo { get; set; }
        public string CuentaContableNombre { get; set; }
        public string CuentaContableTipo { get; set; }
        public bool? EsReembolso { get; set; }
    }
}
