using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class SolicitudCompraDetalleViewModel
    {
        public long Id { get; set; }
        public long SolicitudCompraCabeceraId { get; set; }
        public long EstadoId { get; set; }
        public bool CompraInternacional { get; set; }
        public string Tipo { get; set; }
        public string ProductoCodigoArticulo { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCodigoGrupo { get; set; }
        public ProductoViewModel ProductoBien { get; set; }
        public ProductoViewModel ProductoServicio { get; set; }
        public string Observacion { get; set; }
        public decimal Cantidad { get; set; }
        public string Url { get; set; }

        public decimal Valor { get; set; }
        public decimal Total { get; set; }

        public decimal Saldo { get; set; }

        public bool ProcesadoEnOrdenMadre { get; set; }

        public ImpuestoVigenteViewModel Impuesto { get; set; }
        public ProveedorViewModel Proveedor { get; set; }

        public List<PlantillasDistribucionDetalleViewModel> PlantillaDistribucionDetalle { get; set; }
    }
}
