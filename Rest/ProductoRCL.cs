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
    /// Cliente rest para productos.
    /// </summary>
    public class ProductoRCL
    {
        /// <summary>
        /// Proceso para obtener los productos de mercadeo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<ProductoMercadeoViewModel></returns>
        public static List<ProductoMercadeoViewModel> ObtenerProductosMercadeo(ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerProductoMercadeo", Method.GET);

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
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerProductoMercadeo",
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

            List<ProductoMercadeoViewModel> productos = new List<ProductoMercadeoViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 26, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerProductoMercadeo\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 26, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        productos = JsonConvert.DeserializeObject<List<ProductoMercadeoViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 26, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 26, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerProductoMercadeo");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 26, sesion.OrigenLog);

            return productos;
        }

        /// <summary>
        /// Proceso para obtener los productos de un tipo y de una compañía.
        /// </summary>
        /// <param name="tipoArticulo">Tipo de producto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<ProductoViewModel></returns>
        public static List<ProductoViewModel> ObtenerProductosPorTipo(
            string tipoArticulo, 
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerArticulo", Method.GET);

            reques.AddParameter("codigoCompania", CompaniaCodigo); 
            reques.AddParameter("tipoArticulo", tipoArticulo);

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
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerArticulo",
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

            List<ProductoViewModel> productos = new List<ProductoViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 27, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerArticulo\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 27, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 27, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 27, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerArticulo");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 27, sesion.OrigenLog); 

            return productos;
        }

        /// <summary>
        /// Proceso para obtener un producto.
        /// </summary>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="codigoArticulo">Identificador del producto.</param>
        /// <param name="codigoGrupo">Identificador del grupo de producto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>ProductoViewModel</returns>
        public static ProductoViewModel ObtenerProducto(
            string codigoCompania, 
            string codigoArticulo, 
            string codigoGrupo, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerArticuloPorCodigo", Method.GET);

            reques.AddParameter("codigoCompania", codigoCompania);
            reques.AddParameter("codigoArticulo", codigoArticulo);
            reques.AddParameter("codigoGrupoArticulo", codigoGrupo);

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
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerArticuloPorCodigo",
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

            ProductoViewModel producto = new ProductoViewModel();

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

                EventLogUtil.EscribirLogErrorWeb(json, 28, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerArticuloPorCodigo\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 28, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        producto = JsonConvert.DeserializeObject<ProductoViewModel>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 28, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 28, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerArticuloPorCodigo");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 28, sesion.OrigenLog);

            return producto;
        }
    }
}
