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
    /// Controlador para el módulo de detener solicitudes de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class DetenerSolicitudController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla inicial del módulo.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios en la interfaz de detener solicitudes de compra.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EmpresaViewModel> Empresas = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);
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
                Empresas,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para detener una solicitud de compra.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud que se desea detener.</param>
        /// <param name="EmpresaCodigo">Código de la compañía a la que pertenece la solicitud que se desea detener.</param>
        [HttpPost]
        public JsonResult Detener(
            long NumeroSolicitud, 
            string EmpresaCodigo)
        {
            List<string> validacion = new List<string>();
            sesion = Session["vars"] as ContenedorVariablesSesion;
            
            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var solicitud = db.SolicitudesCompraCabecera.Where(x => x.NumeroSolicitud == NumeroSolicitud && x.EmpresaCodigo == EmpresaCodigo).FirstOrDefault();

                    if(solicitud == null)
                    {
                        validacion.Add("La solicitud no existe.");
                    }
                    else
                    {
                        if(solicitud.NumeroSolicitud == null)
                        {
                            validacion.Add("La solicitud no ha sido enviada.");
                        }
                        else
                        {
                            var tareas = db.Tareas.Where(x => x.SolicitudCompraCabeceraId == solicitud.Id && x.EstadoId == (int)EnumEstado.ACTIVO).ToList();

                            if (tareas != null)
                            {
                                tareas.ForEach(x => x.EstadoId = (int)EnumEstado.INACTIVO);
                                tareas.ForEach(x => x.Accion = "Detenida");
                                tareas.ForEach(x => x.Observacion = "Detenida por " + sesion.usuario.Usuario);
                                tareas.ForEach(x => x.FechaProcesamiento = DateTime.Now);

                                db.SaveChanges();
                                dbcxtransaction.Commit();
                            }
                        }
                    }
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