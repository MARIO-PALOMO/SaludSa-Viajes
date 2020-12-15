using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador del módulo de reasignación de tareas de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class ReasignacionTareasController : Controller
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
        /// Proceso para obtener los datos requeridos en la interfaz de reasignación de tareas.
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
        /// Obtener las tareas de un responsable.
        /// </summary>
        /// <param name="UsuarioResponsable">Usuario del responsable.</param>
        public JsonResult ObtenerTareasPorResponsable(string UsuarioResponsable)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TareaViewModel> Tareas = null;

            try
            {
                Tareas = ReasignacionTareasBLL.ObtenerTareasPorResponsable(UsuarioResponsable, db);
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
                Tareas,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Reasignar tareas.
        /// </summary>
        /// <param name="TareasReasignar">Listado de los identificadores de las tareas a reasignar.</param>
        /// <param name="ResponsableNuevo">Usuario del responsable nuevo.</param>
        /// <param name="ResponsableAnterior">Usuario del responsable anterior.</param>
        [HttpPost]
        public JsonResult Reasignar(
            List<long> TareasReasignar,
            string ResponsableNuevo, 
            string ResponsableAnterior)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    ReasignacionTareasBLL.Reasignar(TareasReasignar, ResponsableNuevo, ResponsableAnterior, db, sesion);

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