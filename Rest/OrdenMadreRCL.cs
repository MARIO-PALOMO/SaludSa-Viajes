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
    /// Cliente rest para órdenes madre.
    /// </summary>
    public class OrdenMadreRCL
    {
        /// <summary>
        /// Proceso para crear ordenes madre.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="Datos">Listado de ordenes madre.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>List<OrdenMadre></returns>
        public static List<OrdenMadre> CrearOrdenesCompraMadre(
            ContenedorVariablesSesion sesion, 
            List<OrdenMadreViewModel> Datos, 
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/CrearOrdenesCompraMadre", Method.POST);

            reques.AddQueryParameter("codigoCompania", codigoCompania);

            var json = JsonConvert.SerializeObject(Datos);
            reques.AddParameter("proveedores", json, "application/json", ParameterType.RequestBody);

            reques.AddHeader("Content-Type", "application/json");

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            var response = cliente.Execute<RespuestaRestViewModel>(reques);

            json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/CrearOrdenesCompraMadre",
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

            List<OrdenMadre> OrdenesMadre = new List<OrdenMadre>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 25, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/CrearOrdenesCompraMadre\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
            }
            else
            {
                if (response != null && response.Data.Datos != null)
                {
                    if (response.Data.Estado == "Error")
                    {
                        string mensaje = string.Empty;

                        if (response.Data.Mensajes != null)
                        {
                            foreach (var mens in response.Data.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 25, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        Dictionary<System.String, System.Object> datos1 = (Dictionary<System.String, System.Object>)response.Data.Datos;
                        JsonArray datos2 = (JsonArray)datos1.ElementAt(0).Value;
                        OrdenesMadre = JsonConvert.DeserializeObject<List<OrdenMadre>>(datos2.ToString());

                        foreach(var OrdMad in OrdenesMadre)
                        {
                            foreach (var Lin in OrdMad.LineasOrdenMadre)
                            {
                                Lin.Id = Lin.Secuencial;
                            }
                        }
                    }
                }
                else
                {
                    if (response != null && response.Data.Estado != null && response.Data.Mensajes != null)
                    {
                        string mensaje = string.Empty;

                        if (response.Data.Mensajes != null)
                        {
                            foreach (var mens in response.Data.Mensajes)
                            {
                                mensaje += mens + " ";
                            }
                        }

                        EventLogUtil.EscribirLogErrorWeb(json, 25, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 25, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/CrearOrdenesCompraMadre");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 25, sesion.OrigenLog);

            return OrdenesMadre;
        }
    }
}
