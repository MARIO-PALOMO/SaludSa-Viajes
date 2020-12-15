using Common.Utilities;
using Common.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest
{
    /// <summary>
    /// Cliente rest para usuarios.
    /// </summary>
    public class UsuarioRCL
    {
        /// <summary>
        /// Proceso para obtener un usuario.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>UsuarioViewModel</returns>
        public static UsuarioViewModel ObtenerUsuario(
            string username, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioDirectorioActivo/api/ObtenerUsuario", Method.GET);

            reques.AddParameter("usuario", username);
            reques.AddParameter("jefe", true);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioDirectorioActivo/api/ObtenerUsuario",
                user = sesion.usuario != null ? sesion.usuario.Usuario : null,
                request = reques.Parameters,
                response = new
                {
                    response.StatusCode,
                    response.StatusDescription,
                    response.ResponseStatus,
                    response.Content
                }
            });

            UsuarioViewModel usuario = new UsuarioViewModel();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string mensaje = string.Empty;

                try
                {
                    var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                    if (Content.Mensajes != null)
                    {
                        foreach (var mens in Content.Mensajes)
                        {
                            mensaje += mens + " ";
                        }
                    }
                }
                catch (Exception)
                {

                }

                EventLogUtil.EscribirLogErrorWeb(json, 33, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioDirectorioActivo/api/ObtenerUsuario\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
            }
            else
            {
                var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                if (Content != null && Content.Datos != null)
                {
                    if (Content.Estado == "Error")
                    {
                        string mensaje = string.Empty;

                        if (Content.Mensajes != null)
                        {
                            foreach (var mens in Content.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 33, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        usuario = JsonConvert.DeserializeObject<UsuarioViewModel>(Content.Datos.ToString());
                    }
                }
                else
                {
                    if (Content != null && Content.Estado != null && Content.Mensajes != null)
                    {
                        string mensaje = string.Empty;

                        if (Content.Mensajes != null)
                        {
                            foreach (var mens in Content.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 33, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 33, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioDirectorioActivo/api/ObtenerUsuario");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 33, sesion.OrigenLog);

            return usuario;
        }

        /// <summary>
        /// Proceso para obtener los usuarios que pertenecen a determinados grupos.
        /// </summary>
        /// <param name="grupos">Listado de grupos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<UsuarioViewModel></returns>
        public static List<UsuarioViewModel> ObtenerUsuariosPorGrupo(
            string[] grupos, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioDirectorioActivo/api/ObtenerUsuariosGrupo", Method.POST);

            var json = JsonConvert.SerializeObject(grupos);
            reques.AddParameter("nombreGrupos", json, "application/json", ParameterType.RequestBody);

            reques.AddHeader("Content-Type", "application/json");
            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioDirectorioActivo/api/ObtenerUsuariosGrupo",
                user = sesion.usuario != null ? sesion.usuario.Usuario : null,
                request = reques.Parameters,
                response = new
                {
                    response.StatusCode,
                    response.StatusDescription,
                    response.ResponseStatus,
                    response.Content
                }
            });

            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string mensaje = string.Empty;

                try
                {
                    var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                    if (Content.Mensajes != null)
                    {
                        foreach (var mens in Content.Mensajes)
                        {
                            mensaje += mens + " ";
                        }
                    }
                }
                catch (Exception)
                {

                }

                EventLogUtil.EscribirLogErrorWeb(json, 34, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioDirectorioActivo/api/ObtenerUsuariosGrupo\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
            }
            else
            {
                var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                if (Content != null && Content.Datos != null)
                {
                    if (Content.Estado == "Error")
                    {
                        string mensaje = string.Empty;

                        if (Content.Mensajes != null)
                        {
                            foreach (var mens in Content.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 34, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        usuarios = JsonConvert.DeserializeObject<List<UsuarioViewModel>>(Content.Datos.ToString());
                    }
                }
                else
                {
                    if (Content != null && Content.Estado != null && Content.Mensajes != null)
                    {
                        string mensaje = string.Empty;

                        if (Content.Mensajes != null)
                        {
                            foreach (var mens in Content.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 34, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 34, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioDirectorioActivo/api/ObtenerUsuariosGrupo");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 34, sesion.OrigenLog);

            return usuarios;
        }
    }
}
