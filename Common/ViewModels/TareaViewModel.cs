using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class TareaViewModel
    {
        public long Id { get; set; }
        public string Actividad { get; set; }
        public string UsuarioResponsable { get; set; }
        public string NombreCompletoResponsable { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public int TiempoColor { get; set; }
        public int TipoTarea { get; set; }
        public long? SolicitudCompraCabeceraId { get; set; }
        public long? SolicitudCompraCabeceraNumero { get; set; }
        public string SolicitudCompraCabeceraDescripcion { get; set; }
        public string SolicitudCompraCabeceraSolicitante { get; set; }
        public long EstadoId { get; set; }
        public string Accion { get; set; }
        public string Observacion { get; set; }
        public bool Recordatorio1 { get; set; }
        public bool Recordatorio2 { get; set; }
        public bool Recordatorio3 { get; set; }
        public bool TiempoAviso { get; set; }
        public bool? RetornaAJefeInmediato { get; set; }
        public string UsuarioGerenteArea { get; set; }
        public string UsuarioVicepresidenteFinanciero { get; set; }
        public string UsuarioAprobadorDesembolso { get; set; }
        public string NombreProveedor { get; set; }
        public long? OrdenMadreId { get; set; }
        public string NumeroOrdenMadre { get; set; }
        public long? RecepcionId { get; set; }
        public int? NumeroRecepcion { get; set; }

        public long? TareaPadreId { get; set; }
        public TareaViewModel TareaPadre { get; set; }
    }
}
