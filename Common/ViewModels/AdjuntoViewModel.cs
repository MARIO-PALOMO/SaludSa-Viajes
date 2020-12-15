using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class AdjuntoViewModel
    {
        public string Id { get; set; }
        public byte[] ContenidoArchivo { get; set; }
        public List<PropiedadAdjuntoViewModel> Propiedades { get; set; }
    }
}
