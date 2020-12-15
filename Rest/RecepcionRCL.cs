using Common.Utilities;
using Common.ViewModels;
using Data.Entidades;
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
    /// Cliente rest para recepciones.
    /// </summary>
    public class RecepcionRCL
    {
        /// <summary>
        /// Proceso para anular una recepción.
        /// </summary>
        /// <param name="NumeroOrdenHija">Número de la orden hija.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void AnularRecepcion(
            string NumeroOrdenHija, 
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/AnularRecepcion", Method.POST);

            reques.AddQueryParameter("ordenCompraHija", NumeroOrdenHija);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

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
                origin = "ServicioAdministracion/api/DynamicsAx/AnularRecepcion",
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

                EventLogUtil.EscribirLogErrorWeb(json, 31, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion//api/DynamicsAx/AnularRecepcion\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 31, sesion.OrigenLog);

                        throw new Exception(mensaje);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 31, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 31, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion//api/DynamicsAx/AnularRecepcion");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 31, sesion.OrigenLog);
        }
    }
}
