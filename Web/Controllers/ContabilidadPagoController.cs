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
    /// Controlador para el módulo de contabilización de solicitudes de pago.
    /// </summary>
    [SesionFilter]
    public class ContabilidadPagoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Proceso para obtener los datos necesarios para contabilizar una solicitud.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a contabilizar.</param>
        public JsonResult ObtenerMetadatos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            string[] gruposJefesAreas = new string[] { "GR Jefes", "GR Subgerentes", "GR Gerentes", "GR Gerente Financiero", "GR Gerentes Generales" };

            string[] gruposSubgerentesAreas = new string[] { "GR Subgerentes" };

            string[] gruposGerentesAreas = new string[] { "GR Gerentes" };

            string[] gruposVicepresidentesFinancieros = new string[] { "GR Gerente Financiero" };

            string[] gruposGerentesGenerales = new string[] { "GR Gerentes Generales" };

            List<EmpresaViewModel> Empresas = null;
            EmpresaViewModel EmpresaActiva = null;

            List<UsuarioViewModel> JefesAreas = null;
            List<UsuarioViewModel> SubgerentesAreas = null;
            List<UsuarioViewModel> GerentesAreas = null;
            List<UsuarioViewModel> VicepresidentesFinancieros = null;
            List<UsuarioViewModel> GerentesGenerales = null;

            List<PerfilAsientoContablePagoViewModel> PerfilesAsientosContables = null;
            List<DepartamentoViewModel> Departamentos = null;
            List<CuentaContableViewModel> CatalogosPago = null;

            try
            {
                var solicitud = db.SolicitudesPagoCabecera.Find(SolicitudId);

                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                EmpresaActiva = new EmpresaViewModel()
                {
                    Codigo = Empresas.ElementAt(0).Codigo,
                    Nombre = Empresas.ElementAt(0).Nombre
                };

                string EmpresaSeleccionada = EmpresaActiva.Codigo;

                JefesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);
                SubgerentesAreas = SolicitudCompraBLL.ObtenerSubgerentes(gruposSubgerentesAreas, sesion);
                GerentesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesAreas, sesion);
                VicepresidentesFinancieros = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposVicepresidentesFinancieros, sesion);
                GerentesGenerales = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesGenerales, sesion);

                PerfilesAsientosContables = SolicitudPagoBLL.ObtenerPerfilesAsientosContablesPago(sesion, solicitud.EmpresaCodigo);
                Departamentos = PlantillaDistribucionBLL.ObtenerDepartamentos(sesion, solicitud.EmpresaCodigo);
                CatalogosPago = TipoPagoBLL.ObtenerCuentasContable(sesion, solicitud.EmpresaCodigo);
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
                Empresas,
                EmpresaActiva,
                JefesAreas,
                SubgerentesAreas,
                GerentesAreas,
                VicepresidentesFinancieros,
                GerentesGenerales,
                sesion.UrlVisorRidePdf,

                PerfilesAsientosContables,
                Departamentos,
                CatalogosPago,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los diarios.
        /// </summary>
        /// <param name="CompaniaCodigo">Código de la compañía de la que se desean obtener los diarios.</param>
        /// <param name="tipoDiario">Tipo de diario seleccionado por el usuario.</param>
        public JsonResult ObtenerDiarios(
            string CompaniaCodigo, 
            int tipoDiario)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<DiarioPagoViewModel> Diarios = null;

            try
            {
                Diarios = SolicitudPagoBLL.ObtenerDiariosPago(sesion, CompaniaCodigo, tipoDiario);
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
                Diarios,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener una solicitud.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="tareaId">Identificador de la tarea que se ha ejecutado.</param>
        public JsonResult ObtenerSolicitud(
            long Id, 
            long tareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudPagoCabeceraViewModel cabecera = null;

            try
            {
                var tarea = db.TareasPago.Find(tareaId);

                cabecera = SolicitudPagoBLL.ObtenerSolicitudContabilidad(Id, (long)tarea.FacturaCabeceraPagoId, db);

                cabecera.SolicitanteObj = SolicitudCompraBLL.ObtenerUsuario(cabecera.SolicitanteObjUsuario, sesion);
                cabecera.AprobacionJefeArea = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionJefeAreaUsuario, sesion);

                if (cabecera.AprobacionSubgerenteAreaUsuario != null)
                {
                    if (cabecera.AprobacionSubgerenteAreaUsuario != "noaplica_0123")
                    {
                        cabecera.AprobacionSubgerenteArea = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionSubgerenteAreaUsuario, sesion);
                    }
                    else
                    {
                        cabecera.AprobacionSubgerenteArea = new UsuarioViewModel()
                        {
                            NombreCompleto = "N/A",
                            Usuario = "noaplica_0123"
                        };
                    }
                }

                if (cabecera.AprobacionGerenteAreaUsuario != null)
                {
                    cabecera.AprobacionGerenteArea = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionGerenteAreaUsuario, sesion);
                }

                if (cabecera.AprobacionVicePresidenteFinancieroUsuario != null)
                {
                    cabecera.AprobacionVicePresidenteFinanciero = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionVicePresidenteFinancieroUsuario, sesion);
                }

                if (cabecera.AprobacionGerenteGeneralUsuario != null)
                {
                    cabecera.AprobacionGerenteGeneral = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionGerenteGeneralUsuario, sesion);
                }

                if (cabecera.Facturas != null)
                {
                    foreach (var factura in cabecera.Facturas)
                    {
                        if (factura.FacturaDetallesPago != null)
                        {
                            foreach (var detalle in factura.FacturaDetallesPago)
                            {
                                if (detalle.PlantillaDistribucionDetalle != null)
                                {
                                    foreach (var distribucion in detalle.PlantillaDistribucionDetalle)
                                    {
                                        distribucion.MetadatosCentrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, distribucion.DepartamentoCodigo, cabecera.EmpresaParaLaQueSeCompra.Codigo);
                                        distribucion.MetadatosPropositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, distribucion.DepartamentoCodigo, distribucion.CentroCostoCodigo, cabecera.EmpresaParaLaQueSeCompra.Codigo);
                                    }
                                }
                            }
                        }
                    }
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
                cabecera,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios para contabilizar una factura.
        /// </summary>
        /// <param name="CompaniaCodigo">Identificador de la compañía a la que pertenece la solicitud.</param>
        public JsonResult ObtenerMetadatosContabilizacionFactura(string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<GrupoImpuestoPagoViewModel> GruposImpuestoPago = null;
            List<GrupoImpuestoArticulosPagoViewModel> GruposImpuestosArticulosPago = null;
            List<SustentoTributarioPagoViewModel> SustentosTributariosPago = null;

            try
            {
                GruposImpuestoPago = SolicitudPagoBLL.ObtenerGruposImpuestoPago(sesion, CompaniaCodigo);
                GruposImpuestosArticulosPago = SolicitudPagoBLL.ObtenerGruposImpuestosArticulosPago(sesion, CompaniaCodigo);
                SustentosTributariosPago = SolicitudPagoBLL.ObtenerSustentosTributariosPago(sesion, CompaniaCodigo);
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
                GruposImpuestoPago,
                GruposImpuestosArticulosPago,
                SustentosTributariosPago,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos complementarios necesarios para contabilizar una factura.
        /// </summary>
        /// <param name="CompaniaCodigo">Identificador de la compañía a la que pertenece la solicitud.</param>
        /// <param name="codigoGrupoImpuestoArticulo">Grupo de impuesto de artículo seleccionado por el usuario.</param>
        public JsonResult ObtenerMetadatosContabilizacionFacturaComplementarios(
            string CompaniaCodigo, 
            string codigoGrupoImpuestoArticulo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel> ImpuestoRentaGrupoImpuestosArticulosPago = null;
            List<IvaGrupoImpuestoArticuloPagoViewModel> IvaGrupoImpuestosArticulosPago = null;

            try
            {
                ImpuestoRentaGrupoImpuestosArticulosPago = SolicitudPagoBLL.ObtenerImpuestoRentaGrupoImpuestoArticuloPago(sesion, CompaniaCodigo, codigoGrupoImpuestoArticulo);
                IvaGrupoImpuestosArticulosPago = SolicitudPagoBLL.ObtenerIvaGrupoImpuestosArticulosPago(sesion, CompaniaCodigo, codigoGrupoImpuestoArticulo);
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
                ImpuestoRentaGrupoImpuestosArticulosPago,
                IvaGrupoImpuestosArticulosPago,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para contabilizar una solicitud de pago.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción seleccionada por el usuario.</param>
        /// <param name="Observacion">Texto de observación entrado por el usuario.</param>
        /// <param name="InformacionContabilidadPago">Objeto que contiene la información de contabilización seleccionada por el usuario.</param>
        /// <param name="NoLiquidacion">Número de liquidación entrado por el usuario.</param>
        /// <param name="NoAutorizacion">Número de autorización entrado por el usuario.</param>
        /// <param name="FechaEmision">Fecha de emisión entrada por el usuario.</param>
        /// <param name="FechaVencimiento">Fecha de vencimiento entrada por el usuario.</param>
        /// <param name="TipoPagoId">Tipo de pago seleccionado.</param>
        /// <param name="Detalles">Lista con la información de los detalles de la factura.</param>
        [HttpPost]
        public JsonResult Contabilizar(
            long TareaId, 
            string Accion, 
            string Observacion, 
            InformacionContabilidadPago InformacionContabilidadPago, 
            string NoLiquidacion,
            string NoAutorizacion,
            DateTime? FechaEmision,
            DateTime? FechaVencimiento,
            long? TipoPagoId,
            List<FacturaDetallePagoViewModel> Detalles)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            string numeroDiario = string.Empty;

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
                        db.SaveChanges();

                        var Solicitud = tarea.SolicitudPagoCabecera;

                        if (Accion == "Contabilizar") {
                            db.InformacionesContabilidadPago.Add(InformacionContabilidadPago);

                            var facturaTem = tarea.FacturaCabeceraPago;
                            facturaTem.NoLiquidacion = NoLiquidacion;
                            facturaTem.NoAutorizacion = NoAutorizacion;
                            facturaTem.FechaEmision = (DateTime)FechaEmision;
                            facturaTem.FechaVencimiento = (DateTime)FechaVencimiento;
                            facturaTem.TipoPagoId = (long)TipoPagoId;
                            db.Entry(facturaTem).State = EntityState.Modified;

                            foreach (var det in Detalles)
                            {
                                var detTem = db.FacturaDetallesPago.Find(det.Id);

                                detTem.GrupoImpuestoCodigo = det.GrupoImpuestoCodigo;
                                detTem.GrupoImpuestoDescripcion = det.GrupoImpuestoDescripcion;
                                detTem.GrupoImpuestoArticuloCodigo = det.GrupoImpuestoArticuloCodigo;
                                detTem.GrupoImpuestoArticuloDescripcion = det.GrupoImpuestoArticuloDescripcion;
                                detTem.GrupoImpuestoArticuloCodigoDescripcion = det.GrupoImpuestoArticuloCodigoDescripcion;
                                detTem.SustentoTributarioCodigo = det.SustentoTributarioCodigo;
                                detTem.SustentoTributarioDescripcion = det.SustentoTributarioDescripcion;
                                detTem.SustentoTributarioCodigoDescripcion = det.SustentoTributarioCodigoDescripcion;
                                detTem.ImpuestoRentaGrupoImpuestoArticuloCodigo = det.ImpuestoRentaGrupoImpuestoArticuloCodigo;
                                detTem.ImpuestoRentaGrupoImpuestoArticuloDescripcion = det.ImpuestoRentaGrupoImpuestoArticuloDescripcion;
                                detTem.ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion = det.ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion;
                                detTem.IvaGrupoImpuestoArticuloCodigo = det.IvaGrupoImpuestoArticuloCodigo;
                                detTem.IvaGrupoImpuestoArticuloDescripcion = det.IvaGrupoImpuestoArticuloDescripcion;
                                detTem.IvaGrupoImpuestoArticuloCodigoDescripcion = det.IvaGrupoImpuestoArticuloCodigoDescripcion;

                                var distribuciones = detTem.Distribuciones;
                                for (int j = 0; j < distribuciones.Count(); j++)
                                {
                                    db.FacturaDetallePagoDistribuciones.Remove(distribuciones.ElementAt(j));
                                    j--;
                                }

                                foreach(var dis in det.PlantillaDistribucionDetalle)
                                {
                                    db.FacturaDetallePagoDistribuciones.Add(new FacturaDetallePagoDistribucion() {
                                        Porcentaje = dis.Porcentaje,
                                        DepartamentoCodigo = dis.DepartamentoCodigo,
                                        DepartamentoDescripcion = dis.DepartamentoDescripcion,
                                        DepartamentoCodigoDescripcion = dis.DepartamentoCodigoDescripcion,
                                        CentroCostoCodigo = dis.CentroCostoCodigo,
                                        CentroCostoDescripcion = dis.CentroCostoDescripcion,
                                        CentroCostoCodigoDescripcion = dis.CentroCostoCodigoDescripcion,
                                        PropositoCodigo = dis.PropositoCodigo,
                                        PropositoDescripcion = dis.PropositoDescripcion,
                                        PropositoCodigoDescripcion = dis.PropositoCodigoDescripcion,
                                        FacturaDetallePagoId = detTem.Id,
                                        EstadoId = dis.EstadoId
                                    });
                                }

                                db.Entry(detTem).State = EntityState.Modified;
                            }

                            db.SaveChanges();

                            numeroDiario = TareaPagoBLL.Contabilizar(sesion, tarea, InformacionContabilidadPago, Detalles, db);
                        }
                        else if (Accion == "Devolver")
                        {
                            TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            /*var CantTareasActivas = Solicitud.Tareas.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.Id != tarea.Id).Count();

                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;*/

                            if (tarea.FacturaCabeceraPago.Tipo == "Electrónica")
                            {
                                RestablecerFacturaElectronica(tarea.FacturaCabeceraPago);
                            }

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazoContabilizacionPago;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{FACTURA}", tarea.FacturaCabeceraPago.NoFactura);
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

                            EmailBLL.EnviarPago(sesion, body, "Rechazo de contabilización de pago", destinatarios, null, tarea.Id, db);
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
            }

            return Json(new
            {
                validacion,
                numeroDiario
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para liberar una factura electrónica.
        /// </summary>
        /// <param name="factura">Objeto que contiene los datos de la factura a liberar.</param>
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