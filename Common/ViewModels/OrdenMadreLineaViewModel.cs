using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class OrdenMadreLineaViewModel
    {
        public string Tipo { get; set; }
        public long Secuencial { get; set; }
        public string CodigoLinea { get; set; }
        public string Descripcion { get; set; }
        public string Observacion { get; set; }
        public string CodigoArticulo { get; set; }
        public decimal Valor { get; set; }
        public string Cantidad { get; set; }
        public double Iva { get; set; }
    }
}
