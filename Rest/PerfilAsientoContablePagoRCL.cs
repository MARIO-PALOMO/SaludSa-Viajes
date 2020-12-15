using Common.Utilities;
using Common.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest
{
    /// <summary>
    /// Cliente rest para perfiles de asiento contable.
    /// </summary>
    public class PerfilAsientoContablePagoRCL
    {
        /// <summary>
        /// Proceso para obtener perfiles de asiento contable de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>List<PerfilAsientoContablePagoViewModel></returns>
        public static List<PerfilAsientoContablePagoViewModel> ObtenerPerfilesAsientosContablesPago(
            ContenedorVariablesSesion sesion, 
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerPerfilAsientosContables", Method.GET);

            reques.AddParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            List<PerfilAsientoContablePagoViewModel> items = new List<PerfilAsientoContablePagoViewModel>();

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerPerfilAsientosContables",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerPerfilAsientosContables\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 37, sesion.OrigenLog);

                throw new Exception(error);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 37, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        items = JsonConvert.DeserializeObject<List<PerfilAsientoContablePagoViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 37, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerPerfilAsientosContables";
                        EventLogUtil.EscribirLogErrorWeb(json, 37, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 37, sesion.OrigenLog);

            return items;
        }
    }
}
