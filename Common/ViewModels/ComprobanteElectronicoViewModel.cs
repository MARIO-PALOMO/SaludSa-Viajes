using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class ComprobanteElectronicoViewModel
    {
        public ComprobanteElectronicoViewModel()
        {
            infoAdicional = new List<ComprobanteElectronicoInfoAdicionalViewModel>();
        }

        public long Id { get; set; }
        public decimal baseImponibleCero { get; set; }
        public decimal baseImponibleIva { get; set; }
        public decimal baseSinCargos { get; set; }
        public string claveAcceso { get; set; }
        public int codigoImpuestoIva { get; set; }
        public string establecimiento { get; set; }
        public int estado { get; set; }
        public string fechaAutorizacion { get; set; }
        public string fechaEmisionRetencion { get; set; }
        public decimal iva { get; set; }
        public string numeroAutorizacion { get; set; }
        public string observaciones { get; set; }
        public decimal porcentajeRetencion { get; set; }
        public string puntoEmision { get; set; }
        public string razonSocial { get; set; }
        public string ruc { get; set; }
        public string secuencial { get; set; }
        public string tipoDocumento { get; set; }
        public decimal valorRetencion { get; set; }
        public decimal valorTotal { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumentoNombre { get; set; }
        public string estadoNombre { get; set; }

        public List<ComprobanteElectronicoInfoAdicionalViewModel> infoAdicional { get; set; }
    }
}
