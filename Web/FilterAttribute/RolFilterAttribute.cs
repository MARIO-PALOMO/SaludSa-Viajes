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
    /// Filtro de atributo para controlar el acceso a los controladores en los que el usuario tiene permiso.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RolFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ContenedorVariablesSesion sesion = filterContext.HttpContext.Session["vars"] as ContenedorVariablesSesion;

            var permiso = false;

            var controlador = filterContext.Controller.ToString();

            switch(controlador)
            {
                case "Web.Controllers.DetenerSolicitudController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_DETENER_SOLICITUDES))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.HistorialTareasController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_REPORTE_HISTORIAL_TAREAS))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.ReasignacionTareasController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_REASIGNACION_TAREAS))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.RolAdministrativoController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_ROLES_ADMINISTRATIVOS))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.RolGestorCompraController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_ROLES_GESTORES_COMPRA))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.SeguimientoProcesosController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_REPORTE_SEGUIMIENTO_PROCESOS))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.ReasignacionOriginadorCompraController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_REASIGNACION_ORIGINADOR))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.EmailPendienteController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_EMAILS_PENDIENTES))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.ImpuestoPagoController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_PAGO_IMPUESTOS))
                    {
                        permiso = true;
                    }
                    break;
                case "Web.Controllers.TipoPagoPagoController":
                    if (sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_PAGO_TIPOS_PAGO))
                    {
                        permiso = true;
                    }
                    break;
            }

            if (!permiso)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Home/Index");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
        }
    }
}