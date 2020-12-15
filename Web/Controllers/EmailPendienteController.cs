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
    /// Controlador para el módulo de emails pendientes.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class EmailPendienteController : Controller
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
        /// Proceso para obtener los emails pendientes.
        /// </summary>
        public JsonResult ObtenerItems()
        {
            List<string> validacion = new List<string>();

            List<EmailPendienteViewModel> emails = null;

            try
            {
                emails = EmailPendienteBLL.ObtenerItems(db);
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

            return Json(new
            {
                emails,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener un email pendiente.
        /// </summary>
        /// <param name="Id">Identificador del email pendiente que se desea obtener.</param>
        public JsonResult ObtenerItem(long Id)
        {
            List<string> validacion = new List<string>();

            EmailPendienteViewModel email = null;

            try
            {

                email = EmailPendienteBLL.ObtenerItem(Id, db);
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

            return Json(new
            {
                email,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para reenviar emails pendientes.
        /// </summary>
        /// <param name="EmailsEnviar">Listado con los identificadores de los emails a reenviar.</param>
        [HttpPost]
        public JsonResult Enviar(List<long> EmailsEnviar)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    EmailPendienteBLL.Enviar(EmailsEnviar, db, sesion);

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