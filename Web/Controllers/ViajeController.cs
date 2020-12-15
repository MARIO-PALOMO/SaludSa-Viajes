using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class ViajeController : Controller
    {
        // GET: Viaje
        public ActionResult Index()
        {

            var sesion = Session["vars"] as ContenedorVariablesSesion;

            return Redirect("https://localhost:44355/client/reservation/list/"+ sesion.usuario.Usuario);
        }
    }
}