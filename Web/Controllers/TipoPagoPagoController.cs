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
    /// Controlador del módulo de tipos de pago.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class TipoPagoPagoController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio del módulo.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos requeridos en la interfaz de crear, editar y visualizar tipos de pago.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EstadoViewModel> Estados = null;
            List<CuentaContableViewModel> CuentasContable = null;
            List<EmpresaViewModel> Empresas = null;
            EmpresaViewModel EmpresaActiva = null;

            try
            {
                Estados = EstadoBLL.ObtenerEstados(db);

                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);

                EmpresaActiva = new EmpresaViewModel()
                {
                    Codigo = Empresas.ElementAt(0).Codigo,
                    Nombre = Empresas.ElementAt(0).Nombre
                };

                string EmpresaSeleccionada = EmpresaActiva.Codigo;

                CuentasContable = TipoPagoBLL.ObtenerCuentasContable(sesion, EmpresaSeleccionada);
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
                CuentasContable,
                Empresas,
                EmpresaActiva,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener las cuentas contables de una compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía de la que se desean obtener las cuentas contables.</param>
        public JsonResult ObtenerCuentasContable(string EmpresaCodigo)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<CuentaContableViewModel> CuentasContable = null;

            try
            {
                CuentasContable = TipoPagoBLL.ObtenerCuentasContable(sesion, EmpresaCodigo);
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
                CuentasContable,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para buscar los tipos de pago.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir en la búsqueda los tipos de pago inactivos.</param>
        public JsonResult Buscar(bool MostrarInactivos)
        {
            List<string> validacion = new List<string>();

            List<TipoPagoViewModel> tiposPago = null;

            try
            {
                tiposPago = TipoPagoBLL.BuscarItems(MostrarInactivos, db);
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
                tiposPago,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para crear un nuevo tipo de pago.
        /// </summary>
        /// <param name="TipoPago">Objeto que contiene los datos del nuevo tipo de pago.</param>
        [HttpPost]
        public JsonResult Create(TipoPago TipoPago)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.TiposPago.Add(TipoPago);
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
                TipoPago.Id
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener el detalle de un tipo de pago.
        /// </summary>
        /// <param name="Id">Identificador del tipo de pago del que se desea obtener el detalle.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            TipoPagoViewModel tipoPago = null;
            List<CuentaContableViewModel> CuentasContable = null;

            try
            {
                tipoPago = TipoPagoBLL.BuscarItem(Id, db);

                CuentasContable = TipoPagoBLL.ObtenerCuentasContable(sesion, tipoPago.EmpresaCodigo);
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
                tipoPago,
                CuentasContable,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para editar un tipo de pago.
        /// </summary>
        /// <param name="TipoPago">Objeto que contiene los datos del tipo de pago editado.</param>
        [HttpPost]
        public JsonResult Edit(TipoPago TipoPago)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(TipoPago).State = EntityState.Modified;

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