using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class PlantillasDistribucionDetalleViewModel
    {
        public long Id { get; set; }
        public decimal Porcentaje { get; set; }
        public long EstadoId { get; set; }
        public DepartamentoViewModel Departamento { get; set; }
        public string DepartamentoCodigo { get; set; }
        public string DepartamentoDescripcion { get; set; }
        public string DepartamentoCodigoDescripcion { get; set; }
        public CentroCostoViewModel CentroCosto { get; set; }
        public string CentroCostoCodigo { get; set; }
        public string CentroCostoDescripcion { get; set; }
        public string CentroCostoCodigoDescripcion { get; set; }
        public PropositoViewModel Proposito { get; set; }
        public string PropositoCodigo { get; set; }
        public string PropositoDescripcion { get; set; }
        public string PropositoCodigoDescripcion { get; set; }
        public List<CentroCostoViewModel> MetadatosCentrosCosto { get; set; }
        public List<PropositoViewModel> MetadatosPropositos { get; set; }
    }
}
