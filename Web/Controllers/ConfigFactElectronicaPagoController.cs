using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Rest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para el módulo de configuraciones de factura electrónica en pagos.
    /// </summary>
    [SesionFilter]
    public class ConfigFactElectronicaPagoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio del módulo.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}