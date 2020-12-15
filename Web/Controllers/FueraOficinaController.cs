using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.FilterAttribute;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para el módulo fuera de oficina.
    /// </summary>
    [SesionFilter]
    public class FueraOficinaController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio del módulo.
        /// </summary>
        /// <param name="mensaje">Mensaje de notificación a visualizar.</param>
        public ActionResult Index(string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;

            return View();
        }

        /// <summary>
        /// Proceso para obtener los datos necesarios en la interfaz de configuración de fuera de oficina.
        /// </summary>
        public JsonResult ObtenerMetadatos()
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            List<UsuarioViewModel> Usuarios = null;
            UsuarioViewModel UsuarioReasignacion = null;

            try
            {
                string[] Grupo = new string[] { "GR Jefes", "GR Gestor Compras" };
                Usuarios = SolicitudCompraBLL.ObtenerUsuariosPorGrupo(Grupo, sesion);

                UsuarioReasignacion = FueraOficinaBLL.ObtenerUsuarioReasignacion(sesion.usuario.Usuario, sesion, db);
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
                Usuarios,
                UsuarioReasignacion,
                validacion
            }, JsonRequestBehavior.AllowGet);

            json.MaxJsonLength = Int32.MaxValue;

            return json;
        }

        /// <summary>
        /// Proceso para editar la configuración de fuera de oficina de un usuario.
        /// </summary>
        /// <param name="Proceso">Proceso que se desea configurar (1 - En la oficina; 2 - Fuera de oficina).</param>
        /// <param name="Usuario">Usuario al que se reasignarán las tareas.</param>
        [HttpPost]
        public JsonResult Edit(
            int Proceso, 
            string Usuario)
        {
            sesion = Session["vars"] as ContenedorVariablesSesion;
            List<string> validacion = new List<string>();

            try
            {
                var item = db.Usuarios.Where(x => x.NombreUsuario == sesion.usuario.Usuario).FirstOrDefault();

                if (item != null)
                {
                    db.Usuarios.Remove(item);
                    db.SaveChanges();
                }

                if (Proceso == 2)
                {
                    if(Usuario == null || Usuario == string.Empty)
                    {
                        validacion.Add("No se ha enviado un usuario para reasignación de las tareas.");
                    }
                    else
                    {
                        Data.Entidades.Usuario nuevo = new Data.Entidades.Usuario()
                        {
                            NombreUsuario = sesion.usuario.Usuario,
                            EnLaOficina = false,
                            UsuarioReasignar = Usuario,
                            EstadoId = (int)EnumEstado.ACTIVO
                        };

                        db.Usuarios.Add(nuevo);

                        db.SaveChanges();
                    }
                }
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
                validacion
            }, JsonRequestBehavior.AllowGet);
        }
    }
}