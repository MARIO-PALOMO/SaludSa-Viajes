using Common.Utilities;
using Common.ViewModels;
using Logic;
using Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Model.External
{
    public class CompanyModel
    {
        public static List<EmpresaViewModel> getCompany(TokenViewModel token)
        {
            ContenedorVariablesSesion sesion = new ContenedorVariablesSesion();

            var url = ConfigurationManager.AppSettings["Url"].ToString();
            //var user = ConfigurationManager.AppSettings["Usuario"].ToString();
            //var pass = ConfigurationManager.AppSettings["Clave"].ToString();
            //var clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
            var applicationCode = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();
            //var token = AccountBLL.ObtenerToken(url, user, pass, clientId, "WebIngenesisCompras");

            sesion.UrlServicios = url;
            sesion.token = token;
            sesion.CodigoAplicacion = applicationCode;
            sesion.SistemaOperativo = "Windows";
            sesion.DispositivoNavegador = "Google-Chrome";
            sesion.DireccionIP = "10.10.10.10";
            sesion.OrigenLog = "WebIngenesisCompras";

            return EmpresaRCL.ObtenerEmpresas(sesion);
        }
    }
}
