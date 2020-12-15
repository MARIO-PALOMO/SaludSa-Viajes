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
    /// Cliente rest para tipos de compra.
    /// </summary>
    public class TipoCompraRCL
    {
        /// <summary>
        /// Proceso para obtener los tipos de compra de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<TipoCompraViewModel></returns>
        public static List<TipoCompraViewModel> ObtenerTiposCompra(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerTipoCompra", Method.GET);

            reques.AddParameter("codigoCompania", CompaniaCodigo);

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
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerTipoCompra",
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

            List<TipoCompraViewModel> tiposCompra = new List<TipoCompraViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 32, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerTipoCompra\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 32, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        tiposCompra = JsonConvert.DeserializeObject<List<TipoCompraViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 32, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 32, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerTipoCompra");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 32, sesion.OrigenLog);

            return tiposCompra;
        }
    }
}
