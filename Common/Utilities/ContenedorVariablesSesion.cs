using System;
using System.Collections.Generic;
using System.Configuration;
using Common.ViewModels;

namespace Common.Utilities
{
    public class ContenedorVariablesSesion
    {
        public ContenedorVariablesSesion()
        {
            errores = new List<string>();
            warnings = new List<string>();
            infos = new List<string>();
            RolesAdministrativos = new List<long>();
        }

        public UsuarioViewModel usuario { get; set; }
        public TokenViewModel token { get; set; }
        public string SistemaOperativo { get; set; }
        public string DispositivoNavegador { get; set; }
        public string DireccionIP { get; set; }

        public string UrlServicios { get; set; }
        public string CodigoAplicacion { get; set; }

        public string ClientId { get; set; }

        public string UsuarioSistema { get; set; }
        public string ClaveSistema { get; set; }

        public string EmailDestinatario { get; set; }
        public string EmailNombreOrigen { get; set; }
        public string EmailEmailOrigen { get; set; }
        public string EmailTiempoEspera { get; set; }

        public string UrlWeb { get; set; }
        public string UrlWebPago { get; set; }

        public string UrlVisorRidePdf { get; set; }

        public string IdClaseMFiles { get; set; }

        public string OrigenLog { get; set; }

        public List<long> RolesAdministrativos { get; set; }

        public List<string> errores { get; set; }
        public List<string> warnings { get; set; }
        public List<string> infos { get; set; }
    }
}
