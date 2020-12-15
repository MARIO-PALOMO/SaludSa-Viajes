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
    /// Controlador del módulo de reporte de historial de tareas de solicitudes de pago.
    /// </summary>
    [SesionFilter]
    public class HistorialTareasPagoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio del módulo.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de solicitud para precargar.</param>
        /// <param name="EmpresaCodigo">Identificador de la compañía de la solicitud para precargar.</param>
        public ActionResult Index(
            long? NumeroSolicitud, 
            string EmpresaCodigo = "")
        {
            ViewBag.EmpresaCodigo = EmpresaCodigo;
            ViewBag.NumeroSolicitud = NumeroSolicitud;
            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios en la interfaz de reprote de historial de tareas.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de solicitud para precargar.</param>
        /// <param name="EmpresaCodigo">Identificador de la compañía de la solicitud para precargar.</param>
        public JsonResult ObtenerMetadatos(
            long? NumeroSolicitud, 
            string EmpresaCodigo = "")
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EmpresaViewModel> Empresas = null;
            List<TareaPagoViewModel> Tareas = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                if (NumeroSolicitud != null && EmpresaCodigo != null && EmpresaCodigo != "")
                {
                    Tareas = HistorialTareasBLL.BuscarTareasPagoPorSolicitud((long)NumeroSolicitud, EmpresaCodigo, db);
                }
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
                Tareas,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para buscar la solicitud de pago de la que se desea obtener el reporte de historial de tareas.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud entrada por el usuario.</param>
        /// <param name="Empresa">Identificador de la compañía seleccionada por el usuario.</param>
        public JsonResult Buscar(
            long NumeroSolicitud, 
            string Empresa)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TareaPagoViewModel> Tareas = null;

            try
            {
                Tareas = HistorialTareasBLL.BuscarTareasPagoPorSolicitud(NumeroSolicitud, Empresa, db);
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
        /// Proceso para visualizar una tarea.
        /// </summary>
        /// <param name="tipo">Tipo de la tarea a visualizar.</param>
        /// <param name="tareaId">Identificador de la tarea que se desea visualizar.</param>
        /// <param name="solicitudId">Identificador de la solicitud a la que pertenece la tarea que se desea visualizar.</param>
        public ActionResult ShowTarea(
            int tipo, 
            long tareaId, 
            long solicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            ViewBag.Accion = "show";
            ViewBag.Id = solicitudId;
            ViewBag.tareaId = tareaId;

            var tarea = db.TareasPago.Find(tareaId);

            switch (tipo)
            {
                case (int)EnumTipoTareaPago.CREACION_SOLICITUD:
                    return View("~/Views/TareaPago/CreacionSolicitudEdit.cshtml");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA;
                    return View("~/Views/TareaPago/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA;
                    return View("~/Views/TareaPago/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA;
                    return View("~/Views/TareaPago/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO;
                    return View("~/Views/TareaPago/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL;
                    return View("~/Views/TareaPago/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTareaPago.CONTABILIZAR:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.CONTABILIZAR;
                    return View("~/Views/TareaPago/ContabilizacionEdit.cshtml");

                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE;
                    return View("~/Views/TareaPago/DevueltaSolicitanteJefeEdit.cshtml");
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD;
                    return View("~/Views/TareaPago/DevueltaSolicitanteContabilidadEdit.cshtml");

                default:
                    return View();
            }
        }
    }
}