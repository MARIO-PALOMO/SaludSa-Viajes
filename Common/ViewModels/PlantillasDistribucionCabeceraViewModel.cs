using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class PlantillasDistribucionCabeceraViewModel
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public long EstadoId { get; set; }
        public string UsuarioPropietario { get; set; }
        public string DescripcionDepartamentoPropietario { get; set; }
        public string EmpresaCodigo { get; set; }
        public List<PlantillasDistribucionDetalleViewModel> Detalles { get; set; }
    }
}
