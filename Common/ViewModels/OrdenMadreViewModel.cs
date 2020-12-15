using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class OrdenMadreViewModel
    {
        public OrdenMadreViewModel()
        {
            LineasOrdenMadre = new List<OrdenMadreLineaViewModel>();
        }

        public string Ruc { get; set; }
        public string NumeroOrdenMadre { get; set; }
        public List<OrdenMadreLineaViewModel> LineasOrdenMadre { get; set; }
    }
}
