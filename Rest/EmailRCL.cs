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
    /// Cliente rest para envío de emails.
    /// </summary>
    public class EmailRCL
    {
        /// <summary>
        /// Proceso para enviar un email sin adjunto.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="email">Objeto que contiene los datos del email.</param>
        /// <returns>EmailRespuestaViewModel</returns>
        public static EmailRespuestaViewModel Enviar(
            ContenedorVariablesSesion sesion, 
            EmailViewModel email)
        {
            EmailRespuestaViewModel respuesta = new EmailRespuestaViewModel();

            try
            {
                RestClient cliente = new RestClient(sesion.UrlServicios);

                var reques = new RestRequest("ServicioComunicaciones/api/CorreoElectronico/Enviar", Method.POST);

                var obj = JsonConvert.SerializeObject(email);
                reques.AddParameter("mensaje", obj, "application/json", ParameterType.RequestBody);

                reques.AddHeader("Content-Type", "application/json");

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
                    origin = "ServicioComunicaciones/api/CorreoElectronico/Enviar",
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

                    EventLogUtil.EscribirLogErrorWeb(json, 9, sesion.OrigenLog);

                    throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioComunicaciones/api/CorreoElectronico/Enviar\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                            EventLogUtil.EscribirLogErrorWeb(json, 9, sesion.OrigenLog);

                            throw new Exception(mensaje);
                        }
                        else
                        {
                            respuesta = JsonConvert.DeserializeObject<EmailRespuestaViewModel>(Content.Datos.ToString());
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

                            EventLogUtil.EscribirLogErrorWeb(json, 9, sesion.OrigenLog);

                            throw new Exception(mensaje);
                        }
                        else
                        {
                            EventLogUtil.EscribirLogErrorWeb(json, 9, sesion.OrigenLog);

                            throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioComunicaciones/api/CorreoElectronico/Enviar");
                        }
                    }
                }

                EventLogUtil.EscribirLogInfoWeb(json, 9, sesion.OrigenLog);

                return respuesta;
            }
            catch(Exception ex)
            {
                respuesta = new EmailRespuestaViewModel();

                respuesta.Mensajes.Add(ex.Message);

                return respuesta;
            }
        }

        /// <summary>
        /// Proceso para enviar un email con adjunto.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="email">Objeto que contiene los datos del email.</param>
        /// <returns>EmailRespuestaViewModel</returns>
        public static EmailRespuestaViewModel EnviarConAdjunto(
            ContenedorVariablesSesion sesion, 
            EmailAdjuntoViewModel email)
        {
            EmailRespuestaViewModel respuesta = new EmailRespuestaViewModel();

            try
            {
                RestClient cliente = new RestClient(sesion.UrlServicios);

                var reques = new RestRequest("ServicioComunicaciones/api/CorreoElectronico/EnviarAdjuntos", Method.POST);

                var json = JsonConvert.SerializeObject(email);
                reques.AddParameter("mensaje", json, "application/json", ParameterType.RequestBody);

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
                    origin = "ServicioComunicaciones/api/CorreoElectronico/EnviarAdjuntos",
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

                    EventLogUtil.EscribirLogErrorWeb(json, 10, sesion.OrigenLog);

                    throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioComunicaciones/api/CorreoElectronico/EnviarAdjuntos\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                            EventLogUtil.EscribirLogErrorWeb(json, 10, sesion.OrigenLog);

                            throw new Exception(mensaje);
                        }
                        else
                        {
                            respuesta = JsonConvert.DeserializeObject<EmailRespuestaViewModel>(Content.Datos.ToString());
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

                            EventLogUtil.EscribirLogErrorWeb(json, 10, sesion.OrigenLog);

                            throw new Exception(mensaje);
                        }
                        else
                        {
                            EventLogUtil.EscribirLogErrorWeb(json, 10, sesion.OrigenLog);

                            throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioComunicaciones/api/CorreoElectronico/EnviarAdjuntos");
                        }
                    }
                }

                EventLogUtil.EscribirLogInfoWeb(json, 10, sesion.OrigenLog);

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta = new EmailRespuestaViewModel();

                respuesta.Mensajes.Add(ex.Message);

                return respuesta;
            }            
        }
    }
}
