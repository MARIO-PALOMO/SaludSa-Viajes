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
    /// Controlador del módulo de reporte de historial de tareas de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class HistorialTareasController : Controller
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
            List<TareaViewModel> Tareas = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                if(NumeroSolicitud != null && EmpresaCodigo != null && EmpresaCodigo != "")
                {
                    Tareas = HistorialTareasBLL.BuscarTareasPorSolicitud((long)NumeroSolicitud, EmpresaCodigo, db);
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
        /// Proceso para buscar la solicitud de compra de la que se desea obtener el reporte de historial de tareas.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud entrada por el usuario.</param>
        /// <param name="Empresa">Identificador de la compañía seleccionada por el usuario.</param>
        public JsonResult Buscar(
            long NumeroSolicitud, 
            string Empresa)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TareaViewModel> Tareas = null;

            try
            {
                Tareas = HistorialTareasBLL.BuscarTareasPorSolicitud(NumeroSolicitud, Empresa, db);
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

            var tarea = db.Tareas.Find(tareaId);

            switch (tipo)
            {
                case (int)EnumTipoTarea.CREACION_SOLICITUD:
                    return View("~/Views/Tarea/CreacionSolicitudEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO:
                    return View("~/Views/Tarea/JefeInmediatoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA:
                    return View("~/Views/Tarea/GestorCompraEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA:
                    return View("~/Views/Tarea/JefeCompraEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO:
                    return View("~/Views/Tarea/JefeCompraProductoOtro.cshtml");
                case (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO:
                    return View("~/Views/Tarea/VerificacionPresupuestoEdit.cshtml");
                case (int)EnumTipoTarea.RECEPCION:
                    return View("~/Views/Tarea/RecepcionEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_DESEMBOLSO:
                    return View("~/Views/Tarea/AprobacionDesembolsoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_ANULACION_RECEPCION:
                    return View("~/Views/Tarea/AprobarAnulacionRecepcionEdit.cshtml");
                case (int)EnumTipoTarea.ADJUNTAR_FACTURA:
                    return View("~/Views/Tarea/AdjuntarFacturaEdit.cshtml");
                case (int)EnumTipoTarea.CONTABILIZAR_RECEPCION:
                    return View("~/Views/Tarea/ContabilizarRecepcionEdit.cshtml");
                case (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE:
                    return View("~/Views/Tarea/DevueltaSolicitanteEdit.cshtml");
                case (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO:
                    return View("~/Views/Tarea/RetornoJefeInmediatoEdit.cshtml");

                case (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA;
                    return View("~/Views/Tarea/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA;
                    return View("~/Views/Tarea/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA;
                    return View("~/Views/Tarea/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO;
                    return View("~/Views/Tarea/AprobacionPorMontoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL;
                    return View("~/Views/Tarea/AprobacionPorMontoEdit.cshtml");

                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA;
                    return View("~/Views/Tarea/AprobacionFueraPresupuestoEdit.cshtml");
                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO;
                    return View("~/Views/Tarea/AprobacionFueraPresupuestoEdit.cshtml");
                default:
                    return View();
            }
        }
    }
}