using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
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
    /// Controlador del módulo de comprobantes electrónicos.
    /// </summary>
    [SesionFilter]
    public class ComprobanteElectronicoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Proceso para buscar comprobantes electrónicos.
        /// </summary>
        /// <param name="ruc">Filtro para el RUC del comprobante a buscar.</param>
        /// <param name="establecimiento">Filtro para el establecimiento del comprobante a buscar.</param>
        /// <param name="puntoEmision">Filtro para el punto de emisión del comprobante a buscar.</param>
        /// <param name="secuencial">Filtro para el secuencial del comprobante a buscar.</param>
        /// <param name="fechaInicio">Filtro para la fecha de inicio del comprobante a buscar.</param>
        /// <param name="fechaFin">Filtro para la fecha de fin del comprobante a buscar.</param>
        [HttpPost]
        public JsonResult Buscar(
            string ruc, 
            string establecimiento, 
            string puntoEmision, 
            string secuencial, 
            string fechaInicio,
            string fechaFin)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ComprobanteElectronicoViewModel> comprobantes = null;

            try
            {
                comprobantes = ComprobanteElectronicoBLL.Buscar(sesion, ruc, establecimiento, puntoEmision, secuencial, fechaInicio, fechaFin);
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
                comprobantes,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para adicionar comprobantes electrónicos.
        /// </summary>
        /// <param name="comprobantesAdd">Lista de comprobante para adicionar.</param>
        /// <param name="RecepcionId">Identificación de la recepción a la que se adicionarán los comprobantes electrónicos.</param>
        [HttpPost]
        public JsonResult Adicionar(
            List<ComprobanteElectronico> comprobantesAdd, 
            long RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ComprobanteElectronicoViewModel> comprobantes = null;

            try
            {
                foreach(var comprobante in comprobantesAdd)
                {
                    ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                    {
                        ruc = comprobante.ruc,
                        establecimiento = comprobante.establecimiento,
                        puntoEmision = comprobante.puntoEmision,
                        secuencial = comprobante.secuencial,
                        tipoDocumento = comprobante.tipoDocumento,
                        estadoDocumento = 5,
                        codigoSistema = 3
                    };

                    TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);

                    comprobante.fechaEmision = ComprobanteElectronicoBLL.ObtenerFechaEmision(sesion, comprobante.claveAcceso);

                    comprobante.RecepcionId = RecepcionId;
                    db.ComprobantesElectronicos.Add(comprobante);
                }

                db.SaveChanges();

                comprobantes = ComprobanteElectronicoBLL.ObtenerComprobantesPorRecepcion(RecepcionId, db);
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
                comprobantes,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener la fecha de emisión de un comprobante electrónico.
        /// </summary>
        /// <param name="claveAcceso">Clave de acceso del comrpobante del que se desea obtener la fecha de emisión.</param>
        [HttpGet]
        public JsonResult ObtenerFechaEmision(string claveAcceso)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            string fechaEmision = null;

            try
            {
                fechaEmision = ComprobanteElectronicoBLL.ObtenerFechaEmision(sesion, claveAcceso);
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
                fechaEmision,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para eliminar un comprobante electrónico.
        /// </summary>
        /// <param name="comprobanteId">Identificador del comprobante electrónico que se desea eliminar.</param>
        /// <param name="RecepcionId">Identificador de la recepción a la que pertenece el comrpobante electrónico que se desea eliminar.</param>
        [HttpPost]
        public JsonResult Eliminar(
            long comprobanteId, 
            long RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ComprobanteElectronicoViewModel> comprobantes = null;

            try
            {
                var comprobante = db.ComprobantesElectronicos.Find(comprobanteId);

                ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                {
                    ruc = comprobante.ruc,
                    establecimiento = comprobante.establecimiento,
                    puntoEmision = comprobante.puntoEmision,
                    secuencial = comprobante.secuencial,
                    tipoDocumento = comprobante.tipoDocumento,
                    estadoDocumento = 1,
                    codigoSistema = 3
                };

                TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);

                comprobante.EstadoId = (int)EnumEstado.INACTIVO;

                db.SaveChanges();

                comprobantes = ComprobanteElectronicoBLL.ObtenerComprobantesPorRecepcion(RecepcionId, db);
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
                comprobantes,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }
    }
}