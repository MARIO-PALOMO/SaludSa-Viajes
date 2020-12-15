using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Common.Utilities;
using Common.ViewModels;
using Logic;

namespace Travel.Model.External
{
    public class UserModel
    {
        public static TokenViewModel getToken()
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var user = ConfigurationManager.AppSettings["Usuario"];
            var pass = ConfigurationManager.AppSettings["Clave"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            return AccountBLL.ObtenerToken(url, user, pass, clientId, "WebIngenesisCompras");
        }       

        public static UsuarioViewModel getUser(TokenViewModel token, string username)
        {
            ContenedorVariablesSesion sesion = new ContenedorVariablesSesion();

            var url = ConfigurationManager.AppSettings["Url"].ToString();
            var applicationCode = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();
   
            sesion.UrlServicios = url;
            sesion.token = token;
            sesion.CodigoAplicacion = applicationCode;
            sesion.SistemaOperativo = "Windows";
            sesion.DispositivoNavegador = "Google-Chrome";
            sesion.DireccionIP = "10.10.10.10";
            sesion.OrigenLog = "WebIngenesisCompras";

            return AccountBLL.ObtenerUsuario(username, sesion);
        }

        public static List<UsuarioViewModel> getUserGroup(TokenViewModel token)
        {
            ContenedorVariablesSesion sesion = new ContenedorVariablesSesion();

            var url = ConfigurationManager.AppSettings["Url"].ToString();
            var applicationCode = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();

            sesion.UrlServicios = url;
            sesion.token = token;
            sesion.CodigoAplicacion = applicationCode;
            sesion.SistemaOperativo = "Windows";
            sesion.DispositivoNavegador = "Google-Chrome";
            sesion.DireccionIP = "10.10.10.10";
            sesion.OrigenLog = "WebIngenesisCompras";

            string[] group = new string[] { "GR Jefes", "GR Subgerentes", "GR Gerentes", "GR Gerente Financiero", "GR Gerentes Generales" };

            List<UsuarioViewModel> lstBoss = null;
            lstBoss = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(group, sesion);

            return lstBoss;
        }


    }
}
