using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
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
    /// Controlador del módulo de reasignar originador de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class ReasignacionOriginadorCompraController : Controller
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

        /// <summary>
        /// Proceso para obtener los datos que se requieren en la interfaz de reasignar originador.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<UsuarioViewModel> Usuarios = null;

            try
            {
                string[] Grupo = new string[] { "GR Jefes", "GR Gestor Compras" };
                Usuarios = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(Grupo, sesion);
            }
            catch (Exception e)
            {
                validacion.Add(e.Message);

                if (e.InnerException != null)
                {
                    if (e.InnerException.InnerException != null)
                    {
                        validacion.Add(e.InnerException.InnerException.Message);
                    }
                    else
                    {
                        validacion.Add(e.InnerException.Message);
                    }
                }
            }

            var json = Json(new
            {
                Usuarios,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de un responsable.
        /// </summary>
        /// <param name="UsuarioResponsable">Usuario del responsable actual.</param>
        public JsonResult ObtenerSolicitudesPorResponsable(string UsuarioResponsable)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<SolicitudCompraCabeceraViewModel> Solicitudes = null;

            try
            {
                Solicitudes = ReasignacionOriginadorCompraBLL.ObtenerSolicitudesPorResponsable(UsuarioResponsable, db);
            }
            catch (Exception e)
            {
                validacion.Add(e.Message);

                if (e.InnerException != null)
                {
                    if (e.InnerException.InnerException != null)
                    {
                        validacion.Add(e.InnerException.InnerException.Message);
                    }
                    else
                    {
                        validacion.Add(e.InnerException.Message);
                    }
                }
            }

            var json = Json(new
            {
                Solicitudes,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para reasignar solicitudes de compra.
        /// </summary>
        /// <param name="SolicitudesReasignar">Lista de los identificadores de las solicitudes a reasignar.</param>
        /// <param name="ResponsableNuevo">Usuario del nuevo responsable.</param>
        /// <param name="ResponsableAnterior">Usuario del responsable anterior.</param>
        [HttpPost]
        public JsonResult Reasignar(
            List<long> SolicitudesReasignar, 
            string ResponsableNuevo, 
            string ResponsableAnterior)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    ReasignacionOriginadorCompraBLL.Reasignar(SolicitudesReasignar, ResponsableNuevo, ResponsableAnterior, db, sesion);

                    db.SaveChanges();
                    dbcxtransaction.Commit();
                }
                catch (Exception e)
                {
                    dbcxtransaction.Rollback();
                    validacion.Add(e.Message);

                    if (e.InnerException != null)
                    {
                        if (e.InnerException.InnerException != null)
                        {
                            validacion.Add(e.InnerException.InnerException.Message);
                        }
                        else
                        {
                            validacion.Add(e.InnerException.Message);
                        }
                    }
                }
            }

            return Json(new
            {
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}