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
    /// Controlador del módulo de reportes de seguimiento de procesos de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class SeguimientoProcesosController : Controller
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
        /// Proceso para obtener los datos requeridos en la interfaz de reportes de seguimiento de procesos.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EmpresaViewModel> Empresas = null;
            List<UsuarioViewModel> Originadores = null;
            EmpresaViewModel EmpresaActual = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                if(Empresas.Count() > 0)
                {
                    EmpresaActual = Empresas.First();
                }

                Originadores = SeguimientoProcesosBLL.ObtenerOriginadores(EmpresaActual.Codigo, db);
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
                Originadores,
                EmpresaActual,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener los originadores de una compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía de la que se desean obtener los originadores.</param>
        public JsonResult ObtenerOriginadores(string EmpresaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<UsuarioViewModel> Originadores = null;

            try
            {
                Originadores = SeguimientoProcesosBLL.ObtenerOriginadores(EmpresaCodigo, db);
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
                Originadores,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de compra.
        /// </summary>
        /// <param name="EmpresaCodigo">Filtro de compañía.</param>
        /// <param name="Originadores">Filtro de originadores.</param>
        /// <param name="FechaDesde">Filtro de fecha desde.</param>
        /// <param name="FechaHasta">Filtro de fecha hasta.</param>
        public JsonResult ObtenerSolicitudes(
            string EmpresaCodigo, 
            List<string> Originadores, 
            DateTime FechaDesde,
            DateTime FechaHasta)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<SolicitudCompraCabeceraViewModel> Solicitudes = null;

            try
            {
                Solicitudes = SeguimientoProcesosBLL.ObtenerSolicitudes(EmpresaCodigo, Originadores, FechaDesde, FechaHasta, db);
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
    }
}