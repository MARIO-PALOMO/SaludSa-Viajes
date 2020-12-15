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
    /// Controlador del módulo de roles administrativos.
    /// </summary>
    [SesionFilter]
    [RolFilter]
    public class RolAdministrativoController : Controller
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
        /// Proceso para obtener los datos requeridos en la interfaz de gestión de roles administrativos.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<RolViewModel> Roles = null;
            List<UsuarioViewModel> Colaboradores = null;
            List<EmpresaViewModel> Empresas = null;
            List<CiudadViewModel> Ciudades = null;

            try
            {
                string[] grupos = new string[] { "GR Jefes", "GR Gestor Compras" };

                Roles = RolAdministrativoBLL.ObtenerRoles(db);
                Colaboradores = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(grupos, sesion);
                Empresas = SolicitudCompraBLL.ObtenerEmpresas(sesion);
                Ciudades = RolAdministrativoBLL.ObtenerCiudades(db);
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
                Roles,
                Colaboradores,
                Empresas,
                Ciudades,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener los roles administrativos.
        /// </summary>
        public JsonResult ObtenerRolesAdministrativos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<RolAdministrativoViewModel> roles = null;

            try
            {
                roles = RolAdministrativoBLL.ObtenerRolesAdministrativos(db);
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
                roles,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para crear un nuevo rol administrativo.
        /// </summary>
        /// <param name="Rol">Objeto que contiene los datos del nuevo rol administrativo.</param>
        [HttpPost]
        public JsonResult Create(RolAdministrativo Rol)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Rol.CreadoPor = sesion.usuario.Usuario;
                        Rol.FechaCreacion = DateTime.Now;

                        db.RolesAdministrativo.Add(Rol);
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
                Rol.Id
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para obtener el detalle de un rol administrativo.
        /// </summary>
        /// <param name="Id">Identificador del rol administrativo del que se quiere obtener el detalle.</param>
        public JsonResult Detalle(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            RolAdministrativoViewModel rol = null;

            try
            {
                rol = RolAdministrativoBLL.ObtenerRolAdministrativo(Id, db);
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
                rol,
                validacion
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso para editar un rol administrativo.
        /// </summary>
        /// <param name="Rol">Objeto que contiene los datos del rol administrativo editado.</param>
        [HttpPost]
        public JsonResult Edit(RolAdministrativo Rol)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            if (ModelState.IsValid)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(Rol).State = EntityState.Modified;

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

        /// <summary>
        /// Proceso para eliminar un rol administrativo.
        /// </summary>
        /// <param name="Id">Identificador del rol administrativo que se desea eliminar.</param>
        [HttpPost]
        public JsonResult Eliminar(long Id)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            var Rol = db.RolesAdministrativo.Find(Id);

            if(Rol != null)
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.RolesAdministrativo.Remove(Rol);

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
            else
            {
                validacion.Add("El rol administrativo no existe.");
            }

            return Json(new
            {
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}