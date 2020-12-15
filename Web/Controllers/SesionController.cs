using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la sesión activa.
    /// </summary>
    [SesionFilter]
    public class SesionController : Controller
    {
        ContenedorVariablesSesion sesion;

        /// <summary>
        /// Proceso para obtener la sesión activa.
        /// </summary>
        public JsonResult ObtenerSesion()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;

            return Json(new {
                sesion.usuario
            }, JsonRequestBehavior.AllowGet);
        }
    }
}