using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class RolGestorCompraViewModel
    {
        public long Id { get; set; }
        public string CodigoEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public EmpresaViewModel Empresa { get; set; }
        public string CodigoTipoCompra { get; set; }
        public string NombreTipoCompra { get; set; }
        public TipoCompraViewModel TipoCompra { get; set; }
        public string UsuarioGestorSierra { get; set; }
        public string NombreGestorSierra { get; set; }
        public UsuarioViewModel GestorSierra { get; set; }
        public string UsuarioGestorCosta { get; set; }
        public string NombreGestorCosta { get; set; }
        public UsuarioViewModel GestorCosta { get; set; }
        public string UsuarioGestorAustro { get; set; }
        public string NombreGestorAustro { get; set; }
        public UsuarioViewModel GestorAustro { get; set; }
        public long EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public EstadoViewModel Estado { get; set; }
    }
}
