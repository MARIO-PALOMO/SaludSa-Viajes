using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Newtonsoft.Json;
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
    /// Controlador del módulo de solicitudes de compra.
    /// </summary>
    [SesionFilter]
    public class SolicitudCompraController : Controller
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
        /// Pantalla inicial para la creación de una solicitud de compra.
        /// </summary>
        public ActionResult Create()
        {
            ViewBag.Accion = "create";
            return View();
        }

        /// <summary>
        /// Proceso de creación de una nueva solicitud de compra.
        /// </summary>
        /// <param name="Cabecera">Objeto que contiene los datos de la nueva solicitud de compra serializados en un string.</param>
        [HttpPost]
        public JsonResult Create(string Cabecera)
        {
            List<string> validacion = new List<string>();
            string path = string.Empty;
            sesion = Session["vars"] as ContenedorVariablesSesion;
            string accion = "guardar";
            List<TareaViewModel> tareas = null;

            SolicitudCompraCabecera Obj = null;

            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                Obj = JsonConvert.DeserializeObject<SolicitudCompraCabecera>(Cabecera);
            }
            catch(Exception e)
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
                        if(Obj.NumeroSolicitud == -1)
                        {
                            Obj.FechaSolicitud = DateTime.Now;
                            long? Numero = SolicitudCompraBLL.ObtenerNumeroSolicitud(db);
                            Obj.NumeroSolicitud = Numero;
                            accion = "enviar";

                            Obj.JsonOriginal = Cabecera;
                        }

                        db.SolicitudesCompraCabecera.Add(Obj);

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

                                adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = Obj.Id.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
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

                                SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                                loop++;
                            }

                            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = Obj.Id.ToString()
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

                            adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);
                        }

                        if (accion == "enviar")
                        {
                            List<string> ValidacionesRolesGestoresCompra = SolicitudCompraBLL.ValidarRolesGestoresCompra(Obj, db);

                            if (ValidacionesRolesGestoresCompra != null && ValidacionesRolesGestoresCompra.Count() > 0)
                            {
                                foreach(var val in ValidacionesRolesGestoresCompra)
                                {
                                    validacion.Add(val);
                                }

                                throw new Exception();
                            }

                            TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.CREACION_SOLICITUD, db, 0);

                            bool irAJefeCompra = false;

                            foreach(var detalle in Obj.Detalles)
                            {
                                if(detalle.Producto == "BIEN_OTRO_P123" || detalle.Producto == "SERVICIO_OTRO_P123")
                                {
                                    irAJefeCompra = true;
                                    break;
                                }
                            }

                            if(irAJefeCompra)
                            {
                                TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO, db, 0);
                            }
                            else
                            {
                                TareaBLL.CrearTarea(Obj, sesion, (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO, db, 0);
                            }
                        }

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
                tareas = SolicitudCompraBLL.ObtenerTareas(Obj.Id, db);
            }
            catch(Exception e)
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
                tareas,
                adjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para clonar una solicitud de compra.
        /// </summary>
        /// <param name="Cabecera">Objeto serializado como un string que representa a la nueva solicitud clonada.</param>
        /// <param name="SolicitudId">Identificador de la solicitud de compra original.</param>
        /// <param name="AdjuntosAnteriores">Listado de adjuntos de la solicitud de compra original.</param>
        [HttpPost]
        public JsonResult Clonar(
            string Cabecera, 
            long SolicitudId, 
            string AdjuntosAnteriores)
        {
            List<string> validacion = new List<string>();
            string path = string.Empty;
            sesion = Session["vars"] as ContenedorVariablesSesion;
            string accion = "guardar";
            List<TareaViewModel> tareas = null;

            SolicitudCompraCabecera Obj = null;

            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                Obj = JsonConvert.DeserializeObject<SolicitudCompraCabecera>(Cabecera);
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
                            long? Numero = SolicitudCompraBLL.ObtenerNumeroSolicitud(db);
                            Obj.NumeroSolicitud = Numero;
                            accion = "enviar";

                            Obj.JsonOriginal = Cabecera;
                        }

                        db.SolicitudesCompraCabecera.Add(Obj);

                        db.SaveChanges();

                        var Files = Request.Files;

                        int loop = 1;
                        string uuid = System.Guid.NewGuid().ToString();

                        if (Files != null)
                        {
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

                                adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = Obj.Id.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
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

                                SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                                loop++;
                            }
                        }

                        if(AdjuntosAnteriores != null && AdjuntosAnteriores != string.Empty)
                        {
                            string[] AdjuntosAnterioresArray = AdjuntosAnteriores.Split('*');

                            foreach(var ad in AdjuntosAnterioresArray)
                            {
                                string[] tem = ad.Split('>');

                                List<PropiedadAdjuntoViewModel> PropiedadesBus = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = SolicitudId.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
                                    },
                                    new PropiedadAdjuntoViewModel() { // AdjuntoId
                                        Codigo = "1087",
                                        Valor = tem[0]
                                    }
                                };

                                var adjuntoOld = SolicitudCompraBLL.DescargarAdjunto(sesion.IdClaseMFiles, PropiedadesBus, sesion);

                                AdjuntoViewModel adjunto = new AdjuntoViewModel()
                                {
                                    ContenidoArchivo = adjuntoOld.ContenidoArchivo,
                                    Propiedades = new List<PropiedadAdjuntoViewModel>()
                                };

                                adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = Obj.Id.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
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

                                string extension = string.Empty;
                                string[] extensionArray = tem[1].Split('.');

                                if(extensionArray.Length > 1)
                                {
                                    extension = "." + extensionArray[extensionArray.Length - 1];
                                }

                                SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, tem[1] + extension, adjunto, sesion);

                                loop++;
                            }
                        }

                        List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                            new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                Codigo = "1063",
                                Valor = Obj.Id.ToString()
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

                        adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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
                                    break;
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
                tareas = SolicitudCompraBLL.ObtenerTareas(Obj.Id, db);
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
                tareas,
                adjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Pantalla inicial para la edición de una solicitud de compra.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud que se desea editar.</param>
        /// <param name="accion">Acción que se está ejecutando (edit, alcance)</param>
        public ActionResult Edit(
            long Id, 
            string accion)
        {
            try
            {
                var tem = db.SolicitudesCompraCabecera.Find(Id);

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

            if(accion == "alcance")
            {
                return View("Alcance");
            }

            return View("Create");
        }

        /// <summary>
        /// Proceso para editar una solicitud de compra.
        /// </summary>
        /// <param name="Cabecera">Objeto serializado como un string que representa a la solicitud editada.</param>
        [HttpPost]
        public JsonResult Edit(string Cabecera)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            string path = string.Empty;

            SolicitudCompraCabecera Obj = null;
            SolicitudCompraCabecera CabeceraTem = null;

            List<TareaViewModel> tareas = null;

            string accion = "guardar";

            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                Obj = JsonConvert.DeserializeObject<SolicitudCompraCabecera>(Cabecera);
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
                        CabeceraTem = db.SolicitudesCompraCabecera.Find(Obj.Id);

                        if(CabeceraTem.NumeroSolicitud != null)
                        {
                            throw new Exception("La solicitud ya ha sido enviada.");
                        }

                        if (Obj.NumeroSolicitud == -1)
                        {
                            CabeceraTem.FechaSolicitud = DateTime.Now;
                            long? Numero = SolicitudCompraBLL.ObtenerNumeroSolicitud(db);
                            CabeceraTem.NumeroSolicitud = Numero;
                            accion = "enviar";

                            CabeceraTem.JsonOriginal = Cabecera;
                        }
                        else if(Obj.NumeroSolicitud != null)
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

                        /*********/
                        var Files = Request.Files;

                        if (Files != null)
                        {
                            int loop = 1;
                            string uuid = System.Guid.NewGuid().ToString();

                            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = Obj.Id.ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "requerimiento"
                                }
                            };

                            //int MaxIdAdjunto = SolicitudCompraBLL.BuscarMaxIdAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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

                                adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = Obj.Id.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
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

                                SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);
                                
                                loop++;
                            }

                            List<PropiedadAdjuntoViewModel> PropiedadesBusquedaTem = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = Obj.Id.ToString()
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

                            adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaTem, sesion);
                        }
                        /*********/

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
                        else if(accion == "reenviar")
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

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            try
            {
                tareas = SolicitudCompraBLL.ObtenerTareas(Obj.Id, db);
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
                CabeceraTem.NumeroSolicitud,
                CabeceraTem.FechaSolicitud,
                tareas,
                adjuntos
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos requeridos en la interfaz de crear, editar, visualizar, clonar y alcance de una solicitud de compra.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud que se está procesando.</param>
        public JsonResult ObtenerMetadatos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            string[] gruposJefesAreas = new string[] { "GR Jefes", "GR Subgerentes", "GR Gerentes", "GR Gerente Financiero", "GR Gerentes Generales" };

            string[] gruposSubgerentesAreas = new string[] { "GR Subgerentes" };

            string[] gruposGerentesAreas = new string[] { "GR Gerentes" };

            string[] gruposVicepresidentesFinancieros = new string[] { "GR Gerente Financiero" };

            string[] gruposGerentesGenerales = new string[] { "GR Gerentes Generales" };

            EmpresaViewModel EmpresaActiva = null;

            List<ProductoMercadeoViewModel> ProductosMercadeo = null;
            List<UsuarioViewModel> JefesAreas = null;
            List<UsuarioViewModel> SubgerentesAreas = null;
            List<UsuarioViewModel> GerentesAreas = null;
            List<UsuarioViewModel> VicepresidentesFinancieros = null;
            List<UsuarioViewModel> GerentesGenerales = null;
            List<ProductoViewModel> Bienes = null;
            List<ProductoViewModel> Servicios = null;
            List<EmpresaViewModel> Empresas = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                EmpresaActiva = new EmpresaViewModel()
                {
                    Codigo = Empresas.ElementAt(0).Codigo,
                    Nombre = Empresas.ElementAt(0).Nombre
                };

                string EmpresaParaLaQueSeCompra = EmpresaActiva.Codigo;

                if (SolicitudId != -1)
                {
                    EmpresaParaLaQueSeCompra = db.SolicitudesCompraCabecera.Find(SolicitudId).EmpresaCodigo;
                }

                ProductosMercadeo = SolicitudCompraBLL.ObtenerProductosMercadeo(sesion);
                JefesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);
                SubgerentesAreas = SolicitudCompraBLL.ObtenerSubgerentes(gruposSubgerentesAreas, sesion);
                GerentesAreas = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesAreas, sesion);
                VicepresidentesFinancieros = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposVicepresidentesFinancieros, sesion);
                GerentesGenerales = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(gruposGerentesGenerales, sesion);
                Bienes = SolicitudCompraBLL.ObtenerProductosPorTipo("0", sesion, EmpresaParaLaQueSeCompra);
                Servicios = SolicitudCompraBLL.ObtenerProductosPorTipo("2", sesion, EmpresaParaLaQueSeCompra);
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
                EmpresaActiva,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener los productos de una compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía de la que se desean obtener sus productos.</param>
        public JsonResult ObtenerProductos(string EmpresaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<ProductoViewModel> Bienes = null;
            List<ProductoViewModel> Servicios = null;

            try
            {
                Bienes = SolicitudCompraBLL.ObtenerProductosPorTipo("0", sesion, EmpresaCodigo);
                Servicios = SolicitudCompraBLL.ObtenerProductosPorTipo("2", sesion, EmpresaCodigo);
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
                Bienes,
                Servicios,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de compra.
        /// </summary>
        /// <param name="SaldoCero">Bandera que indica si se deben incluir las solicitudes con saldo en cero.</param>
        public JsonResult ObtenerSolicitudes(bool SaldoCero)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<SolicitudCompraCabeceraViewModel> solicitudes = null;

            try
            {
                solicitudes = SolicitudCompraBLL.ObtenerSolicitudes(sesion.usuario.Usuario, db, SaldoCero);
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

            return Json(new {
                solicitudes,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método para formatear una fecha con formato yyyyMMddHHmmssffff.
        /// </summary>
        /// <param name="value">Fecha que se desea formatear.</param>
        private string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        /// <summary>
        /// Proceso para obtener el detalle de una solicitud.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud de la que se desea obtener el detalle.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudCompraCabeceraViewModel cabecera = null;
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                cabecera = SolicitudCompraBLL.ObtenerSolicitud(Id, db);

                cabecera.SolicitanteObj = SolicitudCompraBLL.ObtenerUsuario(cabecera.SolicitanteObjUsuario, sesion);
                cabecera.AprobacionJefeArea = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionJefeAreaUsuario, sesion);

                List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
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

                adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

                if (cabecera.AprobacionSubgerenteAreaUsuario != null)
                {
                    if(cabecera.AprobacionSubgerenteAreaUsuario != "noaplica_0123")
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

                if(cabecera.Detalles != null)
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para eliminar una solicitud de compra.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud que se desea elimnar.</param>
        [HttpPost]
        public JsonResult Eliminar(long Id)
        {
            List<string> validacion = new List<string>();

            var Cabecera = db.SolicitudesCompraCabecera.Find(Id);

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
        /// Proceso para eliminar el adjunto de una solicitud.
        /// </summary>
        /// <param name="AdjuntoId">Identificador del adjunto que se desea elimnar.</param>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece el adjunto que se desea eliminar.</param>
        /// <param name="Tipo">Tipo del adjunto según se ha salvado en el repositorio.</param>
        [HttpPost]
        public JsonResult EliminarAdjunto(
            string AdjuntoId, 
            long SolicitudId, 
            string Tipo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<AdjuntoViewModel> adjuntos = null;

            List<PropiedadAdjuntoViewModel> PropiedadesModificar = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = "2"
                    }
                };

            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = SolicitudId.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = Tipo
                    },
                    new PropiedadAdjuntoViewModel() { // Estado
                        Codigo = "1050",
                        Valor = ((int)EnumEstado.ACTIVO).ToString()
                    }
                };

            try
            {
                SolicitudCompraBLL.ModificarPropiedadesAdjunto("0", AdjuntoId, PropiedadesModificar, sesion);
                adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);
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
        /// Proceso para descargar un adjunto.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenece el adjunto.</param>
        /// <param name="AdjuntoId">Identificador del adjunto.</param>
        /// <param name="AdjuntoNombre">Nombre del adjunto.</param>
        /// <param name="Tipo">Tipo del adjunto según se ha salvado en el repositorio.</param>
        public FileResult DescargarAdjunto(
            long SolicitudId, 
            string AdjuntoId, 
            string AdjuntoNombre,
            string Tipo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;

            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                        Codigo = "1063",
                        Valor = SolicitudId.ToString()
                    },
                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                        Codigo = "1086",
                        Valor = Tipo
                    },
                    new PropiedadAdjuntoViewModel() { // AdjuntoId
                        Codigo = "1087",
                        Valor = AdjuntoId
                    }
                };

            var adjunto = SolicitudCompraBLL.DescargarAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

            string mimeType = MimeMapping.GetMimeMapping(AdjuntoNombre);

            return File(adjunto.ContenidoArchivo, mimeType, AdjuntoNombre);
        }

        /// <summary>
        /// Proceso para subir los requerimientos adjuntos.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenecen los requerimientos adjuntos.</param>
        public JsonResult SubirRequerimientosAdjuntos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                var Files = Request.Files;

                if (Files != null)
                {
                    int loop = 1;
                    string uuid = System.Guid.NewGuid().ToString();

                    List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "requerimiento"
                                }
                            };

                    //int MaxIdAdjunto = SolicitudCompraBLL.BuscarMaxIdAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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

                        adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = SolicitudId.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "requerimiento"
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

                        SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                        loop++;
                    }

                    List<PropiedadAdjuntoViewModel> PropiedadesBusquedaTem = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
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

                    adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaTem, sesion);
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para subir las cotizaciones.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenecen las cotizaciones.</param>
        public JsonResult SubirCotizacionesAdjuntos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                var Files = Request.Files;

                if (Files != null)
                {
                    int loop = 1;
                    string uuid = System.Guid.NewGuid().ToString();

                    List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "cotización"
                                }
                            };

                    //int MaxIdAdjunto = SolicitudCompraBLL.BuscarMaxIdAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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

                        adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = SolicitudId.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "cotización"
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

                        SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                        loop++;
                    }

                    List<PropiedadAdjuntoViewModel> PropiedadesBusquedaTem = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
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

                    adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaTem, sesion);
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para subir las evaluaciones.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenecen las evaluaciones.</param>
        public JsonResult SubirEvaluacionesAdjuntos(long SolicitudId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                var Files = Request.Files;

                if (Files != null)
                {
                    int loop = 1;
                    string uuid = System.Guid.NewGuid().ToString();

                    List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "evaluación"
                                }
                            };

                    //int MaxIdAdjunto = SolicitudCompraBLL.BuscarMaxIdAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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

                        adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = SolicitudId.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "evaluación"
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

                        SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                        loop++;
                    }

                    List<PropiedadAdjuntoViewModel> PropiedadesBusquedaTem = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
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

                    adjuntos = SolicitudCompraBLL.BuscarAdjuntos(sesion.IdClaseMFiles, PropiedadesBusquedaTem, sesion);
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para subir las facturas.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenecen las facturas.</param>
        /// <param name="RecepcionId">Identificador de la recepción a la que pertenecen las facturas.</param>
        public JsonResult SubirFacturasAdjuntos(
            long SolicitudId, 
            string RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                var Files = Request.Files;

                if (Files != null)
                {
                    int loop = 1;
                    string uuid = System.Guid.NewGuid().ToString();

                    List<PropiedadAdjuntoViewModel> PropiedadesBusqueda = new List<PropiedadAdjuntoViewModel>() {
                                new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                    Codigo = "1063",
                                    Valor = SolicitudId.ToString()
                                },
                                new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                    Codigo = "1086",
                                    Valor = "factura"
                                }
                            };

                    //int MaxIdAdjunto = SolicitudCompraBLL.BuscarMaxIdAdjunto(sesion.IdClaseMFiles, PropiedadesBusqueda, sesion);

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

                        adjunto.Propiedades = new List<PropiedadAdjuntoViewModel>() {
                                    new PropiedadAdjuntoViewModel() { // CodigoDocumento
                                        Codigo = "1063",
                                        Valor = SolicitudId.ToString()
                                    },
                                    new PropiedadAdjuntoViewModel() { // DescripcionTipo
                                        Codigo = "1086",
                                        Valor = "factura"
                                    },
                                    new PropiedadAdjuntoViewModel() { // IdAdjunto
                                        Codigo = "1087",
                                        Valor = loop + "-" + uuid
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

                        SolicitudCompraBLL.CargarAdjunto(sesion.IdClaseMFiles, Path.GetFileName(postedFile.FileName) + Path.GetExtension(postedFile.FileName), adjunto, sesion);

                        loop++;
                    }

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
        /// Proceso para actualizar las facturas adjuntas a una solicitud y a una recepción.
        /// </summary>
        /// <param name="FacturasActualizar">Listado de identificadores de facturas para actualizar.</param>
        /// <param name="SolicitudId">Identificador de la solicitud a la que pertenecen las facturas.</param>
        /// <param name="RecepcionId">Identificador de la recepción a la que pertenecen las facturas.</param>
        [HttpPost]
        public JsonResult ActualizarFacturasAdjuntas(
            List<int> FacturasActualizar,
            long SolicitudId, 
            string RecepcionId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();
            List<AdjuntoViewModel> adjuntos = null;

            try
            {
                if(FacturasActualizar != null)
                {
                    List<PropiedadAdjuntoViewModel> PropiedadesModificar = new List<PropiedadAdjuntoViewModel>() {
                        new PropiedadAdjuntoViewModel() { // Identificador de recepción (Se guarda en el campo observación)
                            Codigo = "1088",
                            Valor = RecepcionId
                        }
                    };

                    foreach (var AdjuntoId in FacturasActualizar)
                    {
                        SolicitudCompraBLL.ModificarPropiedadesAdjunto("0", AdjuntoId.ToString(), PropiedadesModificar, sesion);
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
                adjuntos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}