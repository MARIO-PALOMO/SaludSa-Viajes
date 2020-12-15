using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace SaludSA.Controllers
{
    /// <summary>
    /// Controlador del módulo de inicio.
    /// </summary>
    [SesionFilter]
    public class HomeController : Controller
    {
        /// <summary>
        /// Pantalla inicial del módulo.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}