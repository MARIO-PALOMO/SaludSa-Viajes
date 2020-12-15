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
using System.Web;

namespace Rest
{
    /// <summary>
    /// Cliente rest para gestión de archivos en MFiles.
    /// </summary>
    public class MFilesRCL
    {
        /// <summary>
        /// Proceso para cargar a MFiles un adjunto.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase del adjunto.</param>
        /// <param name="NombreArchivo">Nombre del adjunto.</param>
        /// <param name="Adjunto">Objeto que contiene los datos del adjunto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void CargarAdjunto(
            string IdClase, 
            string NombreArchivo, 
            AdjuntoViewModel Adjunto, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioGestionDocumentos/api/Archivos/Cargar", Method.POST);

            reques.AddQueryParameter("idClase", IdClase);
            reques.AddQueryParameter("nombreArchivo", NombreArchivo);
            
            var json = JsonConvert.SerializeObject(Adjunto);
            reques.AddParameter("contenedorObjeto", json, "application/json", ParameterType.RequestBody);

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
                origin = "ServicioGestionDocumentos/api/Archivos/Cargar",
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

                EventLogUtil.EscribirLogErrorWeb(json, 13, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioGestionDocumentos/api/Archivos/Cargar\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 13, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 13, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 13, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioGestionDocumentos/api/Archivos/Cargar");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 13, sesion.OrigenLog);
        }

        /// <summary>
        /// Proceso para buscar adjuntos.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase de los adjuntos.</param>
        /// <param name="PropiedadesBusqueda">Listado con propiedades de búsqueda.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<AdjuntoViewModel></returns>
        public static List<AdjuntoViewModel> BuscarAdjuntos(
            string IdClase, 
            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioGestionDocumentos/api/Objetos/Busqueda", Method.POST);

            reques.AddQueryParameter("idClase", IdClase);

            var json = JsonConvert.SerializeObject(PropiedadesBusqueda);
            reques.AddParameter("parametrosBusqueda", json, "application/json", ParameterType.RequestBody);

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
                origin = "ServicioGestionDocumentos/api/Objetos/Busqueda",
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

            List<AdjuntoViewModel> adjuntos = new List<AdjuntoViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 14, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioGestionDocumentos/api/Objetos/Busqueda\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
            }
            else
            {
                var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                if (Content != null && Content.Mensajes.Count() == 1 && Content.Mensajes.ElementAt(0) == "No existen resultados de búsqueda")
                {
                    return adjuntos;
                }
                else if (Content != null && Content.Datos != null)
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

                        EventLogUtil.EscribirLogErrorWeb(json, 14, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        adjuntos = JsonConvert.DeserializeObject<List<AdjuntoViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 14, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 14, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioGestionDocumentos/api/Objetos/Busqueda");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 14, sesion.OrigenLog);

            return adjuntos;
        }

        /// <summary>
        /// Proceso para descargar un adjunto.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase del adjunto.</param>
        /// <param name="PropiedadesBusqueda">Listado con propiedades de búsqueda.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>AdjuntoViewModel</returns>
        public static AdjuntoViewModel DescargarAdjunto(
            string IdClase, 
            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioGestionDocumentos/api/Archivos/Descarga", Method.POST);

            reques.AddQueryParameter("idClase", IdClase);

            var json = JsonConvert.SerializeObject(PropiedadesBusqueda);
            reques.AddParameter("parametrosBusqueda", json, "application/json", ParameterType.RequestBody);

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
                origin = "ServicioGestionDocumentos/api/Archivos/Descarga",
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

            AdjuntoViewModel adjunto = new AdjuntoViewModel();

            if(response.StatusCode != System.Net.HttpStatusCode.OK)
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

                EventLogUtil.EscribirLogErrorWeb(json, 16, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioGestionDocumentos/api/Archivos/Descarga\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
            }
            else
            {
                var Content = JsonConvert.DeserializeObject<RespuestaRestViewModel>(response.Content);

                if(Content != null && Content.Datos != null)
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

                        EventLogUtil.EscribirLogErrorWeb(json, 16, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        ArchivoViewModel archivo = JsonConvert.DeserializeObject<ArchivoViewModel>(Content.Datos.ToString());                        
                        adjunto.ContenidoArchivo = archivo.Contenido;
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

                        EventLogUtil.EscribirLogErrorWeb(json, 16, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 16, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioGestionDocumentos/api/Archivos/Descarga");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 16, sesion.OrigenLog);

            return adjunto;
        }

        /// <summary>
        /// Proceso para modificar en MFiles las propiedades de un adjunto.
        /// </summary>
        /// <param name="idTipoObjeto">Identificador de tipo de objeto en MFiles.</param>
        /// <param name="idObjeto">Identificador de objeto en MFiles.</param>
        /// <param name="PropiedadesModificar">Listado de propiedades a modificar.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>AdjuntoViewModel</returns>
        public static AdjuntoViewModel ModificarPropiedadesAdjunto(
            string idTipoObjeto, 
            string idObjeto, 
            List<PropiedadAdjuntoViewModel> PropiedadesModificar, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioGestionDocumentos/api/Objetos/ActualizarPropiedades", Method.PUT);

            reques.AddQueryParameter("idTipoObjeto", idTipoObjeto);
            reques.AddQueryParameter("idObjeto", idObjeto);

            var json = JsonConvert.SerializeObject(PropiedadesModificar);
            reques.AddParameter("propiedadesActualizadas", json, "application/json", ParameterType.RequestBody);

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
                origin = "ServicioGestionDocumentos/api/Objetos/ActualizarPropiedades",
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

            AdjuntoViewModel adjunto = new AdjuntoViewModel();

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

                EventLogUtil.EscribirLogErrorWeb(json, 17, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioGestionDocumentos/api/Objetos/ActualizarPropiedades\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 17, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        adjunto = JsonConvert.DeserializeObject<AdjuntoViewModel>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 17, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 17, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioGestionDocumentos/api/Objetos/ActualizarPropiedades");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 17, sesion.OrigenLog);

            return adjunto;
        }
    }
}
