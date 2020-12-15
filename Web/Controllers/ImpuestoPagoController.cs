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
    /// Controlador del módulo de impuestos de solicitudes de pago.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class ImpuestoPagoController : Controller
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
        /// Proceso para obtener los datos necesarios en la interfaz de creación, edición y visualización de un impuesto.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EstadoViewModel> Estados = null;

            try
            {
                Estados = EstadoBLL.ObtenerEstados(db);
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
                Estados,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para buscar impuestos.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si en la busqueda se deben incluir los impuestos inactivos.</param>
        public JsonResult Buscar(bool MostrarInactivos)
        {
            List<string> validacion = new List<string>();

            List<ImpuestoPagoViewModel> impuestos = null;

            try
            {
                impuestos = ImpuestoPagoBLL.BuscarItems(MostrarInactivos, db);
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
                impuestos,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para crear un impuesto.
        /// </summary>
        /// <param name="Impuesto">Objeto que contiene los datos del nuevo impuesto.</param>
        [HttpPost]
        public JsonResult Create(ImpuestoPago Impuesto)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.ImpuestosPago.Add(Impuesto);
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
                Impuesto.Id
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener un impuesto.
        /// </summary>
        /// <param name="Id">Identificador del impuesto que se desea obtener.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            ImpuestoPagoViewModel impuesto = null;

            try
            {
                impuesto = ImpuestoPagoBLL.BuscarItem(Id, db);
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
                impuesto,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para editar un impuesto.
        /// </summary>
        /// <param name="Impuesto">Objeto que contiene los datos del impuesto editado.</param>
        [HttpPost]
        public JsonResult Edit(ImpuestoPago Impuesto)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(Impuesto).State = EntityState.Modified;

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
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}