using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class RolAdministrativoViewModel
    {
        public long Id { get; set; }
        public UsuarioViewModel ColaboradorObj { get; set; }
        public EmpresaViewModel EmpresaObj { get; set; }
        public RolViewModel RolObj { get; set; }
        public CiudadViewModel CiudadObj { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
