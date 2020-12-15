using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class CiudadViewModel
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Provincia { get; set; }
        public string Direccion { get; set; }
        public RegionViewModel Region { get; set; }
    }
}
