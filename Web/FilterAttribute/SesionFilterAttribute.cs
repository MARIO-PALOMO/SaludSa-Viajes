using Common.Utilities;
using Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.FilterAttribute
{
    /// <summary>
    /// Filtro de atributo para controlar la sesión (Caducidad, refrescamiento y cierre).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SesionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ContenedorVariablesSesion sesion = filterContext.HttpContext.Session["vars"] as ContenedorVariablesSesion;

            if (sesion == null 
                || sesion.token == null 
                || sesion.usuario == null
                || sesion.SistemaOperativo == null
                || sesion.DispositivoNavegador == null
                || sesion.DireccionIP == null
                || sesion.UrlServicios == null
                || sesion.CodigoAplicacion == null
                || sesion.ClientId == null
                || sesion.UsuarioSistema == null
                || sesion.ClaveSistema == null
                || sesion.SistemaOperativo == string.Empty 
                || sesion.DispositivoNavegador == string.Empty 
                || sesion.DireccionIP == string.Empty
                || sesion.UrlServicios == string.Empty
                || sesion.CodigoAplicacion == string.Empty
                || sesion.ClientId == string.Empty
                || sesion.UsuarioSistema == string.Empty
                || sesion.ClaveSistema == string.Empty)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Account/Login");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
            else if (sesion.token.expires <= DateTime.Now)
            {
                var token = AutenticationRCL.RefrescarToken(sesion.UrlServicios, sesion.ClientId, sesion.token.refresh_token, sesion.OrigenLog);

                if (token == null || token.error != null)
                {
                    token = AutenticationRCL.ObtenerToken(sesion.UrlServicios, sesion.UsuarioSistema, sesion.ClaveSistema, sesion.ClientId, sesion.OrigenLog);

                    if (token == null || token.error != null)
                    {
                        var url = new UrlHelper(filterContext.RequestContext);
                        var loginUrl = url.Content("~/Account/Login");
                        filterContext.HttpContext.Response.Redirect(loginUrl, true);
                    }
                    else
                    {
                        sesion.token = token;
                    }
                }
                else
                {
                    sesion.token = token;
                }
            }
        }
    }
}