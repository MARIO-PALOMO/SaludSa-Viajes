using System;
using System.Collections.Generic;
using System.Web;

namespace Common.ViewModels
{
    public class OrdenCompraLineaViewModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Observacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Iva { get; set; }
        public decimal Importe { get; set; }
    }
}