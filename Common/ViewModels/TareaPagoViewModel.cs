using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class TareaPagoViewModel
    {
        public long Id { get; set; }
        public string Actividad { get; set; }
        public string UsuarioResponsable { get; set; }
        public string NombreCompletoResponsable { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public int TipoTarea { get; set; }
        public long? SolicitudPagoCabeceraId { get; set; }
        public long? SolicitudPagoCabeceraNumero { get; set; }
        public string SolicitudPagoCabeceraDescripcion { get; set; }
        public string SolicitudPagoCabeceraSolicitante { get; set; }
        public long EstadoId { get; set; }
        public string Accion { get; set; }
        public string Observacion { get; set; }
        public long? TareaPadreId { get; set; }
        public long? FacturaCabeceraPagoId { get; set; }
        public TareaPagoViewModel TareaPadre { get; set; }
        public FacturaCabeceraPagoViewModel FacturaCabeceraPago { get; set; }
        public InformacionContabilidadPagoViewModel InformacionContabilidadPago { get; set; }
        public string SolicitudPagoCabeceraObservacion { get; set; }
    }
}
