using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ProveedorViewModel
    {
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Bloqueado { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
