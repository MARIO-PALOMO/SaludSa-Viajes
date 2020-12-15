using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class RecepcionLineaViewModel
    {
        public long Id { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Valor { get; set; }
        public long RecepcionId { get; set; }
        public long SolicitudCompraDetalleId { get; set; }
        public long EstadoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal? PorcentajeImpuestoVigente { get; set; }
        public decimal Saldo { get; set; }
        public decimal ValorUnitario { get; set; }

        public List<PlantillasDistribucionDetalleViewModel> PlantillaDistribucionDetalle { get; set; }
    }
}
