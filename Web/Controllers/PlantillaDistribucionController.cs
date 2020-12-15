using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Rest;
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
    /// Controlador del módulo de platillas de distribución.
    /// </summary>
    [SesionFilter]
    public class PlantillaDistribucionController : Controller
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
        /// Proceso para obtener los datos necesarios en la interfaz de crear, editar y visualizar una platilla de distribución.
        /// </summary>
        /// <param name="PlantillaId">Identificador de una plantilla para precargar.</param>
        /// <param name="EmpresaCodigo">Identificador de la compañía a la que pertenece la plantilla.</param>
        public JsonResult ObtenerMetadatos(
            long PlantillaId, 
            string EmpresaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EmpresaViewModel> Empresas = null;
            EmpresaViewModel EmpresaActiva = null;
            List<DepartamentoViewModel> Departamentos = null;
            List<PlantillasDistribucionCabeceraViewModel> Plantillas = null;

            try
            {
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                EmpresaActiva = new EmpresaViewModel()
                {
                    Codigo = Empresas.ElementAt(0).Codigo,
                    Nombre = Empresas.ElementAt(0).Nombre
                };

                string EmpresaSeleccionada = EmpresaActiva.Codigo;

                if (PlantillaId != -1)
                {
                    EmpresaSeleccionada = db.PlantillasDistribucionCabecera.Find(PlantillaId).EmpresaCodigo;
                }
                else if(EmpresaCodigo != "-1")
                {
                    EmpresaSeleccionada = EmpresaCodigo;
                }

                Departamentos = PlantillaDistribucionBLL.ObtenerDepartamentos(sesion, EmpresaSeleccionada);
                Plantillas = PlantillaDistribucionBLL.ObtenerPlantillasDistribucion(db, EmpresaSeleccionada);
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
                Departamentos,
                Empresas,
                EmpresaActiva,
                Plantillas,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener las plantillas de distribución por compañía.
        /// </summary>
        /// <param name="CompaniaCodigo">Identificador de la compañía de la que se quieren obtener las plantillas de distribución.</param>
        public JsonResult ObtenerPlantillasDistribucion(string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<PlantillasDistribucionCabeceraViewModel> plantillas = null;

            try
            {
                plantillas = PlantillaDistribucionBLL.ObtenerPlantillasDistribucion(db, CompaniaCodigo);
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

            return Json(new {
                plantillas,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los departamentos por compañía.
        /// </summary>
        /// <param name="CompaniaCodigo">Identificador de la compañía de la que se quieren obtener los departamentos.</param>
        public JsonResult ObtenerDepartamentos(string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();        

            List<DepartamentoViewModel> departamentos = null;

            try
            {
                departamentos = PlantillaDistribucionBLL.ObtenerDepartamentos(sesion, CompaniaCodigo);
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
                departamentos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los centros de costo por departamento y compañía.
        /// </summary>
        /// <param name="DepartamentoCodigo">Identificador del departamento del que se quieren obtener los centros de costo.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía de la que se quieren obtener los centros de costo.</param>
        public JsonResult ObtenerCentrosCosto(
            string DepartamentoCodigo, 
            string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<CentroCostoViewModel> centrosCosto = null;

            try
            {
                centrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, DepartamentoCodigo, CompaniaCodigo);
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
                centrosCosto,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los propósitos por departamento, centro de costo y compañía.
        /// </summary>
        /// <param name="DepartamentoCodigo">Identificador del departamento del que se quieren obtener los propósitos.</param>
        /// <param name="CentroCostoCodigo">Identificador del centro de costo del que se quieren obtener los propósitos.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía de la que se quieren obtener los propósitos.</param>
        public JsonResult ObtenerPropositos(
            string DepartamentoCodigo, 
            string CentroCostoCodigo, 
            string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<PropositoViewModel> propositos = null;

            try
            {
                propositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, DepartamentoCodigo, CentroCostoCodigo, CompaniaCodigo);
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
                propositos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para cargar una plantilla de distribución.
        /// </summary>
        /// <param name="Id">Identificador de la plantilla de distribución que se desea cargar.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía a la que pertenece la plantilla de distribución.</param>
        public JsonResult CargarPlantilla(
            long Id, 
            string CompaniaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<PlantillasDistribucionDetalleViewModel> Detalles = null;

            try
            {
                Detalles = PlantillaDistribucionBLL.ObtenerDetallesPlantilla(Id, db);

                foreach (var Detalle in Detalles)
                {
                    Detalle.MetadatosCentrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, Detalle.Departamento.Codigo, CompaniaCodigo);
                    Detalle.MetadatosPropositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, Detalle.Departamento.Codigo, Detalle.CentroCosto.Codigo, CompaniaCodigo);
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

            return Json(new {
                Detalles,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para crear una nueva plantilla de distribución.
        /// </summary>
        /// <param name="Cabecera">Objeto que contiene los datos de la nueva plantilla de distribución.</param>
        [HttpPost]
        public JsonResult Create(PlantillaDistribucionCabecera Cabecera)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int cont = db.PlantillasDistribucionCabecera.Where(x => x.Descripcion == Cabecera.Descripcion && x.DescripcionDepartamentoPropietario == Cabecera.DescripcionDepartamentoPropietario && x.EstadoId == (int)EnumEstado.ACTIVO).Count();

                        if(cont > 0)
                        {
                            validacion.Add("La descripción entrada ya está en uso en otra plantilla.");
                        }
                        else
                        {
                            db.PlantillasDistribucionCabecera.Add(Cabecera);
                            db.SaveChanges();
                            dbcxtransaction.Commit();
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
            }

            var error = (from item in ModelState
                         where item.Value.Errors.Any()
                         select new ErrorValidacion { error = item.Value.Errors[0].ErrorMessage });

            return Json(new
            {
                error,
                validacion,
                Cabecera.Id
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener el detalle de una plantilla de distribución.
        /// </summary>
        /// <param name="Id">Identificador de la plantilla de distribución que se desea obetner el detalle.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            PlantillasDistribucionCabeceraViewModel plantilla = null;
            List<DepartamentoViewModel> Departamentos = null;
            List<EmpresaViewModel> Empresas = null;

            try
            {
                plantilla = PlantillaDistribucionBLL.ObtenerPlantilla(Id, db);

                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                foreach (var Detalle in plantilla.Detalles)
                {
                    Detalle.MetadatosCentrosCosto = PlantillaDistribucionBLL.ObtenerCentrosCosto(sesion, Detalle.Departamento.Codigo, plantilla.EmpresaCodigo);
                    Detalle.MetadatosPropositos = PlantillaDistribucionBLL.ObtenerPropositos(sesion, Detalle.Departamento.Codigo, Detalle.CentroCosto.Codigo, plantilla.EmpresaCodigo);
                }

                Departamentos = PlantillaDistribucionBLL.ObtenerDepartamentos(sesion, plantilla.EmpresaCodigo);
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
                Distribucion = plantilla,
                Departamentos,
                Empresas,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para editar una plantilla de distribución.
        /// </summary>
        /// <param name="Cabecera">Objeto que contiene los datos de la cabecera de la plantilla de distribución editada.</param>
        /// <param name="Detalles">Lista de objetos que contiene los datos de los detalles de la plantilla de distribución editada.</param>
        [HttpPost]
        public JsonResult Edit(
            PlantillaDistribucionCabecera Cabecera, 
            List<PlantillaDistribucionDetalle> Detalles)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int cont = db.PlantillasDistribucionCabecera.Where(x => x.Descripcion == Cabecera.Descripcion && x.DescripcionDepartamentoPropietario == Cabecera.DescripcionDepartamentoPropietario && x.EstadoId == (int)EnumEstado.ACTIVO && x.Id != Cabecera.Id).Count();

                        if (cont > 0)
                        {
                            validacion.Add("La descripción entrada ya está en uso en otra plantilla.");
                        }
                        else if(Cabecera.UsuarioPropietario != sesion.usuario.Usuario)
                        {
                            validacion.Add("No puede editar la plantilla pues no es su propietario.");
                        }
                        else
                        {
                            db.Entry(Cabecera).State = EntityState.Modified;

                            if (Detalles != null)
                            {
                                foreach (var Detalle in Detalles)
                                {
                                    if (Detalle.Id > 0)
                                    {
                                        if (Detalle.EstadoId == (int)EnumEstado.ACTIVO)
                                        {
                                            db.Entry(Detalle).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            var DetTem = db.PlantillasDistribucionDetalle.Find(Detalle.Id);
                                            DetTem.EstadoId = (int)EnumEstado.INACTIVO;
                                            db.Entry(DetTem).State = EntityState.Modified;
                                        }
                                    }
                                    else
                                    {
                                        db.PlantillasDistribucionDetalle.Add(Detalle);
                                    }
                                }
                            }

                            db.SaveChanges();
                            dbcxtransaction.Commit();
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
        /// Proceso para eliminar una plantilla de distribución.
        /// </summary>
        /// <param name="Id">Identificador de la plantilla de distribución que se desea eliminar.</param>
        [HttpPost]
        public JsonResult Eliminar(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            var Cabecera = db.PlantillasDistribucionCabecera.Find(Id);

            if(Cabecera != null)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (Cabecera.UsuarioPropietario != sesion.usuario.Usuario)
                        {
                            validacion.Add("No puede eliminar la plantilla pues no es su propietario.");
                        }
                        else
                        {
                            Cabecera.EstadoId = (int)EnumEstado.INACTIVO;
                            db.Entry(Cabecera).State = EntityState.Modified;

                            foreach (var Detalle in Cabecera.Detalles)
                            {
                                Detalle.EstadoId = (int)EnumEstado.INACTIVO;
                                db.Entry(Detalle).State = EntityState.Modified;
                            }

                            db.SaveChanges();
                            dbcxtransaction.Commit();
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
            }
            else
            {
                validacion.Add("La plantilla no existe.");
            }

            return Json(new
            {
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}