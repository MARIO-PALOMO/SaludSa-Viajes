using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Logic;
using Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador del módulo de roles de gestores de compra.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class RolGestorCompraController : Controller
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
        /// Proceso para obtener los datos requeridos en la interfaz de gestión de roles de gestores de compra.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<EmpresaViewModel> Empresas = null;

            string[] GrupoGestoresCompra = new string[] { "GR Gestor Compras" };
            List<UsuarioViewModel> Gestores = null;

            List<EstadoViewModel> Estados = null;

            EmpresaViewModel EmpresaActiva = null;

            List<TipoCompraViewModel> TiposCompra = null;

            try
            {
                Empresas = RolGestorCompraBLL.ObtenerEmpresas(sesion);

                Gestores = RolGestorCompraBLL.ObtenerUsuariosPorGrupo(GrupoGestoresCompra, sesion);

                Estados = EstadoBLL.ObtenerEstados(db);

                EmpresaActiva = new EmpresaViewModel()
                {
                    Codigo = Empresas.ElementAt(0).Codigo,
                    Nombre = Empresas.ElementAt(0).Nombre
                };

                TiposCompra = RolGestorCompraBLL.ObtenerTiposCompra(sesion, EmpresaActiva.Codigo);
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
                Empresas,
                TiposCompra,
                Gestores,
                Estados,
                EmpresaActiva,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los roles de gestores de compra.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los roles inactivos.</param>
        public JsonResult ObtenerRolesGestorCompra(bool MostrarInactivos)
        {
            List<string> validacion = new List<string>();

            List<RolGestorCompraViewModel> roles = null;

            try
            {
                roles = RolGestorCompraBLL.ObtenerRolesGestorCompra(MostrarInactivos, db);
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
                roles,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los tipos de compra por compañía.
        /// </summary>
        /// <param name="CodigoCompania">Identificador de la compañía de la que se desean obtener los tipos de compra.</param>
        public JsonResult ObtenerTiposCompra(string CodigoCompania)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<TipoCompraViewModel> TiposCompra = null;

            try
            {
                TiposCompra = RolGestorCompraBLL.ObtenerTiposCompra(sesion, CodigoCompania);
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
                TiposCompra,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para crear un nuevo rol de gestor de compra.
        /// </summary>
        /// <param name="Rol">Objeto que contiene los datos del nuevo rol.</param>
        [HttpPost]
        public JsonResult Create(RolGestorCompra Rol)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int cont = db.RolesGestorCompra.Where(x => x.CodigoEmpresa == Rol.CodigoEmpresa && x.CodigoTipoCompra == Rol.CodigoTipoCompra && x.EstadoId == (int)EnumEstado.ACTIVO).Count();

                        if (cont > 0 && Rol.EstadoId == (int)EnumEstado.ACTIVO)
                        {
                            validacion.Add("Ya se ha configurado un rol para ese tipo de compra.");
                        }
                        else
                        {
                            db.RolesGestorCompra.Add(Rol);
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
                Rol.Id
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener el detalle de un rol de gestor de compra.
        /// </summary>
        /// <param name="Id">Identificador del rol de gestor de compra del que se desea obtener el detalles.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            RolGestorCompraViewModel rol = null;

            try
            {
                rol = RolGestorCompraBLL.ObtenerRolGestorCompra(Id, db);

                rol.GestorSierra = RolGestorCompraBLL.ObtenerGestor(rol.UsuarioGestorSierra, sesion);
                rol.GestorCosta = RolGestorCompraBLL.ObtenerGestor(rol.UsuarioGestorCosta, sesion);
                rol.GestorAustro = RolGestorCompraBLL.ObtenerGestor(rol.UsuarioGestorAustro, sesion);

                rol.GestorSierra.JefeInmediato = null;
                rol.GestorCosta.JefeInmediato = null;
                rol.GestorAustro.JefeInmediato = null;
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
                rol,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para editar un rol de gestor de compra.
        /// </summary>
        /// <param name="Rol">Objeto que contiene la información del rol editado.</param>
        [HttpPost]
        public JsonResult Edit(RolGestorCompra Rol)
        {
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int cont = db.RolesGestorCompra.Where(x => x.CodigoEmpresa == Rol.CodigoEmpresa && x.CodigoTipoCompra == Rol.CodigoTipoCompra && x.EstadoId == (int)EnumEstado.ACTIVO && Rol.Id != x.Id).Count();

                        if (cont > 0 && Rol.EstadoId == (int)EnumEstado.ACTIVO)
                        {
                            validacion.Add("Ya se ha configurado un rol para ese tipo de compra.");
                        }
                        else
                        {
                            db.Entry(Rol).State = EntityState.Modified;

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
    }
}