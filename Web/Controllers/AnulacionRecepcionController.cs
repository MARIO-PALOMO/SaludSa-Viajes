using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador del módulo de anulación de recepciones.
    /// </summary>
    [SesionFilter]
    public class AnulacionRecepcionController : Controller
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
        /// Proceso para buscar recepciones.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud de la que se desean obtener las recepciones.</param>
        public JsonResult BuscarRecepciones(long NumeroSolicitud)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<RecepcionViewModel> recepciones = null;
            SolicitudCompraCabeceraViewModel solicitud = null;

            try
            {
                var Cabecera = db.SolicitudesCompraCabecera.Where(x => x.NumeroSolicitud == NumeroSolicitud && x.EstadoId == (int)EnumEstado.ACTIVO).FirstOrDefault();

                if(Cabecera != null)
                {
                    recepciones = AnulacionRecepcionBLL.BuscarRecepciones(NumeroSolicitud, Cabecera, sesion);

                    solicitud = new SolicitudCompraCabeceraViewModel()
                    {
                        Id = Cabecera.Id
                    };
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
                solicitud,
                recepciones,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para obtener una recepción.
        /// </summary>
        /// <param name="Id">Identificador de la recepción.</param>
        public JsonResult CargarRecepcion(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            RecepcionViewModel recepcion = null;

            try
            {
                recepcion = TareaBLL.ObtenerRecepcion(Id, db);

                foreach (var linea in recepcion.RecepcionLineas)
                {
                    foreach (var distribucion in linea.PlantillaDistribucionDetalle)
                    {
                        distribucion.MetadatosCentrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, distribucion.DepartamentoCodigo, recepcion.EmpresaParaLaQueSeCompra);
                        distribucion.MetadatosPropositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, distribucion.DepartamentoCodigo, distribucion.CentroCostoCodigo, recepcion.EmpresaParaLaQueSeCompra);
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
                recepcion,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los datos de la solicitud y de la orden madre a la que pertenece una recepción.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud.</param>
        /// <param name="OrdenMadreId">Identificador de la orden madre.</param>
        public JsonResult ObtenerDatos(
            long SolicitudId, 
            long OrdenMadreId)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            SolicitudCompraCabeceraViewModel cabecera = null;

            try
            {
                cabecera = TareaBLL.ObtenerSolicitudConSaldosEnDetallesParaRecepcion(SolicitudId, OrdenMadreId, db);

                cabecera.SolicitanteObj = SolicitudCompraBLL.ObtenerUsuario(cabecera.SolicitanteObjUsuario, sesion);
                cabecera.AprobacionJefeArea = SolicitudCompraBLL.ObtenerUsuario(cabecera.AprobacionJefeAreaUsuario, sesion);

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
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para anular una recepción.
        /// </summary>
        /// <param name="Id">Identificador de la recepción.</param>
        /// <param name="Motivo">Motivo de la anulación.</param>
        public JsonResult AnularRecepcion(
            long Id, 
            string Motivo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var recepcion = db.Recepciones.Find(Id);

                    if (recepcion.Contabilizada == false)
                    {
                        recepcion.EsperandoAutorizacionAnulacion = true;
                        db.Entry(recepcion).State = EntityState.Modified;

                        var tarea = TareaBLL.CrearAnulacionRecepcion(recepcion.OrdenMadre.SolicitudCompraCabecera, sesion, db, string.Empty, 0, recepcion, Motivo);
                        TareaBLL.CrearAprobarAnulacionRecepcion(recepcion.OrdenMadre.SolicitudCompraCabecera, sesion, db, string.Empty, 0, recepcion, Motivo, tarea.Id);

                        db.SaveChanges();
                        dbcxtransaction.Commit();
                    }
                    else
                    {
                        validacion.Add("La recepción no se puede anular debido a que está contabilizada.");
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