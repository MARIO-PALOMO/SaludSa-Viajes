using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class TipoPagoViewModel
    {
        public long Id { get; set; }
        public string CuentaContableCodigo { get; set; }
        public string CuentaContableNombre { get; set; }
        public string CuentaContableTipo { get; set; }
        public string Referencia { get; set; }
        public bool EsReembolso { get; set; }
        public string EmpresaCodigo { get; set; }

        public CuentaContableViewModel CuentaContable { get; set; }

        public long EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public EstadoViewModel Estado { get; set; }
    }
}
