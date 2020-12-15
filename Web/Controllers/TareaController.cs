using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Newtonsoft.Json;
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
    /// Controlador del módulo de tareas de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    public class TareaController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla inicial del módulo.
        /// </summary>
        /// <param name="mensaje">Mensaje de notificación a visualizar.</param>
        public ActionResult Index(string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios en la interfaz de gestión de tareas. (Con proveedores e impuestos).
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece la tarea.</param>
        public JsonResult ObtenerMetadatosConProveedoresEImpuestos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            string[] gruposJefesAreas = new string[] { "GR Jefes", "GR Subgerentes", "GR Gerentes", "GR Gerente Financiero", "GR Gerentes Generales" };

            string[] gruposSubgerentesAreas = new string[] { "GR Subgerentes" };

            string[] gruposGerentesAreas = new string[] { "GR Gerentes" };

            string[] gruposVicepresidentesFinancieros = new string[] { "GR Gerente Financiero" };

            string[] gruposGerentesGenerales = new string[] { "GR Gerentes Generales" };

            //EmpresaViewModel EmpresaActiva = null;

            List<ProductoMercadeoViewModel> ProductosMercadeo = null;
            List<UsuarioViewModel> JefesAreas = null;
            List<UsuarioViewModel> SubgerentesAreas = null;
            List<UsuarioViewModel> GerentesAreas = null;
            List<UsuarioViewModel> VicepresidentesFinancieros = null;
            List<UsuarioViewModel> GerentesGenerales = null;
            List<ProductoViewModel> Bienes = null;
            List<ProductoViewModel> Servicios = null;
            List<EmpresaViewModel> Empresas = null;

            List<ProveedorViewModel> Proveedores = null;
            List<ImpuestoVigenteViewModel> Impuestos = null;

            try
            {
                var EmpresaParaLaQueSeCompra = db.SolicitudesCompraCabecera.Find(SolicitudId).EmpresaCodigo;

                //EmpresaActiva = new EmpresaViewModel()
                //{
                //    Codigo = sesion.usuario.CompaniaCodigo,
                //    Nombre = sesion.usuario.CompaniaDescripcion
                //};

                ProductosMercadeo = SolicitudCompraBLL.ObtenerProductosMercadeo(sesion);
                JefesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);
                SubgerentesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposSubgerentesAreas, sesion);
                GerentesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesAreas, sesion);
                VicepresidentesFinancieros = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposVicepresidentesFinancieros, sesion);
                GerentesGenerales = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesGenerales, sesion);
                Bienes = SolicitudCompraBLL.ObtenerProductosPorTipo("0", sesion, EmpresaParaLaQueSeCompra);
                Servicios = SolicitudCompraBLL.ObtenerProductosPorTipo("2", sesion, EmpresaParaLaQueSeCompra);
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                Proveedores = SolicitudCompraBLL.ObtenerProveedores(sesion, EmpresaParaLaQueSeCompra);
                Impuestos = SolicitudCompraBLL.ObtenerImpuestosVigente(sesion, EmpresaParaLaQueSeCompra);
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
                ProductosMercadeo,
                JefesAreas,
                SubgerentesAreas,
                GerentesAreas,
                VicepresidentesFinancieros,
                GerentesGenerales,
                Bienes,
                Servicios,
                Empresas,
                //EmpresaActiva,
                Proveedores,
                Impuestos,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios en la interfaz de gestión de tareas. (Con recepción).
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece la tarea.</param>
        /// <param name="TareaId">Identificador de la tarea.</param>
        public JsonResult ObtenerMetadatosRecepcion(
            long SolicitudId,
            long TareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            string[] gruposJefesAreas = new string[] { "GR Jefes", "GR Subgerentes", "GR Gerentes", "GR Gerente Financiero", "GR Gerentes Generales" };

            //EmpresaViewModel EmpresaActiva = null;

            List<ProductoMercadeoViewModel> ProductosMercadeo = null;
            List<UsuarioViewModel> JefesAreas = null;
            List<ProductoViewModel> Bienes = null;
            List<ProductoViewModel> Servicios = null;
            List<EmpresaViewModel> Empresas = null;
            int? NumeroRecepcion = null;
            List<RecepcionViewModel> HistorialRecepciones = null;

            try
            {
                var EmpresaParaLaQueSeCompra = db.SolicitudesCompraCabecera.Find(SolicitudId).EmpresaCodigo;

                var tarea = db.Tareas.Find(TareaId);

                //EmpresaActiva = new EmpresaViewModel()
                //{
                //    Codigo = sesion.usuario.CompaniaCodigo,
                //    Nombre = sesion.usuario.CompaniaDescripcion
                //};

                ProductosMercadeo = SolicitudCompraBLL.ObtenerProductosMercadeo(sesion);
                JefesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);
                Bienes = SolicitudCompraBLL.ObtenerProductosPorTipo("0", sesion, EmpresaParaLaQueSeCompra);
                Servicios = SolicitudCompraBLL.ObtenerProductosPorTipo("2", sesion, EmpresaParaLaQueSeCompra);
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                NumeroRecepcion = SolicitudCompraBLL.ObtenerNumeroRecepcion((long)tarea.OrdenMadreId, db);

                HistorialRecepciones = SolicitudCompraBLL.ObtenerHistorialRecepciones(TareaId, db);
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
                ProductosMercadeo,
                JefesAreas,
                Bienes,
                Servicios,
                Empresas,
                //EmpresaActiva,
                NumeroRecepcion,
                FechaRecepcion = DateTime.Now,
                HistorialRecepciones,
                sesion.UrlVisorRidePdf,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
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

            var tarea = db.Tareas.Find(tareaId);

            if(tarea.EstadoId == (int)EnumEstado.INACTIVO || tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                return Redirect("Index");
            }

            switch (tipo)
            {
                case (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO:
                    return View("JefeInmediatoEdit");
                case (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA:
                    return View("GestorCompraEdit");
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA:
                    return View("JefeCompraEdit");
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO:
                    return View("JefeCompraProductoOtro");
                case (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO:
                    return View("VerificacionPresupuestoEdit");
                case (int)EnumTipoTarea.RECEPCION:
                    return View("RecepcionEdit");
                case (int)EnumTipoTarea.APROBACION_DESEMBOLSO:
                    return View("AprobacionDesembolsoEdit");
                case (int)EnumTipoTarea.APROBACION_ANULACION_RECEPCION:
                    return View("AprobarAnulacionRecepcionEdit");
                case (int)EnumTipoTarea.ADJUNTAR_FACTURA:
                    return View("AdjuntarFacturaEdit");
                case (int)EnumTipoTarea.CONTABILIZAR_RECEPCION:
                    return View("ContabilizarRecepcionEdit");
                case (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE:
                    return View("DevueltaSolicitanteEdit");
                case (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO:
                    return View("RetornoJefeInmediatoEdit");

                case (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO;
                    return View("AprobacionPorMontoEdit");
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL;
                    return View("AprobacionPorMontoEdit");

                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA;
                    return View("AprobacionFueraPresupuestoEdit");
                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO:
                    ViewBag.Tipo = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO;
                    return View("AprobacionFueraPresupuestoEdit");
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

            List<TareaViewModel> tareas = null;

            try
            {
                tareas = TareaBLL.ObtenerTareas(sesion, db);
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
        /// Proceso para obtener los datos de una tarea y la solicitud a la que pertenece.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece la tarea.</param>
        /// <param name="TareaId">Identificador de la tarea.</param>
        public JsonResult ObtenerDatos(
            long SolicitudId,
            long TareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudCompraCabeceraViewModel cabecera = null;
            TareaViewModel tarea = null;

            List<AdjuntoViewModel> adjuntos = null;
            List<AdjuntoViewModel> cotizacionesAdjuntos = null;
            List<AdjuntoViewModel> evaluacionesAdjuntos = null;

            try
            {
                tarea = TareaBLL.ObtenerTarea(TareaId, db);

                if(tarea.TareaPadreId != null)
                {
                    tarea.TareaPadre = TareaBLL.ObtenerTarea((long)tarea.TareaPadreId, db);
                }

                cabecera = SolicitudCompraBLL.ObtenerSolicitud(SolicitudId, db);

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaReque = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "requerimiento"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaCoti = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "cotización"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaEva = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "evaluación"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaReque, sesion);
                cotizacionesAdjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaCoti, sesion);
                evaluacionesAdjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaEva, sesion);

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

                if (cabecera.Detalles != null)
                {
                    foreach (var detalle in cabecera.Detalles)
                    {
                        detalle.ProductoBien = detalle.Tipo == "0" ? SolicitudCompraBLL.ObtenerProducto(cabecera.EmpresaParaLaQueSeCompra.Codigo, detalle.ProductoCodigoArticulo, detalle.ProductoCodigoGrupo, sesion) : null;
                        detalle.ProductoServicio = detalle.Tipo == "2" ? SolicitudCompraBLL.ObtenerProducto(cabecera.EmpresaParaLaQueSeCompra.Codigo, detalle.ProductoCodigoArticulo, detalle.ProductoCodigoGrupo, sesion) : null;

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
                tarea,
                validacion,
                adjuntos,
                cotizacionesAdjuntos,
                evaluacionesAdjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos de una tarea y la solicitud a la que pertenece (Cuando es recepción).
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece la tarea.</param>
        /// <param name="TareaId">Identificador de la tarea.</param>
        public JsonResult ObtenerDatosRecepcion(
            long SolicitudId, 
            long TareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudCompraCabeceraViewModel cabecera = null;
            TareaViewModel tarea = null;

            List<AdjuntoViewModel> adjuntos = null;
            List<AdjuntoViewModel> cotizacionesAdjuntos = null;
            List<AdjuntoViewModel> evaluacionesAdjuntos = null;

            try
            {
                tarea = TareaBLL.ObtenerTarea(TareaId, db);

                if (tarea.TareaPadreId != null)
                {
                    tarea.TareaPadre = TareaBLL.ObtenerTarea((long)tarea.TareaPadreId, db);
                }

                cabecera = TareaBLL.ObtenerSolicitudConSaldosEnDetallesParaRecepcion(SolicitudId, (long)tarea.OrdenMadreId, db);

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaReque = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "requerimiento"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaCoti = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "cotización"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaEva = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = cabecera.Id.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "evaluación"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

                adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaReque, sesion);
                cotizacionesAdjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaCoti, sesion);
                evaluacionesAdjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaEva, sesion);

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

                if (cabecera.Detalles != null)
                {
                    foreach (var detalle in cabecera.Detalles)
                    {
                        detalle.ProductoBien = detalle.Tipo == "0" ? SolicitudCompraBLL.ObtenerProducto(cabecera.EmpresaParaLaQueSeCompra.Codigo, detalle.ProductoCodigoArticulo, detalle.ProductoCodigoGrupo, sesion) : null;
                        detalle.ProductoServicio = detalle.Tipo == "2" ? SolicitudCompraBLL.ObtenerProducto(cabecera.EmpresaParaLaQueSeCompra.Codigo, detalle.ProductoCodigoArticulo, detalle.ProductoCodigoGrupo, sesion) : null;

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
                tarea,
                validacion,
                adjuntos,
                cotizacionesAdjuntos,
                evaluacionesAdjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener una recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de la recepción.</param>
        public JsonResult ObtenerRecepcion(long RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            RecepcionViewModel recepcion = null;
            List<AdjuntoViewModel> facturasAdjuntos = null;

            try
            {
                recepcion = TareaBLL.ObtenerRecepcion(RecepcionId, db);

                foreach (var linea in recepcion.RecepcionLineas)
                {
                    foreach (var distribucion in linea.PlantillaDistribucionDetalle)
                    {
                        distribucion.MetadatosCentrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, distribucion.DepartamentoCodigo, recepcion.EmpresaParaLaQueSeCompra);
                        distribucion.MetadatosPropositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, distribucion.DepartamentoCodigo, distribucion.CentroCostoCodigo, recepcion.EmpresaParaLaQueSeCompra);
                    }
                }

                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaFac = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = "factura"
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // 
                        Codigo = "1088",
                        Valor = recepcion.NumeroOrdenMadre + "-" + recepcion.NumeroSolicitud + "-" + recepcion.NumeroRecepcion
                    }
                };

                facturasAdjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaFac, sesion);
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
                recepcion,
                facturasAdjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener las facturas físicas de una recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de la recepción a la que pertenecen las facturas.</param>
        public JsonResult ObtenerFacturasFisicas(string RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                List<PropiedadAdjuntoViewModel> PropiedadesBusquedaTem = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "factura"
                                },
                                new PropiedadAdjuntoViewModel() { // Estado
                                    Codigo = "1050",
                                    Valor = ((int)EnumEstado.ACTIVO).ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // Identificador de recepción (Se guarda en el campo observación)
                                    Codigo = "1088",
                                    Valor = RecepcionId
                                }
                            };

                adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaTem, sesion);
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para devolución a solicitante.
        /// </summary>
        /// <param name="Cabecera">Objeto serializado como un string que representa a la solicitud.</param>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        [HttpPost]
        public JsonResult EditSolicitudDevueltaASolicitante(
            string Cabecera, 
            long TareaId, 
            string Accion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            string path = string.Empty;

            SolicitudCompraCabecera Obj = null;
            SolicitudCompraCabecera CabeceraTem = null;

            List<TareaViewModel> tareas = null;

            string accion = "guardar";

            List<AdjuntoViewModel> adjuntos = null;

            var tarea = db.Tareas.Find(TareaId);

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
                try
                {
                    Obj = JsonConvert.DeserializeObject<SolicitudCompraCabecera>(Cabecera);
                }
                catch (Exception e1)
                {
                    validacion.Add(e1.Message);

                    if (e1.InnerException != null)
                    {
                        if (e1.InnerException.InnerException != null)
                        {
                            validacion.Add(e1.InnerException.InnerException.Message);
                        }
                        else
                        {
                            validacion.Add(e1.InnerException.Message);
                        }
                    }
                }

                if (validacion.Count() == 0 && ModelState.IsValid && TryValidateModel(Obj))
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            tarea.EstadoId = (int)EnumEstado.INACTIVO;
                            tarea.FechaProcesamiento = DateTime.Now;
                            tarea.Accion = Accion;
                            db.Entry(tarea).State = EntityState.Modified;

                            CabeceraTem = db.SolicitudesCompraCabecera.Find(Obj.Id);

                            if (Obj.NumeroSolicitud == -1)
                            {
                                CabeceraTem.FechaSolicitud = DateTime.Now;
                                long? Numero = SolicitudCompraBLL.ObtenerNumeroSolicitud(db);
                                CabeceraTem.NumeroSolicitud = Numero;
                                accion = "enviar";
                            }
                            else if (Obj.NumeroSolicitud != null)
                            {
                                accion = "reenviar";
                            }

                            CabeceraTem.EmpresaCodigo = Obj.EmpresaCodigo;
                            CabeceraTem.EmpresaNombre = Obj.EmpresaNombre;
                            CabeceraTem.ProveedorSugerido = Obj.ProveedorSugerido;
                            CabeceraTem.Frecuencia = Obj.Frecuencia;
                            CabeceraTem.MontoEstimado = Obj.MontoEstimado;
                            CabeceraTem.ProductoMercadeoCodigo = Obj.ProductoMercadeoCodigo;
                            CabeceraTem.ProductoMercadeoNombre = Obj.ProductoMercadeoNombre;
                            CabeceraTem.Descripcion = Obj.Descripcion;
                            CabeceraTem.AprobacionJefeArea = Obj.AprobacionJefeArea;
                            CabeceraTem.AprobacionSubgerenteArea = Obj.AprobacionSubgerenteArea;
                            CabeceraTem.AprobacionGerenteArea = Obj.AprobacionGerenteArea;
                            CabeceraTem.AprobacionVicePresidenteFinanciero = Obj.AprobacionVicePresidenteFinanciero;
                            CabeceraTem.AprobacionGerenteGeneral = Obj.AprobacionGerenteGeneral;

                            var detalles = CabeceraTem.Detalles;
                            for (int i = 0; i < detalles.Count(); i++)
                            {
                                var distribuciones = detalles.ElementAt(i).Distribuciones;
                                for (int j = 0; j < distribuciones.Count(); j++)
                                {
                                    db.SolicitudCompraDetalleDistribuciones.Remove(distribuciones.ElementAt(j));
                                    j--;
                                }

                                db.SolicitudesCompraDetalle.Remove(detalles.ElementAt(i));
                                i--;
                            }

                            foreach (var Det in Obj.Detalles)
                            {
                                Det.SolicitudCompraCabeceraId = Obj.Id;
                                db.SolicitudesCompraDetalle.Add(Det);
                            }

                            db.Entry(CabeceraTem).State = EntityState.Modified;

                            if (accion == "enviar")
                            {
                                List<string> ValidacionesRolesGestoresCompra = SolicitudCompraBLL.ValidarRolesGestoresCompra(Obj, db);

                                if (ValidacionesRolesGestoresCompra != null && ValidacionesRolesGestoresCompra.Count() > 0)
                                {
                                    foreach (var val in ValidacionesRolesGestoresCompra)
                                    {
                                        validacion.Add(val);
                                    }

                                    throw new Exception();
                                }

                                TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.CREACION_SOLICITUD, db, 0);

                                bool irAJefeCompra = false;

                                foreach (var detalle in Obj.Detalles)
                                {
                                    if (detalle.Producto == "BIEN_OTRO_P123" || detalle.Producto == "SERVICIO_OTRO_P123")
                                    {
                                        irAJefeCompra = true;
                                    }
                                }

                                if (irAJefeCompra)
                                {
                                    TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO, db, 0);
                                }
                                else
                                {
                                    TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO, db, 0);
                                }
                            }
                            else if (accion == "reenviar")
                            {
                                List<string> ValidacionesRolesGestoresCompra = SolicitudCompraBLL.ValidarRolesGestoresCompra(Obj, db);

                                if (ValidacionesRolesGestoresCompra != null && ValidacionesRolesGestoresCompra.Count() > 0)
                                {
                                    foreach (var val in ValidacionesRolesGestoresCompra)
                                    {
                                        validacion.Add(val);
                                    }

                                    throw new Exception();
                                }

                                TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO, db, 0);
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
            }

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            try
            {
                if(Obj != null)
                {
                    tareas = SolicitudCompraBLL.ObtenerTareas(Obj.Id, db);
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
                error,
                validacion,
                tareas,
                adjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobación de jefe inmediato.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaJefeInmediatoEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

            if (tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if(tarea.UsuarioResponsable != sesion.usuario.Usuario)
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

                        if (Accion == "Aprobar")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA, db, tarea.Id);
                        }
                        else if (Accion == "Devolver")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudCompraCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudCompraCabecera).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para cotización y selección de proveedor. (Gestor de compra).
        /// </summary>
        /// <param name="Detalles">Listado con los detalles modificados por el gestor de compras.</param>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="SolicitudId">Identificador de la solicitud de compra procesada.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        /// <param name="RetornaAJefeInmediato">Bandera que indica si la solicitud debe retornar al jefe inmediato.</param>
        [HttpPost]
        public JsonResult TareaGestorCompraEdit(
            List<SolicitudCompraDetalle> Detalles, 
            long TareaId, 
            long SolicitudId, 
            string Observacion, 
            bool RetornaAJefeInmediato)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var Tarea = db.Tareas.Find(TareaId);

            if (Tarea == null)
            {
                validacion.Add("La tarea no existe.");
            }
            else if (Tarea.EstadoId == (int)EnumEstado.INACTIVO)
            {
                validacion.Add("La tarea está en estado inactivo.");
            }
            else if (Tarea.UsuarioResponsable != sesion.usuario.Usuario)
            {
                validacion.Add("Usted no tiene permisos para actualizar la tarea.");
            }
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var detallesOld = db.SolicitudesCompraDetalle.Where(x => x.SolicitudCompraCabeceraId == SolicitudId).ToList();

                        for (int i = 0; i < detallesOld.Count(); i++)
                        {
                            var distribuciones = detallesOld.ElementAt(i).Distribuciones;
                            for (int j = 0; j < distribuciones.Count(); j++)
                            {
                                db.SolicitudCompraDetalleDistribuciones.Remove(distribuciones.ElementAt(j));
                            }

                            db.SolicitudesCompraDetalle.Remove(detallesOld.ElementAt(i));
                        }

                        foreach (var Det in Detalles)
                        {
                            Det.SolicitudCompraCabeceraId = SolicitudId;
                            db.SolicitudesCompraDetalle.Add(Det);
                        }

                        Tarea.Observacion = Observacion;
                        Tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        Tarea.RetornaAJefeInmediato = RetornaAJefeInmediato;
                        Tarea.Accion = "Aprobar";
                        Tarea.FechaProcesamiento = DateTime.Now;

                        db.SaveChanges();

                        var Obj = db.SolicitudesCompraCabecera.Find(SolicitudId);

                        List<string> ValidacionesRolesGestoresCompra = SolicitudCompraBLL.ValidarRolesGestoresCompra(Obj, db);

                        if (ValidacionesRolesGestoresCompra != null && ValidacionesRolesGestoresCompra.Count() > 0)
                        {
                            foreach (var val in ValidacionesRolesGestoresCompra)
                            {
                                validacion.Add(val);
                            }

                            throw new Exception();
                        }

                        TareaBLL.CrearTarea(Tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_JEFE_COMPRA, db, Tarea.Id);

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

                try
                {
                    tareas = SolicitudCompraBLL.ObtenerTareas(SolicitudId, db);
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

            var json = Json(new
            {
                validacion,
                tareas
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para aprobación de jefe de compra.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaJefeCompraEdit(
            long TareaId,
            string Accion, 
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        if (Accion == "Aprobar")
                        {
                            if (tarea.TareaPadre.RetornaAJefeInmediato == true)
                            {
                                TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO, db, tarea.Id);
                            }
                            else
                            {
                                TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO, db, tarea.Id);
                            }
                        }
                        else if (Accion == "Devolver")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.DEVOLVER_A_GESTOR_COMPRA, db, tarea.Id);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para retorno a jefe inmediato.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaRetornoJefeInmediatoEdit(
            long TareaId, 
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        if (Accion == "Aprobar")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO, db, tarea.Id);
                        }
                        else if (Accion == "Devolver")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudCompraCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudCompraCabecera).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para verificación de presupuesto.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        /// <param name="GerenteArea">Nombre de usuario de gerente de área seleccionado por el usuario.</param>
        /// <param name="VicepresidenteFinanciero">Nombre de usuario de vicepresidente financiero seleccionado por el usuario.</param>
        [HttpPost]
        public JsonResult TareaVerificacionPresupuestoEdit(
            long TareaId,
            string Accion,
            string Observacion,
            string GerenteArea, 
            string VicepresidenteFinanciero)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        var Solicitud = tarea.SolicitudCompraCabecera;

                        if (Accion == "Dentro")
                        {
                            if (Solicitud.AprobacionSubgerenteArea == "noaplica_0123" && Solicitud.MontoEstimado >= 2500)
                            {
                                if (Solicitud.AprobacionGerenteArea == null)
                                {
                                    throw new Exception("No se definió un Gerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else if (Solicitud.MontoEstimado >= 2500)
                            {
                                if (Solicitud.AprobacionSubgerenteArea == null)
                                {
                                    throw new Exception("No se definió un Subgerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaBLL.GenerarOrdenesMadre(Solicitud, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                            }
                        }
                        else if (Accion == "Fuera")
                        {
                            tarea.UsuarioGerenteArea = GerenteArea;
                            tarea.UsuarioVicepresidenteFinanciero = VicepresidenteFinanciero;

                            TareaBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA, db, tarea.Id, GerenteArea);
                        }
                        else if (Accion == "Devolver")
                        {
                            TareaBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTarea.DEVOLVER_A_GESTOR_COMPRA, db, tarea.Id);
                        }

                        db.Entry(tarea).State = EntityState.Modified;

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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para aprobación de fuera de presupuesto por gerente de área.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionFueraPresupuestoGerenteAreaEdit(
            long TareaId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        if (Accion == "Aprobar")
                        {
                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudCompraCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudCompraCabecera).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para aprobación de fuera de presupuesto por vicepresidente financiero.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobacionFueraPresupuestoVicepresidenteFinancieroEdit(
            long TareaId, 
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        if (Accion == "Aprobar")
                        {
                            TareaBLL.GenerarOrdenesMadre(tarea.SolicitudCompraCabecera, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudCompraCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudCompraCabecera).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso de aprobación por monto para subgerente de área.
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
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        var Solicitud = tarea.SolicitudCompraCabecera;

                        if (Accion == "Aprobar")
                        {
                            if (Solicitud.MontoEstimado >= 5000)
                            {
                                if (Solicitud.AprobacionGerenteArea == null)
                                {
                                    throw new Exception("No se definió un Gerente de Área en la solicitud.");
                                }
                                else
                                {
                                    TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaBLL.GenerarOrdenesMadre(tarea.SolicitudCompraCabecera, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso de aprobación por monto para gerente de área.
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
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        var Solicitud = tarea.SolicitudCompraCabecera;

                        if (Accion == "Aprobar")
                        {
                            if(Solicitud.MontoEstimado >= 10000)
                            {
                                if (Solicitud.AprobacionVicePresidenteFinanciero == null)
                                {
                                    throw new Exception("No se definió un Vicepresidente Financiero en la solicitud.");
                                }
                                else
                                {
                                    TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO, db, tarea.Id);
                                }
                            }
                            else
                            {
                                TareaBLL.GenerarOrdenesMadre(tarea.SolicitudCompraCabecera, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso de aprobación por monto para vicepresidente financiero.
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
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        var Solicitud = tarea.SolicitudCompraCabecera;

                        if (Accion == "Aprobar")
                        {
                            if(Solicitud.MontoEstimado >= 120000)
                            {
                                if(Solicitud.AprobacionGerenteGeneral == null)
                                {
                                    throw new Exception("No se definió un Gerente General en la solicitud.");
                                }
                                else
                                {
                                    //TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL, db, tarea.Id);
                                    TareaBLL.CrearAprobacionPorMontoGerenteGeneral(tarea.SolicitudCompraCabecera, sesion, db, string.Empty, 0, tarea.Id, AppDomain.CurrentDomain.BaseDirectory);
                                }
                            }
                            else
                            {
                                TareaBLL.GenerarOrdenesMadre(tarea.SolicitudCompraCabecera, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                            }
                        }
                        else if (Accion == "Negar")
                        {
                            Solicitud.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Solicitud).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso de aprobación por monto para gerente general.
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
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        if (Accion == "Aprobar")
                        {
                            TareaBLL.GenerarOrdenesMadre(tarea.SolicitudCompraCabecera, sesion, db, AppDomain.CurrentDomain.BaseDirectory, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            tarea.SolicitudCompraCabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(tarea.SolicitudCompraCabecera).State = EntityState.Modified;

                            // NOTIFICAR POR EMAIL
                            string body = EmailTemplatesResource.EmailRechazo;

                            body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                            body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                            body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                            body = body.Replace("{COMENTARIO}", tarea.Observacion);

                            List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                            {
                                new EmailDestinatarioViewModel()
                                {
                                    Nombre = tarea.NombreCompletoResponsable,
                                    Direccion = tarea.EmailResponsable
                                }
                            };

                            EmailBLL.Enviar(sesion, body, "Rechazo de solicitud", destinatarios, null, tarea.Id, db);
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para aprobación por el jefe de compras cuando el solicitante selecciona como producto "Otro".
        /// </summary>
        /// <param name="Detalles">Detalles de la solicitud actualizados por el jefe de compras.</param>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="SolicitudId">Identificador de la solicitud de compra procesada.</param>
        [HttpPost]
        public JsonResult TareaJefeCompraProductoOtroEdit(
            List<SolicitudCompraDetalle> Detalles, 
            long TareaId, 
            long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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
                        var detallesOld = db.SolicitudesCompraDetalle.Where(x => x.SolicitudCompraCabeceraId == SolicitudId).ToList();

                        for (int i = 0; i < detallesOld.Count(); i++)
                        {
                            var distribuciones = detallesOld.ElementAt(i).Distribuciones;
                            for (int j = 0; j < distribuciones.Count(); j++)
                            {
                                db.SolicitudCompraDetalleDistribuciones.Remove(distribuciones.ElementAt(j));
                            }

                            db.SolicitudesCompraDetalle.Remove(detallesOld.ElementAt(i));
                        }

                        foreach (var Det in Detalles)
                        {
                            Det.SolicitudCompraCabeceraId = SolicitudId;
                            db.SolicitudesCompraDetalle.Add(Det);
                        }

                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.Accion = "Corregido";
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.SaveChanges();

                        var Obj = db.SolicitudesCompraCabecera.Find(SolicitudId);

                        List<string> ValidacionesRolesGestoresCompra = SolicitudCompraBLL.ValidarRolesGestoresCompra(Obj, db);

                        if (ValidacionesRolesGestoresCompra != null && ValidacionesRolesGestoresCompra.Count() > 0)
                        {
                            foreach (var val in ValidacionesRolesGestoresCompra)
                            {
                                validacion.Add(val);
                            }

                            throw new Exception();
                        }

                        TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO, db, tarea.Id);

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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para registrar una recepción.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="AprobadorDesembolso">Nombre del usuario que debe aprobar el desembolso.</param>
        /// <param name="Recepcion">Objeto que contiene los datos de la recepción registrada.</param>
        [HttpPost]
        public JsonResult TareaRecepcionEdit(
            long TareaId, 
            string AprobadorDesembolso, 
            Recepcion Recepcion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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
                if (ModelState.IsValid)
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if(Recepcion.ComprobantesElectronicos != null)
                            {
                                foreach (var comprobante in Recepcion.ComprobantesElectronicos)
                                {
                                    ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel() {
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
                                }
                            }

                            int? NumeroRecepcion = SolicitudCompraBLL.ObtenerNumeroRecepcion((long)tarea.OrdenMadreId, db);

                            if(Recepcion.NumeroRecepcion < NumeroRecepcion)
                            {
                                Recepcion.NumeroRecepcion = (int)NumeroRecepcion;
                            }

                            Recepcion.Contabilizada = false;
                            Recepcion.UsuarioCreador = sesion.usuario.Usuario;
                            db.Recepciones.Add(Recepcion);

                            tarea.Accion = "Recepción";
                            tarea.UsuarioAprobadorDesembolso = AprobadorDesembolso;
                            tarea.EstadoId = (int)EnumEstado.INACTIVO;
                            tarea.FechaProcesamiento = DateTime.Now;                            

                            db.Entry(tarea).State = EntityState.Modified;

                            db.SaveChanges();

                            tarea.RecepcionId = Recepcion.Id;

                            db.Entry(tarea).State = EntityState.Modified;

                            db.SaveChanges();

                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.APROBACION_DESEMBOLSO, db, tarea.Id);

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

                if (tarea != null)
                {
                    try
                    {
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            return Json(new
            {
                error,
                validacion,
                tareas,
                Recepcion.NumeroRecepcion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para aprobar un desembolso.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="RecepcionId">Identificador de la recepción para aprobar.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobarDesembolsoEdit(
            long TareaId,
            long RecepcionId, 
            string Accion, 
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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
                    string NumeroOrdenHija = string.Empty;

                    try
                    {
                        tarea.Accion = Accion;
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        if (Accion == "Aprobar")
                        {
                            var recepcion = db.Recepciones.Find(RecepcionId);
                            recepcion.Aprobada = true;
                            db.Entry(recepcion).State = EntityState.Modified;

                            NumeroOrdenHija = TareaBLL.GenerarOrdenHija(recepcion, tarea.SolicitudCompraCabecera, sesion, db);

                            if (TareaBLL.ValidarSaldo(recepcion.OrdenMadreId, db))
                            {
                                TareaBLL.CrearRecepcion(tarea.SolicitudCompraCabecera, sesion, db, string.Empty, 0, tarea.Id, recepcion.OrdenMadreId);
                            }

                            TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.ADJUNTAR_FACTURA, db, tarea.Id);
                        }
                        else if (Accion == "Negar")
                        {
                            var recepcion = db.Recepciones.Find(RecepcionId);

                            if(recepcion.OrdenHija != null)
                            {
                                TareaBLL.BorrarFacturaAX(recepcion.OrdenHija.NumeroOrdenHija, tarea.SolicitudCompraCabecera.EmpresaCodigo, sesion);
                            }

                            if (recepcion.ComprobantesElectronicos != null)
                            {
                                foreach(var comprobante in recepcion.ComprobantesElectronicos)
                                {
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
                                }
                            }

                            recepcion.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(recepcion).State = EntityState.Modified;

                            foreach(var linea in recepcion.RecepcionLineas)
                            {
                                linea.EstadoId = (int)EnumEstado.INACTIVO;
                                db.Entry(linea).State = EntityState.Modified;
                            }

                            // Notificar
                            var UsuarioObj = UsuarioRCL.ObtenerUsuario(tarea.SolicitudCompraCabecera.SolicitanteUsuario, sesion);

                            if(UsuarioObj.Email != null)
                            {
                                string body = EmailTemplatesResource.EmailRecepcionNegada;

                                body = body.Replace("{RECEPCION}", tarea.SolicitudCompraCabecera.NumeroSolicitud + "-" + recepcion.NumeroRecepcion);
                                body = body.Replace("{COMPRA}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                                body = body.Replace("{SOLICITANTE}", tarea.SolicitudCompraCabecera.SolicitanteNombreCompleto);
                                body = body.Replace("{MOTIVO}", tarea.Observacion);

                                List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                                {
                                    new EmailDestinatarioViewModel()
                                    {
                                        Nombre = UsuarioObj.NombreCompleto,
                                        Direccion = UsuarioObj.Email
                                    }
                                };

                                EmailBLL.Enviar(sesion, body, "Recepción negada", destinatarios, null, tarea.Id, db);
                            }

                            TareaBLL.CrearRecepcion(tarea.SolicitudCompraCabecera, sesion, db, string.Empty, 0, tarea.Id, recepcion.OrdenMadreId);
                        }

                        db.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbcxtransaction.Rollback();
                        validacion.Add(e.Message);

                        if(NumeroOrdenHija != string.Empty)
                        {
                            TareaBLL.AnularRecepcion(NumeroOrdenHija, tarea.SolicitudCompraCabecera.EmpresaCodigo, sesion);
                        }

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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para adjuntar factura a una recepción.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAdjuntarFacturaEdit(
            long TareaId,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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
                        var recepcion = tarea.Recepcion;

                        if(recepcion.ComprobantesElectronicos != null)
                        {
                            var comprobantesList = recepcion.ComprobantesElectronicos.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO).ToList();

                            foreach (var comprobante in comprobantesList)
                            {
                                TareaBLL.ValidarFacturaAX(
                                    comprobante.establecimiento + comprobante.puntoEmision + comprobante.secuencial,
                                    comprobante.ruc,
                                    tarea.SolicitudCompraCabecera.EmpresaCodigo,
                                    sesion
                                );
                            }

                            foreach (var comprobante in comprobantesList)
                            {
                                TareaBLL.BorrarFacturaAX(recepcion.OrdenHija.NumeroOrdenHija, tarea.SolicitudCompraCabecera.EmpresaCodigo, sesion);
                            }

                            foreach (var comprobante in comprobantesList)
                            {
                                string[] fechaEmisionTem = comprobante.fechaEmision.Split('/');
                                string[] fechaAutorizacionTem = comprobante.fechaAutorizacion.Substring(0, 10).Split('/');

                                string fechaAut = string.Empty;

                                if (fechaAutorizacionTem.Length != 3)
                                {
                                    fechaAutorizacionTem = comprobante.fechaAutorizacion.Substring(0, 10).Split('-');
                                    fechaAut = fechaAutorizacionTem[1] + "/" + fechaAutorizacionTem[2] + "/" + fechaAutorizacionTem[0];
                                }
                                else
                                {
                                    fechaAut = fechaAutorizacionTem[1] + "/" + fechaAutorizacionTem[0] + "/" + fechaAutorizacionTem[2];
                                }                                

                                TareaBLL.InsertarFacturaAX(
                                    comprobante.numeroAutorizacion,
                                    comprobante.establecimiento + comprobante.puntoEmision + comprobante.secuencial,
                                    fechaEmisionTem[1] + "/" + fechaEmisionTem[0] + "/" + fechaEmisionTem[2],
                                    fechaAut,
                                    recepcion.OrdenHija.NumeroOrdenHija,
                                    comprobante.ruc,
                                    tarea.SolicitudCompraCabecera.EmpresaCodigo,
                                    sesion
                                );

                                /**************************/

                                ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                {
                                    ruc = comprobante.ruc,
                                    establecimiento = comprobante.establecimiento,
                                    puntoEmision = comprobante.puntoEmision,
                                    secuencial = comprobante.secuencial,
                                    tipoDocumento = comprobante.tipoDocumento,
                                    estadoDocumento = 2,
                                    codigoSistema = 3
                                };

                                TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                            }
                        }

                        tarea.Accion = "Adjuntado";
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        TareaBLL.CrearTarea(tarea.SolicitudCompraCabecera, sesion, (int)EnumTipoTarea.CONTABILIZAR_RECEPCION, db, tarea.Id);

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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para registrar la contabilización de una recepción.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaContabilizarRecepcionEdit(
            long TareaId, 
            string Accion, 
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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
                        tarea.Accion = "Contabilizar";
                        tarea.Observacion = Observacion;
                        tarea.EstadoId = (int)EnumEstado.INACTIVO;
                        tarea.FechaProcesamiento = DateTime.Now;

                        db.Entry(tarea).State = EntityState.Modified;

                        if(Accion == "Continuar")
                        {
                            var recepcion = db.Recepciones.Find(tarea.RecepcionId);
                            recepcion.Contabilizada = true;

                            db.Entry(recepcion).State = EntityState.Modified;
                        }
                        else if (Accion == "Devolver")
                        {

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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
        /// Proceso para aprobar la anulación de una recepción.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea ejecutada.</param>
        /// <param name="RecepcionId"></param>
        /// <param name="Accion">Acción selecionada por el usuario.</param>
        /// <param name="Observacion">Observación entrada por el usuario.</param>
        [HttpPost]
        public JsonResult TareaAprobarAnulacionRecepcionEdit(
            long TareaId,
            long RecepcionId,
            string Accion,
            string Observacion)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<TareaViewModel> tareas = null;

            var tarea = db.Tareas.Find(TareaId);

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

                        var recepcion = db.Recepciones.Find(RecepcionId);

                        if (Accion == "Aprobar")
                        {
                            if(!recepcion.Contabilizada)
                            {
                                //if(recepcion.Aprobada)
                                //{
                                    TareaBLL.BorrarFacturaAX(recepcion.OrdenHija.NumeroOrdenHija, tarea.SolicitudCompraCabecera.EmpresaCodigo, sesion);
                                    TareaBLL.AnularRecepcion(recepcion.OrdenHija.NumeroOrdenHija, tarea.SolicitudCompraCabecera.EmpresaCodigo, sesion);
                                //}

                                if (recepcion.ComprobantesElectronicos != null)
                                {
                                    foreach (var comprobante in recepcion.ComprobantesElectronicos)
                                    {
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
                                    }
                                }

                                var tareasAnular = db.Tareas.Where(x => x.RecepcionId == recepcion.Id && x.EstadoId == (int)EnumEstado.ACTIVO).ToList();

                                foreach(var tareaAnular in tareasAnular)
                                {
                                    tareaAnular.EstadoId = (int)EnumEstado.INACTIVO;
                                    tareaAnular.FechaProcesamiento = DateTime.Now;
                                    tareaAnular.Accion = "Anulada";
                                    db.Entry(tareaAnular).State = EntityState.Modified;
                                }

                                recepcion.EsperandoAutorizacionAnulacion = false;
                                recepcion.EstadoId = (int)EnumEstado.INACTIVO;
                                db.Entry(recepcion).State = EntityState.Modified;

                                foreach(var linea in recepcion.RecepcionLineas)
                                {
                                    linea.EstadoId = (int)EnumEstado.INACTIVO;
                                    db.Entry(linea).State = EntityState.Modified;
                                }

                                if(db.Tareas.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.OrdenMadreId == tarea.OrdenMadreId && x.TipoTarea == (int)EnumTipoTarea.RECEPCION).Count() == 0)
                                {
                                    if(db.Tareas.Where(x => x.SolicitudCompraCabeceraId == tarea.SolicitudCompraCabeceraId && x.Accion == "Detenida").Count() == 0)
                                    {
                                        TareaBLL.CrearRecepcion(tarea.SolicitudCompraCabecera, sesion, db, string.Empty, 0, tarea.Id, recepcion.OrdenMadreId);
                                    }
                                }

                            }
                            else
                            {
                                validacion.Add("No se pudo anular la recepción debido a que está contabilizada.");
                            }                           
                        }
                        else if (Accion == "Negar")
                        {
                            recepcion.EsperandoAutorizacionAnulacion = false;
                            db.Entry(recepcion).State = EntityState.Modified;
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
                        tareas = SolicitudCompraBLL.ObtenerTareas((long)tarea.SolicitudCompraCabeceraId, db);
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
    }
}