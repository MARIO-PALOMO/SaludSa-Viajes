using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Common.ViewModels;
using Common.Utilities;
using Rest;
using System.Net;
using Logic;
using Data.Context;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para manejar la autenticación y desconección.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        ContenedorVariablesSesion sesion;
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Pantalla de inicio de login.
        /// </summary>
        /// <param name="returnUrl">Url de retorno o redirección.</param>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session["vars"] = new ContenedorVariablesSesion();

            return View();
        }

        /// <summary>
        /// Proceso de autenticación.
        /// </summary>
        /// <param name="model">Objeto que contiene el usuario y la contraseña.</param>
        /// <param name="returnUrl">Url de retorno o redirección.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(
            LoginViewModel model, 
            string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            sesion = Session["vars"] as ContenedorVariablesSesion;

            if (sesion == null)
            {
                Session["vars"] = new ContenedorVariablesSesion();
                sesion = Session["vars"] as ContenedorVariablesSesion;
            }

            sesion.SistemaOperativo = Request.Browser.Platform.PadRight(6, 'x');
            sesion.DispositivoNavegador = Request.Browser.Browser.PadRight(6, 'x');
            //IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            //sesion.DireccionIP = ips[1].ToString();

            sesion.DireccionIP = "10.10.10.10";

            var UrlServicios = ConfigurationManager.AppSettings["Url"].ToString();
            var Usuario = ConfigurationManager.AppSettings["Usuario"].ToString();
            var Clave = ConfigurationManager.AppSettings["Clave"].ToString();
            var ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();
            var CodigoAplicacion = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();

            var EmailDestinatario = ConfigurationManager.AppSettings["EmailDestinatario"].ToString();
            var EmailNombreOrigen = ConfigurationManager.AppSettings["EmailNombreOrigen"].ToString();
            var EmailEmailOrigen = ConfigurationManager.AppSettings["EmailEmailOrigen"].ToString();
            var EmailTiempoEspera = ConfigurationManager.AppSettings["EmailTiempoEspera"].ToString();

            var UsuarioAcceso = ConfigurationManager.AppSettings["UsuarioAcceso"].ToString();

            var UrlVisorRidePdf = ConfigurationManager.AppSettings["UrlVisorRidePdf"].ToString();

            var IdClaseMFiles = ConfigurationManager.AppSettings["IdClaseMFiles"].ToString();

            sesion.UrlServicios = UrlServicios;
            sesion.CodigoAplicacion = CodigoAplicacion;
            sesion.ClientId = ClientId;
            sesion.UsuarioSistema = Usuario;
            sesion.ClaveSistema = Clave;

            sesion.EmailDestinatario = EmailDestinatario;
            sesion.EmailNombreOrigen = EmailNombreOrigen;
            sesion.EmailEmailOrigen = EmailEmailOrigen;
            sesion.EmailTiempoEspera = EmailTiempoEspera;

            var urlBuilder =new System.UriBuilder(Request.Url.AbsoluteUri) {
                Path = Url.Action("Edit", "Tarea"),
                Query = null,
            };

            //Uri uri = urlBuilder.Uri;
            sesion.UrlWeb = urlBuilder.ToString();

            var urlBuilderPago = new System.UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Action("Edit", "TareaPago"),
                Query = null,
            };

            sesion.UrlWebPago = urlBuilderPago.ToString();

            sesion.UrlVisorRidePdf = UrlVisorRidePdf;

            sesion.IdClaseMFiles = IdClaseMFiles;

            sesion.OrigenLog = "WebIngenesisCompras";

            var token = AccountBLL.ObtenerToken(UrlServicios, Usuario, Clave, ClientId, sesion.OrigenLog);

            if(token == null || token.error != null)
            {
                if(token.error_description != null)
                {
                    ModelState.AddModelError("", token.error_description + " (" + token.error + ").");
                }
                else
                {
                    ModelState.AddModelError("", "Error de credenciales del sistema (" + token.error + ").");
                }
                
                return View(model);
            }

            var validacionCredenciales = AccountBLL.ValidarCredenciales(model.Username, model.Password, sesion);

            if (validacionCredenciales != null)
            {
                switch(validacionCredenciales.Estado)
                {
                    case "OK":
                        sesion.token = token;
                        UsuarioViewModel usuario = null;
                        try
                        {
                            sesion.RolesAdministrativos = RolAdministrativoBLL.ObtenerRolesAdministrativosPorUsuario(model.Username, db);

                            if (UsuarioAcceso != null && UsuarioAcceso.Trim() != string.Empty)
                            {
                                usuario = AccountBLL.ObtenerUsuario(UsuarioAcceso, sesion);
                            }
                            else
                            {
                                usuario = AccountBLL.ObtenerUsuario(model.Username, sesion);
                            }
                        }
                        catch(Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                            return View(model);
                        }

                        sesion.usuario = usuario;
                        return RedirectToLocal(returnUrl);
                    case "Error":
                        if (validacionCredenciales.Mensajes != null)
                        {
                            foreach (var error in validacionCredenciales.Mensajes)
                            {
                                ModelState.AddModelError("", error);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                        }
                        return View(model);
                    default:
                        if(validacionCredenciales.Mensajes != null)
                        {
                            foreach(var error in validacionCredenciales.Mensajes)
                            {
                                ModelState.AddModelError("", error);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                        }
                        return View(model);
                }
            }
            
            ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
            return View(model);
        }

        /// <summary>
        /// Proveso de redirección.
        /// </summary>
        /// <param name="returnUrl">Url de retorno o redirección.</param>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Proceso de desconectarse.
        /// </summary>
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session["vars"] = new ContenedorVariablesSesion();
            return RedirectToAction("Login", "Account");
        }
    }
}