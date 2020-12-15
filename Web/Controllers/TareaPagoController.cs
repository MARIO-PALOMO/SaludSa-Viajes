using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador del módulo de tareas de solicitudes de pago.
    /// </summary>
    [SesionFilter]
    public class TareaPagoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio del módulo.
        /// </summary>
        /// <param name="mensaje">Mensaje de notificación a visualizar.</param>
        /// <param name="numeroDiario">Número de diario creado a visualizar.</param>
        public ActionResult Index(
            string mensaje = "", 
            string numeroDiario = "")
        {
            ViewBag.Mensaje = mensaje;
            ViewBag.NumeroDiario = numeroDiario;
            return View();
        }

        /// <summary>
        /// Proceso para procesar una tarea.
        /// </summary>
        /// <param name="tipo">Tipo de la tarea.</param>
        /// <param name="tareaId">Identificador de la tarea.</param>
        /// <param name="solicitudId">Identificador de la solicitud a la que pertenece la tarea.</param>
        public ActionResult Edit(
            int tipo,
            long tareaId, 
            long solicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            ViewBag.Accion = "detail";
            ViewBag.Id = solicitudId;
            ViewBag.tareaId = tareaId;

            var tarea = db.TareasPago.Find(tareaId);

            if(tarea.EstadoId == (int)EnumEstado.INACTIVO || tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                return Redirect("Index");
            }

            switch (tipo)
            {
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTareaPago.CONTABILIZAR:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.CONTABILIZAR;
                    return View("ContabilizacionEdit");
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE;
                    return View("DevueltaSolicitanteJefeEdit");
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD:
                    ViewBag.Tipo = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD;
                    return View("DevueltaSolicitanteContabilidadEdit");
                default:
                    return View();
            }            
        }

        /// <summary>
        /// Proceso para buscar las tareas del usuario autenticado.
        /// </summary>
        public JsonResult Buscar()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TareaPagoViewModel> tareas = null;

            try
            {
                tareas = TareaPagoBLL.ObtenerTareas(sesion, db);
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
                tareas,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación de jefe inmediato.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionJefeInmediatoEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaPagoViewModel> tareas = null;

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Aprobar")
                        {
                            var AprobacionSubgerenteAreaUsuario = string.Empty;
                            var AprobacionGerenteAreaUsuario = string.Empty;

                            if (tarea.FacturaCabeceraPagoId != null)
                            {
                                AprobacionSubgerenteAreaUsuario = tarea.FacturaCabeceraPago.AprobacionSubgerenteArea;
                                AprobacionGerenteAreaUsuario = tarea.FacturaCabeceraPago.AprobacionGerenteArea;
                            }
                            else
                            {
                                AprobacionSubgerenteAreaUsuario = Solicitud.AprobacionSubgerenteArea;
                                AprobacionGerenteAreaUsuario = Solicitud.AprobacionGerenteArea;
                            }

                            if (AprobacionSubgerenteAreaUsuario == "noaplica_0123" && Solicitud.MontoTotal >= 2500)
                            {
                                if (AprobacionGerenteAreaUsuario == null)
                                {
                                    throw new Exception("No se definió un Gerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else if (Solicitud.MontoTotal >= 2500)
                            {
                                if (AprobacionSubgerenteAreaUsuario == null)
                                {
                                    throw new Exception("No se definió un Subgerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Devolver")
                        {
                            if(tarea.FacturaCabeceraPagoId != null)
                            {
                                TareaPagoBLL.CrearTarea(tarea.SolicitudPagoCabecera, sesion, (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE_CONTABILIDAD, db, tarea.Id);
                            }
                            else
                            {
                                TareaPagoBLL.CrearTarea(tarea.SolicitudPagoCabecera, sesion, (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudPagoCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudPagoCabecera).State = EntityState.Modified;

                            foreach(var fac in Solicitud.Facturas)
                            {
                                if(fac.Tipo == "Electrónica")
                                {
                                    RestablecerFacturaElectronica(fac);
                                }
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudPagoCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudPagoCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
                        }

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudPagoBLL.ObtenerTareas((long)tarea.SolicitudPagoCabeceraId, db);
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
                }
            }

            return Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación por montos para subgerente de área.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionPorMontoSubgerenteAreaEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaPagoViewModel> tareas = null;

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Aprobar")
                        {
                            if (Solicitud.MontoTotal >= 5000)
                            {
                                var AprobacionGerenteAreaUsuario = string.Empty;

                                if (tarea.FacturaCabeceraPagoId != null)
                                {
                                    AprobacionGerenteAreaUsuario = tarea.FacturaCabeceraPago.AprobacionGerenteArea;
                                }
                                else
                                {
                                    AprobacionGerenteAreaUsuario = Solicitud.AprobacionGerenteArea;
                                }

                                if (AprobacionGerenteAreaUsuario == null)
                                {
                                    throw new Exception("No se definió un Gerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            foreach (var fac in Solicitud.Facturas)
                            {
                                if (fac.Tipo == "Electrónica")
                                {
                                    RestablecerFacturaElectronica(fac);
                                }
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
                        }

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudPagoBLL.ObtenerTareas((long)tarea.SolicitudPagoCabeceraId, db);
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
                }
            }

            return Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación por montos para gerente de área.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionPorMontoGerenteAreaEdit(
            long TareaId, 
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaPagoViewModel> tareas = null;

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Aprobar")
                        {
                            if (Solicitud.MontoTotal >= 10000)
                            {
                                var AprobacionVicePresidenteFinancieroUsuario = string.Empty;

                                if (tarea.FacturaCabeceraPagoId != null)
                                {
                                    AprobacionVicePresidenteFinancieroUsuario = tarea.FacturaCabeceraPago.AprobacionVicePresidenteFinanciero;
                                }
                                else
                                {
                                    AprobacionVicePresidenteFinancieroUsuario = Solicitud.AprobacionVicePresidenteFinanciero;
                                }

                                if (AprobacionVicePresidenteFinancieroUsuario == null)
                                {
                                    throw new Exception("No se definió un Vicepresidente Financiero en la solicitud.");
                                }
                                else
                                {
                                    TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            foreach (var fac in Solicitud.Facturas)
                            {
                                if (fac.Tipo == "Electrónica")
                                {
                                    RestablecerFacturaElectronica(fac);
                                }
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
                        }

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudPagoBLL.ObtenerTareas((long)tarea.SolicitudPagoCabeceraId, db);
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
                }
            }

            return Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación por montos para vicepresidente financiero.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionPorMontoVicepresidenteFinancieroEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaPagoViewModel> tareas = null;

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Aprobar")
                        {
                            if (Solicitud.MontoTotal >= 120000)
                            {
                                var AprobacionGerenteGeneralUsuario = string.Empty;

                                if (tarea.FacturaCabeceraPagoId != null)
                                {
                                    AprobacionGerenteGeneralUsuario = tarea.FacturaCabeceraPago.AprobacionGerenteGeneral;
                                }
                                else
                                {
                                    AprobacionGerenteGeneralUsuario = Solicitud.AprobacionGerenteGeneral;
                                }

                                if (AprobacionGerenteGeneralUsuario == null)
                                {
                                    throw new Exception("No se definió un Gerente General en la solicitud.");
                                }
                                else
                                {
                                    TareaPagoBLL.CrearAprobacionPorMontoGerenteGeneral(Solicitud, sesion, db, string.Empty, 0, tarea.Id, AppDomain.CurrentDomain.BaseDirectory);
                                }
                            }
                            else
                            {
                                TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            foreach (var fac in Solicitud.Facturas)
                            {
                                if (fac.Tipo == "Electrónica")
                                {
                                    RestablecerFacturaElectronica(fac);
                                }
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
                        }

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudPagoBLL.ObtenerTareas((long)tarea.SolicitudPagoCabeceraId, db);
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
                }
            }

            return Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación por montos para gerente general.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionPorMontoGerenteGeneralEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaPagoViewModel> tareas = null;

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Aprobar")
                        {
                            TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudPagoCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudPagoCabecera).State = EntityState.Modified;

                            foreach (var fac in Solicitud.Facturas)
                            {
                                if (fac.Tipo == "Electrónica")
                                {
                                    RestablecerFacturaElectronica(fac);
                                }
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
                        }

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudPagoBLL.ObtenerTareas((long)tarea.SolicitudPagoCabeceraId, db);
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
                }
            }

            return Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para devolución a solicitante desde jefe inmediato.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Cabecera">Objeto serializado como un string que representa a la solicitud editada por el usuario.</param>
        /// <param name="FacturasEliminar">Facturas a eliminar.</param>
        /// <param name="FacturasEliminarAdjunto">Adjuntos a eliminar en las facturas.</param>
        [HttpPost]
        public JsonResult TareaDevueltaSolicitanteJefeEdit(
            long TareaId, 
            string Cabecera, 
            string FacturasEliminar, 
            string FacturasEliminarAdjunto)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                tarea.Accion = "Editada";
                tarea.Observacion = "";
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;

                db.Entry(tarea).State = EntityState.Modified;

                SolicitudPagoCabecera Obj = null;
                SolicitudPagoCabecera CabeceraTem = null;
                long[] FacturasEliminarArr = null;
                long[] FacturasEliminarAdjuntoArr = null;

                try
                {
                    var format = "dd/MM/yyyy"; // your datetime format
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                    Obj = JsonConvert.DeserializeObject<SolicitudPagoCabecera>(Cabecera, dateTimeConverter);
                    FacturasEliminarArr = JsonConvert.DeserializeObject<long[]>(FacturasEliminar);
                    FacturasEliminarAdjuntoArr = JsonConvert.DeserializeObject<long[]>(FacturasEliminarAdjunto);
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

                if (validacion.Count() == 0 && ModelState.IsValid && TryValidateModel(Obj))
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            CabeceraTem = db.SolicitudesPagoCabecera.Find(Obj.Id);

                            CabeceraTem.EmpresaCodigo = Obj.EmpresaCodigo;
                            CabeceraTem.EmpresaNombre = Obj.EmpresaNombre;
                            CabeceraTem.AprobacionJefeArea = Obj.AprobacionJefeArea;
                            CabeceraTem.AprobacionSubgerenteArea = Obj.AprobacionSubgerenteArea;
                            CabeceraTem.AprobacionGerenteArea = Obj.AprobacionGerenteArea;
                            CabeceraTem.AprobacionVicePresidenteFinanciero = Obj.AprobacionVicePresidenteFinanciero;
                            CabeceraTem.AprobacionGerenteGeneral = Obj.AprobacionGerenteGeneral;
                            CabeceraTem.NombreCorto = Obj.NombreCorto;
                            CabeceraTem.Observacion = Obj.Observacion;
                            CabeceraTem.BeneficiarioIdentificacion = Obj.BeneficiarioIdentificacion;
                            CabeceraTem.BeneficiarioNombre = Obj.BeneficiarioNombre;
                            CabeceraTem.BeneficiarioTipoIdentificacion = Obj.BeneficiarioTipoIdentificacion;
                            CabeceraTem.MontoTotal = Obj.MontoTotal;

                            if (FacturasEliminarArr != null)
                            {
                                foreach (var i in FacturasEliminarArr)
                                {
                                    var facturaEliminar = db.FacturaCabecerasPago.Find(i);

                                    if (facturaEliminar.Tipo == "Física")
                                    {
                                        this.DeshabilitarAdjunto(CabeceraTem.Id, i);
                                    }
                                    else if (facturaEliminar.Tipo == "Electrónica")
                                    {
                                        //cambiar el estado de las facturas electrónicas
                                        ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                        {
                                            ruc = facturaEliminar.ComprobanteElectronico.ruc,
                                            establecimiento = facturaEliminar.ComprobanteElectronico.establecimiento,
                                            puntoEmision = facturaEliminar.ComprobanteElectronico.puntoEmision,
                                            secuencial = facturaEliminar.ComprobanteElectronico.secuencial,
                                            tipoDocumento = facturaEliminar.ComprobanteElectronico.tipoDocumento,
                                            estadoDocumento = 1,
                                            codigoSistema = 3
                                        };

                                        TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                    }

                                    var detallesEliminar = db.FacturaDetallesPago.Where(x => x.FacturaCabeceraPagoId == i).ToList();

                                    foreach (var detElim in detallesEliminar)
                                    {
                                        var distribucionesEliminar = db.FacturaDetallePagoDistribuciones.Where(x => x.FacturaDetallePagoId == detElim.Id).ToList();

                                        foreach (var distElim in distribucionesEliminar)
                                        {
                                            db.FacturaDetallePagoDistribuciones.Remove(distElim);
                                        }

                                        db.FacturaDetallesPago.Remove(detElim);
                                    }

                                    db.FacturaCabecerasPago.Remove(facturaEliminar);
                                }
                            }

                            if (FacturasEliminarAdjuntoArr != null)
                            {
                                foreach (var i in FacturasEliminarAdjuntoArr)
                                {
                                    var facturaEliminar = db.FacturaCabecerasPago.Find(i);

                                    if (facturaEliminar.Tipo == "Física")
                                    {
                                        this.DeshabilitarAdjunto(CabeceraTem.Id, i);
                                    }
                                    else if (facturaEliminar.Tipo == "Electrónica")
                                    {
                                        //cambiar el estado de las facturas electrónicas
                                        ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                        {
                                            ruc = facturaEliminar.ComprobanteElectronico.ruc,
                                            establecimiento = facturaEliminar.ComprobanteElectronico.establecimiento,
                                            puntoEmision = facturaEliminar.ComprobanteElectronico.puntoEmision,
                                            secuencial = facturaEliminar.ComprobanteElectronico.secuencial,
                                            tipoDocumento = facturaEliminar.ComprobanteElectronico.tipoDocumento,
                                            estadoDocumento = 1,
                                            codigoSistema = 3
                                        };

                                        TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                    }
                                }
                            }

                            /***/
                            foreach (var Factura in Obj.Facturas)
                            {
                                if (Factura.Id > 0)
                                {
                                    var FacturaTem = db.FacturaCabecerasPago.Find(Factura.Id);

                                    FacturaTem.NoFactura = Factura.NoFactura;
                                    FacturaTem.Concepto = Factura.Concepto;
                                    FacturaTem.FechaEmision = Factura.FechaEmision;
                                    FacturaTem.FechaVencimiento = Factura.FechaVencimiento;
                                    FacturaTem.Total = Factura.Total;
                                    FacturaTem.TipoPagoId = Factura.TipoPagoId;
                                    FacturaTem.NoAutorizacion = Factura.NoAutorizacion;
                                    FacturaTem.Tipo = Factura.Tipo;

                                    if (FacturaTem.Tipo == "Electrónica")
                                    {
                                        if (FacturasEliminarAdjuntoArr.Contains(Factura.Id))
                                        {
                                            var docElectTem = db.ComprobantesElectronicos.Find(FacturaTem.ComprobanteElectronicoId);
                                            docElectTem = Factura.ComprobanteElectronico;
                                            docElectTem.Id = (long)FacturaTem.ComprobanteElectronicoId;

                                            db.Entry(FacturaTem).State = EntityState.Modified;

                                            //cambiar el estado de las facturas electrónicas
                                            ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                            {
                                                ruc = Factura.ComprobanteElectronico.ruc,
                                                establecimiento = Factura.ComprobanteElectronico.establecimiento,
                                                puntoEmision = Factura.ComprobanteElectronico.puntoEmision,
                                                secuencial = Factura.ComprobanteElectronico.secuencial,
                                                tipoDocumento = Factura.ComprobanteElectronico.tipoDocumento,
                                                estadoDocumento = 5,
                                                codigoSistema = 3
                                            };

                                            TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                        }
                                    }

                                    var detalles = FacturaTem.FacturaDetallesPago;
                                    for (int i = 0; i < detalles.Count(); i++)
                                    {
                                        var distribuciones = detalles.ElementAt(i).Distribuciones;
                                        for (int j = 0; j < distribuciones.Count(); j++)
                                        {
                                            db.FacturaDetallePagoDistribuciones.Remove(distribuciones.ElementAt(j));
                                            j--;
                                        }

                                        db.FacturaDetallesPago.Remove(detalles.ElementAt(i));
                                        i--;
                                    }

                                    foreach (var Det in Factura.FacturaDetallesPago)
                                    {
                                        Det.FacturaCabeceraPagoId = Factura.Id;
                                        db.FacturaDetallesPago.Add(Det);
                                    }

                                    db.Entry(FacturaTem).State = EntityState.Modified;
                                }
                                else
                                {
                                    Factura.SolicitudPagoCabeceraId = CabeceraTem.Id;
                                    db.FacturaCabecerasPago.Add(Factura);

                                    if (Factura.Tipo == "Electrónica")
                                    {
                                        //cambiar el estado de las facturas electrónicas
                                        ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                        {
                                            ruc = Factura.ComprobanteElectronico.ruc,
                                            establecimiento = Factura.ComprobanteElectronico.establecimiento,
                                            puntoEmision = Factura.ComprobanteElectronico.puntoEmision,
                                            secuencial = Factura.ComprobanteElectronico.secuencial,
                                            tipoDocumento = Factura.ComprobanteElectronico.tipoDocumento,
                                            estadoDocumento = 5,
                                            codigoSistema = 3
                                        };

                                        TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                    }
                                }
                            }
                            /***/

                            db.Entry(CabeceraTem).State = EntityState.Modified;
                            db.SaveChanges();

                            /***/
                            var Files = Request.Files;

                            if (Files != null)
                            {
                                int loop = 1;
                                string uuid = System.Guid.NewGuid().ToString();

                                foreach (string key in Files)
                                {
                                    HttpPostedFileBase postedFile = Request.Files[key];

                                    byte[] BinaryFile = null;

                                    using (BinaryReader b = new BinaryReader(postedFile.InputStream))
                                    {
                                        BinaryFile = b.ReadBytes(postedFile.ContentLength);
                                    }

                                    AdjuntoViewModel adjunto = new AdjuntoViewModel()
                                    {
                                        ContenidoArchivo = BinaryFile,
                                        Propiedades = new List<PropiedadAdjuntoViewModel>()
                                    };

                                    long idFac = 0;

                                    foreach (var fac in Obj.Facturas)
                                    {
                                        if (fac.AdjuntoName == postedFile.FileName)
                                        {
                                            idFac = fac.Id;
                                            break;
                                        }
                                    }

                                    if (idFac == 0)
                                    {
                                        throw new Exception("No se ha podido emparejar las facturas con los archivos. " + postedFile.FileName);
                                    }

                                    adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = Obj.Id.ToString() + "-" + idFac.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "factura-pago"
                                    },
                                    new PropiedadAdjuntoViewModel() { // IdAdjunto
                                        Codigo = "1087",
                                        Valor = loop + "-" + uuid
                                    },
                                    new PropiedadAdjuntoViewModel() { // Estado
                                        Codigo = "1050",
                                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                                    }
                                };

                                    SolicitudPagoBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                                    loop++;
                                }
                            }
                            /***/

                            TareaPagoBLL.CrearTarea(CabeceraTem, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA, db, 0);

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
                }
            }

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            return Json(new
            {
                error,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para devolución a solicitante desde contabilidad.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Factura">Factura editada por el usuario.</param>
        /// <param name="FacturasEliminarAdjunto">Adjuntos de la factura a eliminar.</param>
        /// <param name="AprobacionJefeArea">Nombre del usuario aprobador (Jefe inmediato).</param>
        /// <param name="AprobacionSubgerenteArea">Nombre del usuario aprobador (Subgerente de área).</param>
        /// <param name="AprobacionGerenteArea">Nombre del usuario aprobador (Gerente de área).</param>
        /// <param name="AprobacionVicePresidenteFinanciero">Nombre del usuario aprobador (Vicepresidente financiero).</param>
        /// <param name="AprobacionGerenteGeneral">Nombre del usuario aprobador (Gerente general).</param>
        public JsonResult TareaDevueltaSolicitanteContabilidadEdit(
            long TareaId, 
            string Factura, 
            string FacturasEliminarAdjunto,
            string AprobacionJefeArea,
            string AprobacionSubgerenteArea,
            string AprobacionGerenteArea,
            string AprobacionVicePresidenteFinanciero,
            string AprobacionGerenteGeneral)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            var tarea = db.TareasPago.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                tarea.Accion = "Editada";
                tarea.Observacion = "";
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;

                db.Entry(tarea).State = EntityState.Modified;

                FacturaCabeceraPago Obj = null;
                FacturaCabeceraPago FacturaTem = null;
                long[] FacturasEliminarAdjuntoArr = null;

                try
                {
                    var format = "dd/MM/yyyy"; // your datetime format
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                    Obj = JsonConvert.DeserializeObject<FacturaCabeceraPago>(Factura, dateTimeConverter);
                    FacturasEliminarAdjuntoArr = JsonConvert.DeserializeObject<long[]>(FacturasEliminarAdjunto);
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

                if (validacion.Count() == 0 && ModelState.IsValid && TryValidateModel(Obj))
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            /****/
                            FacturaTem = db.FacturaCabecerasPago.Find(Obj.Id);

                            FacturaTem.NoFactura = Obj.NoFactura;
                            FacturaTem.Concepto = Obj.Concepto;
                            FacturaTem.FechaEmision = Obj.FechaEmision;
                            FacturaTem.FechaVencimiento = Obj.FechaVencimiento;
                            FacturaTem.Total = Obj.Total;
                            //FacturaTem.TipoPagoId = Obj.TipoPagoId;
                            FacturaTem.NoAutorizacion = Obj.NoAutorizacion;
                            FacturaTem.Tipo = Obj.Tipo;

                            FacturaTem.AprobacionJefeArea = AprobacionJefeArea;
                            FacturaTem.AprobacionSubgerenteArea = AprobacionSubgerenteArea;
                            FacturaTem.AprobacionGerenteArea = AprobacionGerenteArea;
                            FacturaTem.AprobacionVicePresidenteFinanciero = AprobacionVicePresidenteFinanciero;
                            FacturaTem.AprobacionGerenteGeneral = AprobacionGerenteGeneral;

                            if (FacturaTem.Tipo == "Electrónica")
                            {
                                if (FacturasEliminarAdjuntoArr.Contains(Obj.Id))
                                {
                                    var docElectTem = db.ComprobantesElectronicos.Find(FacturaTem.ComprobanteElectronicoId);
                                    docElectTem = Obj.ComprobanteElectronico;
                                    docElectTem.Id = (long)FacturaTem.ComprobanteElectronicoId;

                                    db.Entry(FacturaTem).State = EntityState.Modified;

                                    //cambiar el estado de las facturas electrónicas
                                    ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                    {
                                        ruc = Obj.ComprobanteElectronico.ruc,
                                        establecimiento = Obj.ComprobanteElectronico.establecimiento,
                                        puntoEmision = Obj.ComprobanteElectronico.puntoEmision,
                                        secuencial = Obj.ComprobanteElectronico.secuencial,
                                        tipoDocumento = Obj.ComprobanteElectronico.tipoDocumento,
                                        estadoDocumento = 5,
                                        codigoSistema = 3
                                    };

                                    TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                }
                            }

                            var detalles = FacturaTem.FacturaDetallesPago;
                            for (int i = 0; i < detalles.Count(); i++)
                            {
                                var distribuciones = detalles.ElementAt(i).Distribuciones;
                                for (int j = 0; j < distribuciones.Count(); j++)
                                {
                                    db.FacturaDetallePagoDistribuciones.Remove(distribuciones.ElementAt(j));
                                    j--;
                                }

                                db.FacturaDetallesPago.Remove(detalles.ElementAt(i));
                                i--;
                            }

                            foreach (var Det in Obj.FacturaDetallesPago)
                            {
                                Det.FacturaCabeceraPagoId = Obj.Id;
                                db.FacturaDetallesPago.Add(Det);
                            }

                            db.Entry(FacturaTem).State = EntityState.Modified;
                            /****/

                            if (FacturasEliminarAdjuntoArr != null)
                            {
                                foreach (var i in FacturasEliminarAdjuntoArr)
                                {
                                    var facturaEliminar = db.FacturaCabecerasPago.Find(i);

                                    if (facturaEliminar.Tipo == "Física")
                                    {
                                        this.DeshabilitarAdjunto(FacturaTem.Id, i);
                                    }
                                    else if (facturaEliminar.Tipo == "Electrónica")
                                    {
                                        //cambiar el estado de las facturas electrónicas
                                        ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                        {
                                            ruc = facturaEliminar.ComprobanteElectronico.ruc,
                                            establecimiento = facturaEliminar.ComprobanteElectronico.establecimiento,
                                            puntoEmision = facturaEliminar.ComprobanteElectronico.puntoEmision,
                                            secuencial = facturaEliminar.ComprobanteElectronico.secuencial,
                                            tipoDocumento = facturaEliminar.ComprobanteElectronico.tipoDocumento,
                                            estadoDocumento = 1,
                                            codigoSistema = 3
                                        };

                                        TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                                    }
                                }
                            }

                            db.Entry(FacturaTem).State = EntityState.Modified;
                            db.SaveChanges();

                            /***/
                            var Files = Request.Files;

                            if (Files != null)
                            {
                                int loop = 1;
                                string uuid = System.Guid.NewGuid().ToString();

                                foreach (string key in Files)
                                {
                                    HttpPostedFileBase postedFile = Request.Files[key];

                                    byte[] BinaryFile = null;

                                    using (BinaryReader b = new BinaryReader(postedFile.InputStream))
                                    {
                                        BinaryFile = b.ReadBytes(postedFile.ContentLength);
                                    }

                                    AdjuntoViewModel adjunto = new AdjuntoViewModel()
                                    {
                                        ContenidoArchivo = BinaryFile,
                                        Propiedades = new List<PropiedadAdjuntoViewModel>()
                                    };

                                    long idFac = 0;

                                    if (Obj.AdjuntoName == postedFile.FileName)
                                    {
                                        idFac = Obj.Id;
                                        break;
                                    }

                                    if (idFac == 0)
                                    {
                                        throw new Exception("No se ha podido emparejar las facturas con los archivos. " + postedFile.FileName);
                                    }

                                    adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                        new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                            Codigo = "1063",
                                            Valor = Obj.Id.ToString() + "-" + idFac.ToString()
                                        },
                                        new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                            Codigo = "1086",
                                            Valor = "factura-pago"
                                        },
                                        new PropiedadAdjuntoViewModel() { // IdAdjunto
                                            Codigo = "1087",
                                            Valor = loop + "-" + uuid
                                        },
                                        new PropiedadAdjuntoViewModel() { // Estado
                                            Codigo = "1050",
                                            Valor = ((int)EnumEstado.ACTIVO).ToString()
                                        }
                                    };

                                    SolicitudPagoBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                                    loop++;
                                }
                            }
                            /***/

                            var Solicitud = db.SolicitudesPagoCabecera.Find(tarea.SolicitudPagoCabeceraId);

                            decimal totalTem = 0;

                            foreach(var fac in Solicitud.Facturas)
                            {
                                totalTem += fac.Total;
                            }

                            Solicitud.MontoTotal = totalTem;

                            db.Entry(Solicitud).State = EntityState.Modified;

                            SolicitudPagoCabecera SolicitudTem = new SolicitudPagoCabecera()
                            {
                                Id = (long)tarea.SolicitudPagoCabeceraId,
                                AprobacionJefeArea = AprobacionJefeArea,
                                NumeroSolicitud = Solicitud.NumeroSolicitud,
                                SolicitanteNombreCompleto = Solicitud.SolicitanteNombreCompleto,

                            };

                            TareaPagoBLL.CrearTarea(SolicitudTem, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA, db, tarea.Id);

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
                }
            }

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            return Json(new
            {
                error,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para deshabilitar los adjuntos.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud de la que se desean deshabilitar los adjuntos.</param>
        /// <param name="FacturaId">Identificador de la factura de la que se desean deshabilitar los adjuntos.</param>
        private void DeshabilitarAdjunto(
            long SolicitudId, 
            long FacturaId)
        {
            /** OBTENER OBJETO DE MFILES **/
            sesion = Session["vars"] as ContenedorVariablesSesion;

            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = SolicitudId.ToString() + "-" + FacturaId.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "factura-pago"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = "1"
                    }
                };

            var adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);
            /******************************/

            if (adjuntos.Count() > 0)
            {
                List<PropiedadAdjuntoViewModel> PropiedadesModificar = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = "2"
                    }
                };

                SolicitudCompraBLL.ModificarPropiedadesAdjunto("0", adjuntos.ElementAt(0).Id, PropiedadesModificar, sesion);
            }
        }

        /// <summary>
        /// Proceso para obtener una tarea.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea que se desea obtener.</param>
        public JsonResult ObtenerTarea(long TareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            TareaPagoViewModel tarea = null;

            try
            {
                tarea = TareaPagoBLL.ObtenerTarea(TareaId, db);

                if (tarea.TareaPadreId != null)
                {
                    tarea.TareaPadre = TareaPagoBLL.ObtenerTarea((long)tarea.TareaPadreId, db);
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

            return Json(new
            {
                tarea,
                validacion,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para restablecer el estado de una factura electrónica.
        /// </summary>
        /// <param name="factura">Objeto que contiene los datos de la factura electrónica a restablecer.</param>
        private void RestablecerFacturaElectronica(FacturaCabeceraPago factura)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;

            ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
            {
                ruc = factura.ComprobanteElectronico.ruc,
                establecimiento = factura.ComprobanteElectronico.establecimiento,
                puntoEmision = factura.ComprobanteElectronico.puntoEmision,
                secuencial = factura.ComprobanteElectronico.secuencial,
                tipoDocumento = factura.ComprobanteElectronico.tipoDocumento,
                estadoDocumento = 1,
                codigoSistema = 3
            };

            TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
        }
    }
}