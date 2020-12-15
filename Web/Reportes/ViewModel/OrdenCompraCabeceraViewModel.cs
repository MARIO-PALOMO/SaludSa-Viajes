using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Reportes
{
    public class OrdenCompraCabeceraViewModel
    {
        public string NombreProveedor { get; set; }
        public string DireccionProveedor { get; set; }
        public string EmailProveedor { get; set; }
        public string NumeroSolicitud { get; set; }
        public string NumeroOrdenMadre { get; set; }
        public DateTime FechaOrdenCompra { get; set; }
        public string NombreSolicitante { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}