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
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador del módulo de solicitudes de pago.
    /// </summary>
    [SesionFilter]
    public class SolicitudPagoController : Controller
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
        /// Pantalla inicial para la creación de una solicitud de pago.
        /// </summary>
        public ActionResult Create()
        {
            ViewBag.Accion = "create";
            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos requeridos en la interfaz de crear, editar y visualizar de una solicitud de pago.
        /// </summary>
        public JsonResult ObtenerMetadatos()
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

            try
            {
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
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos requeridos en la interfaz de crear, editar y visualizar una factura en una solicitud de pago.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía a la que pertenece la solicitud.</param>
        /// <param name="EsReembolso">Bandera que indica si es un reembolso.</param>
        public JsonResult ObtenerMetadatosFactura(
            string EmpresaCodigo,
            bool? EsReembolso)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TipoPagoViewModel> TiposPago = null;
            List<ImpuestoPagoViewModel> ImpuestosPago = null;

            try
            {
                TiposPago = TipoPagoBLL.BuscarItemsPorEmpresa(EmpresaCodigo, EsReembolso, db);
                ImpuestosPago = ImpuestoPagoBLL.BuscarItems(false, db);
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
                TiposPago,
                ImpuestosPago,
                sesion.UrlVisorRidePdf,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener proveedores.
        /// </summary>
        /// <param name="EmpresaCodigo">Filtro de compañía.</param>
        /// <param name="identificacion">Filtro de identificación.</param>
        /// <param name="nombresApellidos">Filtro de nombre y apellidos.</param>
        public JsonResult ObtenerProveedoresPagoViaje(
            string EmpresaCodigo,
            string identificacion, 
            string nombresApellidos)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ProveedorPagoViajeViewModel> ProveedoresPagoViaje = null;

            try
            {
                ProveedoresPagoViaje = SolicitudPagoBLL.ObtenerProveedoresPagoViaje(sesion, EmpresaCodigo, identificacion, nombresApellidos);
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
                ProveedoresPagoViaje,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso de creación de una nueva solicitud de pago.
        /// </summary>
        /// <param name="Cabecera">Objeto que contiene los datos de la nueva solicitud de pago serializados en un string.</param>
        [HttpPost]
        public JsonResult Create(string Cabecera)
        {
            List<string> validacion = new List<string>();
            string path = string.Empty;
            sesion = Session["vars"] as ContenedorVariablesSesion;
            string accion = "guardar";

            List<TareaPagoViewModel> tareas = null;
            SolicitudPagoCabecera Obj = null;

            try
            {
                var format = "dd/MM/yyyy"; // your datetime format
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                Obj = JsonConvert.DeserializeObject<SolicitudPagoCabecera>(Cabecera, dateTimeConverter);
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

            if (ModelState.IsValid && TryValidateModel(Obj) && validacion.Count() == 0)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (Obj.NumeroSolicitud == -1)
                        {
                            Obj.FechaSolicitud = DateTime.Now;
                            long? Numero = SolicitudPagoBLL.ObtenerNumeroSolicitud(db);
                            Obj.NumeroSolicitud = Numero;
                            accion = "enviar";

                            Obj.JsonOriginal = Cabecera;
                        }

                        db.SolicitudesPagoCabecera.Add(Obj);

                        db.SaveChanges();

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

                                foreach(var fac in Obj.Facturas)
                                {
                                    if(fac.AdjuntoName == postedFile.FileName)
                                    {
                                        idFac = fac.Id;
                                        break;
                                    }
                                }

                                if(idFac == 0)
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

                        if (accion == "enviar")
                        {
                            TareaPagoBLL.CrearTarea(Obj, sesion, (int)EnumTipoTareaPago.CREACION_SOLICITUD, db, 0);

                            TareaPagoBLL.CrearTarea(Obj, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA, db, 0);
                        }

                        foreach (var fac in Obj.Facturas)
                        {
                            if (fac.Tipo == "Electrónica")
                            {
                                //cambiar el estado de las facturas electrónicas
                                ComprobanteElectronicoDocumentoViewModel documento = new ComprobanteElectronicoDocumentoViewModel()
                                {
                                    ruc = fac.ComprobanteElectronico.ruc,
                                    establecimiento = fac.ComprobanteElectronico.establecimiento,
                                    puntoEmision = fac.ComprobanteElectronico.puntoEmision,
                                    secuencial = fac.ComprobanteElectronico.secuencial,
                                    tipoDocumento = fac.ComprobanteElectronico.tipoDocumento,
                                    estadoDocumento = 5,
                                    codigoSistema = 3
                                };

                                TareaBLL.ActualizarEstadoComprobanteElectronico(sesion, documento);
                            }
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

            var error = (from item in ModelState
                        where item.Value.Errors.Any()
                        select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            try
            {
                tareas = SolicitudPagoBLL.ObtenerTareas(Obj.Id, db);
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
                Obj.Id,
                Obj.NumeroSolicitud,
                Obj.FechaSolicitud,
                tareas
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de pago.
        /// </summary>
        /// <param name="MostrarContabilizados">Bandera que indica si se deben incluir las solicitudes contabilizadas.</param>
        public JsonResult ObtenerSolicitudes(bool MostrarContabilizados)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<SolicitudPagoCabeceraViewModel> solicitudes = null;

            try
            {
                solicitudes = SolicitudPagoBLL.ObtenerSolicitudes(sesion.usuario.Usuario, db, MostrarContabilizados);
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
                solicitudes,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Pantalla inicial para la edición de una solicitud de pago.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud que se desea editar.</param>
        /// <param name="accion">Acción que se está ejecutando (edit)</param>
        public ActionResult Edit(
            long Id, 
            string accion)
        {
            try
            {
                var tem = db.SolicitudesPagoCabecera.Find(Id);

                if (tem.NumeroSolicitud != null && accion == "edit")
                {
                    throw new Exception("No se puede editar una solicitud enviada.");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            ViewBag.Accion = accion;
            ViewBag.Id = Id;

            return View("Create");
        }

        /// <summary>
        /// Proceso para obtener el detalle de una solicitud.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud de la que se desea obtener el detalle.</param>
        /// <param name="TareaId">Identificador de la tarea que se está ejecutando.</param>
        public JsonResult Detalle(
            long Id,
            long? TareaId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudPagoCabeceraViewModel cabecera = null;

            try
            {
                var tarea = db.TareasPago.Find(TareaId);

                cabecera = SolicitudPagoBLL.ObtenerSolicitud(Id, (tarea != null ? tarea.FacturaCabeceraPagoId : null), db);

                cabecera.SolicitanteObj = SolicitudCompraBLL.ObtenerUsuario(cabecera.SolicitanteObjUsuario, sesion);

                var AprobacionJefeAreaUsuario = string.Empty;
                var AprobacionSubgerenteAreaUsuario = string.Empty;
                var AprobacionGerenteAreaUsuario = string.Empty;
                var AprobacionVicePresidenteFinancieroUsuario = string.Empty;
                var AprobacionGerenteGeneralUsuario = string.Empty;

                if (tarea != null && tarea.FacturaCabeceraPagoId != null)
                {
                    AprobacionJefeAreaUsuario = tarea.FacturaCabeceraPago.AprobacionJefeArea;
                    AprobacionSubgerenteAreaUsuario = tarea.FacturaCabeceraPago.AprobacionSubgerenteArea;
                    AprobacionGerenteAreaUsuario = tarea.FacturaCabeceraPago.AprobacionGerenteArea;
                    AprobacionVicePresidenteFinancieroUsuario = tarea.FacturaCabeceraPago.AprobacionVicePresidenteFinanciero;
                    AprobacionGerenteGeneralUsuario = tarea.FacturaCabeceraPago.AprobacionGerenteGeneral;
                }
                else
                {
                    AprobacionJefeAreaUsuario = cabecera.AprobacionJefeAreaUsuario;
                    AprobacionSubgerenteAreaUsuario = cabecera.AprobacionSubgerenteAreaUsuario;
                    AprobacionGerenteAreaUsuario = cabecera.AprobacionGerenteAreaUsuario;
                    AprobacionVicePresidenteFinancieroUsuario = cabecera.AprobacionVicePresidenteFinancieroUsuario;
                    AprobacionGerenteGeneralUsuario = cabecera.AprobacionGerenteGeneralUsuario;
                }

                cabecera.AprobacionJefeArea = SolicitudCompraBLL.ObtenerUsuario(AprobacionJefeAreaUsuario, sesion);

                if (AprobacionSubgerenteAreaUsuario != null && AprobacionSubgerenteAreaUsuario != "null")
                {
                    if (AprobacionSubgerenteAreaUsuario != "noaplica_0123")
                    {
                        cabecera.AprobacionSubgerenteArea = SolicitudCompraBLL.ObtenerUsuario(AprobacionSubgerenteAreaUsuario, sesion);
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

                if (AprobacionGerenteAreaUsuario != null && AprobacionGerenteAreaUsuario != "null")
                {
                    cabecera.AprobacionGerenteArea = SolicitudCompraBLL.ObtenerUsuario(AprobacionGerenteAreaUsuario, sesion);
                }

                if (AprobacionVicePresidenteFinancieroUsuario != null && AprobacionVicePresidenteFinancieroUsuario != "null")
                {
                    cabecera.AprobacionVicePresidenteFinanciero = SolicitudCompraBLL.ObtenerUsuario(AprobacionVicePresidenteFinancieroUsuario, sesion);
                }

                if (AprobacionGerenteGeneralUsuario != null && AprobacionGerenteGeneralUsuario != "null")
                {
                    cabecera.AprobacionGerenteGeneral = SolicitudCompraBLL.ObtenerUsuario(AprobacionGerenteGeneralUsuario, sesion);
                }

                if (cabecera.Facturas != null)
                {
                    foreach (var factura in cabecera.Facturas)
                    {
                        if(factura.FacturaDetallesPago != null)
                        {
                            foreach(var detalle in factura.FacturaDetallesPago)
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
        /// Proceso para eliminar una solicitud de pago.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud que se desea elimnar.</param>
        [HttpPost]
        public JsonResult Eliminar(long Id)
        {
            List<string> validacion = new List<string>();

            var Cabecera = db.SolicitudesPagoCabecera.Find(Id);

            if (Cabecera != null)
            {
                if (Cabecera.NumeroSolicitud != null)
                {
                    throw new Exception("No se puede eliminar una solicitud enviada.");
                }
                else
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            Cabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Cabecera).State = EntityState.Modified;

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
            else
            {
                validacion.Add("La solicitud no existe.");
            }

            return Json(new
            {
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para descargar un adjunto.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece el adjunto.</param>
        /// <param name="FacturaId">Identificador de la factura a la que pertenece el adjunto.</param>
        /// <param name="NoFactura">Número de la factura a la que pertenece el adjunto.</param>
        public FileResult DescargarAdjunto(
            long SolicitudId, 
            long FacturaId,
            string NoFactura)
        {
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

            var adjunto = SolicitudCompraBLL.DescargarAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

            string mimeType = MimeMapping.GetMimeMapping(NoFactura + ".pdf");

            return File(adjunto.ContenidoArchivo, mimeType, NoFactura + ".pdf");
        }

        /// <summary>
        /// Proceso para editar una solicitud de compra.
        /// </summary>
        /// <param name="Cabecera">Objeto serializado como un string que representa a la solicitud editada.</param>
        /// <param name="FacturasEliminar">Listado serializado de las facturas a eliminar.</param>
        /// <param name="FacturasEliminarAdjunto">Listado serializado de los adjuntos a aliminar.</param>
        [HttpPost]
        public JsonResult Edit(
            string Cabecera,
            string FacturasEliminar,
            string FacturasEliminarAdjunto)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

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
                        string accion = "guardar";

                        CabeceraTem = db.SolicitudesPagoCabecera.Find(Obj.Id);

                        if (CabeceraTem.NumeroSolicitud != null)
                        {
                            throw new Exception("La solicitud ya ha sido enviada.");
                        }

                        if (Obj.NumeroSolicitud == -1)
                        {
                            CabeceraTem.FechaSolicitud = DateTime.Now;
                            long? Numero = SolicitudPagoBLL.ObtenerNumeroSolicitud(db);
                            CabeceraTem.NumeroSolicitud = Numero;
                            accion = "enviar";

                            CabeceraTem.JsonOriginal = Cabecera;
                        }
                        else if (Obj.NumeroSolicitud != null)
                        {
                            accion = "reenviar";
                        }

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

                        if(FacturasEliminarArr != null)
                        {
                            foreach (var i in FacturasEliminarArr)
                            {
                                var facturaEliminar = db.FacturaCabecerasPago.Find(i);

                                if(facturaEliminar.Tipo == "Física")
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

                                foreach(var detElim in detallesEliminar)
                                {
                                    var distribucionesEliminar = db.FacturaDetallePagoDistribuciones.Where(x => x.FacturaDetallePagoId == detElim.Id).ToList();

                                    foreach(var distElim in distribucionesEliminar)
                                    {
                                        db.FacturaDetallePagoDistribuciones.Remove(distElim);
                                    }

                                    db.FacturaDetallesPago.Remove(detElim);
                                }

                                db.FacturaCabecerasPago.Remove(facturaEliminar);
                            }
                        }

                        if(FacturasEliminarAdjuntoArr != null)
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
                            if(Factura.Id > 0)
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
                                    if(FacturasEliminarAdjuntoArr.Contains(Factura.Id))
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

                        if (accion == "enviar")
                        {
                            TareaPagoBLL.CrearTarea(CabeceraTem, sesion, (int)EnumTipoTareaPago.CREACION_SOLICITUD, db, 0);

                            TareaPagoBLL.CrearTarea(CabeceraTem, sesion, (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA, db, 0);
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

            var error = (from item in ModelState
                        where item.Value.Errors.Any()
                        select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            return Json(new
            {
                error,
                validacion,
                CabeceraTem.NumeroSolicitud
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

            if(adjuntos.Count() > 0)
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
        /// Proceso para verificar la existencia de una factura.
        /// </summary>
        /// <param name="NoFactura">Número de la factura que se desea verificar.</param>
        /// <param name="RUC">RUC de la factura que se desea verificar.</param>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece la factura que se desea verificar.</param>
        public ActionResult VerificarSiExisteFactura(
            string NoFactura,
            string RUC, 
            long? SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            try
            {
                if(SolicitudPagoBLL.VerificarSiExisteFactura(db, NoFactura, RUC, SolicitudId))
                {
                    validacion.Add("Ya existe una factura con el mismo número para ese proveedor.");
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
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

    }
}