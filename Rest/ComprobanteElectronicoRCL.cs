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
    /// Cliente rest para comprobantes electrónicos.
    /// </summary>
    public class ComprobanteElectronicoRCL
    {
        /// <summary>
        /// Proceso para buscar comprobantes electrónicos.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="tipoDocumento">Filtro de tipo de documento.</param>
        /// <param name="fechaInicio">Filtro de fecha de inicio.</param>
        /// <param name="fechaFin">Filtro de fecha de fin.</param>
        /// <param name="ruc">Filtro de ruc.</param>
        /// <param name="establecimiento">Filtro de establecimiento.</param>
        /// <param name="puntoEmision">Filtro de punto de emisión.</param>
        /// <param name="secuencial">Filtro de secuencial.</param>
        /// <returns>List<ComprobanteElectronicoViewModel></returns>
        public static List<ComprobanteElectronicoViewModel> Buscar(
            ContenedorVariablesSesion sesion, 
            string tipoDocumento,
            string fechaInicio,
            string fechaFin,
            string ruc,
            string establecimiento,
            string puntoEmision,
            string secuencial)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioRepositorio/api/Documento/Buscar", Method.GET);

            reques.AddQueryParameter("tipoDocumento", tipoDocumento);
            reques.AddQueryParameter("fechaInicio", fechaInicio);
            reques.AddQueryParameter("fechaFin", fechaFin);
            reques.AddQueryParameter("ruc", ruc);
            reques.AddQueryParameter("establecimiento", establecimiento);
            reques.AddQueryParameter("puntoEmision", puntoEmision);
            reques.AddQueryParameter("secuencial", secuencial);

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
                origin = "ServicioRepositorio/api/Documento/Buscar",
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

            List<ComprobanteElectronicoViewModel> comprobantesElectronicos = new List<ComprobanteElectronicoViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 53, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioRepositorio/api/Documento/Buscar\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 53, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        try
                        {
                            var tem = JsonConvert.DeserializeObject<ComprobanteElectronicoNewViewModel>(Content.Datos.ToString());
                            comprobantesElectronicos = tem.Entidades;
                        }
                        catch (Exception ex1)
                        {
                            try
                            {
                                var NoDocumentos = JsonConvert.DeserializeObject<ComprobanteElectronicoRespuestaSinDocumentosViewModel>(Content.Datos.ToString());
                            }
                            catch (Exception)
                            {
                                EventLogUtil.EscribirLogErrorWeb(json, 53, sesion.OrigenLog);

                                throw ex1;
                            }
                        }
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

                        EventLogUtil.EscribirLogErrorWeb(json, 53, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 53, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioRepositorio/api/Documento/Buscar");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 53, sesion.OrigenLog);

            return comprobantesElectronicos;
        }

        /// <summary>
        /// Proceso para obtener el XML de un comprobante electrónico.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="claveAcceso">Clave de acceso del comprobante.</param>
        /// <returns>string</returns>
        public static string ObtenerXml(
            ContenedorVariablesSesion sesion, 
            string claveAcceso)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioRepositorio/api/Documento/ObtenerXml", Method.GET);

            reques.AddQueryParameter("claveAcceso", claveAcceso);

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
                origin = "ServicioRepositorio/api/Documento/ObtenerXml",
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

            string xml = string.Empty;

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

                EventLogUtil.EscribirLogErrorWeb(json, 54, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioRepositorio/api/Documento/ObtenerXml\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 54, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        xml = Content.Datos.ToString();
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

                        EventLogUtil.EscribirLogErrorWeb(json, 54, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 54, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioRepositorio/api/Documento/ObtenerXml");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 54, sesion.OrigenLog);

            return xml;
        }

        /// <summary>
        /// Proceso para cambiar el estado de un comprobante electrónico.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="ruc">Filtro de ruc.</param>
        /// <param name="establecimiento">Filtro de establecimiento.</param>
        /// <param name="puntoEmision">Filtro de punto de emisión.</param>
        /// <param name="secuencial">Filtro de secuencial.</param>
        /// <param name="tipoDocumento">Filtro de tipo de documento.</param>
        /// <param name="estadoDocumento">Nuevo estado.</param>
        /// <param name="codigoSistema">Filtro de código de sistema.</param>
        /// <param name="usuario">Filtro de usuario.</param>
        public static void CambiarEstado(
            ContenedorVariablesSesion sesion,
            string ruc,
            string establecimiento,
            string puntoEmision,
            string secuencial,
            string tipoDocumento,
            int estadoDocumento,
            int codigoSistema,
            string usuario)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioRepositorio/api/Documento/CambiarEstado", Method.GET);

            reques.AddQueryParameter("ruc", ruc);
            reques.AddQueryParameter("establecimiento", establecimiento);
            reques.AddQueryParameter("puntoEmision", puntoEmision);
            reques.AddQueryParameter("secuencial", secuencial);
            reques.AddQueryParameter("tipoDocumento", tipoDocumento);
            reques.AddQueryParameter("estadoDocumento", estadoDocumento.ToString());
            reques.AddQueryParameter("codigoSistema", codigoSistema.ToString());
            reques.AddQueryParameter("usuario", usuario);

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
                origin = "ServicioRepositorio/api/Documento/CambiarEstado",
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

                EventLogUtil.EscribirLogErrorWeb(json, 55, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioRepositorio/api/Documento/CambiarEstado\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 55, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        bool resultado = (bool)Content.Datos;

                        if (resultado != true)
                        {
                            EventLogUtil.EscribirLogErrorWeb(json, 55, sesion.OrigenLog);

                            throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioRepositorio/api/Documento/CambiarEstado.");
                        }
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

                        EventLogUtil.EscribirLogErrorWeb(json, 55, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 55, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioRepositorio/api/Documento/CambiarEstado");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 55, sesion.OrigenLog);
        }

        /// <summary>
        /// Proceso para obtener el objeto comprobante completo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="claveAcceso">Clave de acceso del comprobante.</param>
        /// <returns>ComprobanteElectronicoJsonViewModel</returns>
        public static ComprobanteElectronicoJsonViewModel ObtenerFactura(
            ContenedorVariablesSesion sesion, 
            string claveAcceso)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioRepositorio/api/Documento/ObtenerFactura", Method.GET);

            reques.AddQueryParameter("claveAcceso", claveAcceso);

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
                origin = "ServicioRepositorio/api/Documento/ObtenerFactura",
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

            ComprobanteElectronicoJsonViewModel factura = new ComprobanteElectronicoJsonViewModel();

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

                EventLogUtil.EscribirLogErrorWeb(json, 56, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioRepositorio/api/Documento/ObtenerFactura\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 56, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        factura = JsonConvert.DeserializeObject<ComprobanteElectronicoJsonViewModel>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 56, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 56, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioRepositorio/api/Documento/ObtenerFactura");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 56, sesion.OrigenLog);

            return factura;
        }
    }
}
