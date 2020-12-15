using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class SolicitudCompraCabeceraViewModel
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public long? NumeroSolicitud { get; set; }
        public string ProveedorSugerido { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public int CantTareasActivas { get; set; }

        public long EstadoId { get; set; }
        public string SolicitanteObjUsuario { get; set; }
        public UsuarioViewModel SolicitanteObj { get; set; }
        public ProductoMercadeoViewModel ProductoMercadeo { get; set; }
        public string AprobacionJefeAreaUsuario { get; set; }
        public UsuarioViewModel AprobacionJefeArea { get; set; }
        public string AprobacionSubgerenteAreaUsuario { get; set; }
        public UsuarioViewModel AprobacionSubgerenteArea { get; set; }
        public string AprobacionGerenteAreaUsuario { get; set; }
        public UsuarioViewModel AprobacionGerenteArea { get; set; }
        public string AprobacionVicePresidenteFinancieroUsuario { get; set; }
        public UsuarioViewModel AprobacionVicePresidenteFinanciero { get; set; }
        public string AprobacionGerenteGeneralUsuario { get; set; }
        public UsuarioViewModel AprobacionGerenteGeneral { get; set; }
        public EmpresaViewModel EmpresaParaLaQueSeCompra { get; set; }
        public string Frecuencia { get; set; }
        public decimal MontoEstimado { get; set; }
        public string RequerimientosAdjuntosPrevisualizar { get; set; }
        public string JsonOriginal { get; set; }
        public List<SolicitudCompraDetalleViewModel> Detalles { get; set; }
        public List<TareaViewModel> Tareas { get; set; }
    }
}
