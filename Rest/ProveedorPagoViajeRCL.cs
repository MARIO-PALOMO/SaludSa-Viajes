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
    /// Cliente rest para proveedores de pagos y viajes.
    /// </summary>
    public class ProveedorPagoViajeRCL
    {
        /// <summary>
        /// Proceso para obtener proveedor por criterio de búsqueda.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <param name="identificacion">Identificación del proveedor.</param>
        /// <param name="nombresApellidos">Nombre y apellidos del proveedor.</param>
        /// <returns>List<ProveedorPagoViajeViewModel></returns>
        public static List<ProveedorPagoViajeViewModel> ObtenerProveedoresPagoViaje(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo, 
            string identificacion, 
            string nombresApellidos)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerProveedorPagosViajes", Method.GET);

            reques.AddParameter("codigoCompania", CompaniaCodigo);

            if(identificacion != null && identificacion != string.Empty)
            {
                reques.AddParameter("identificacion", identificacion);
            }

            if (nombresApellidos != null && nombresApellidos != string.Empty)
            {
                reques.AddParameter("nombresApellidos", nombresApellidos);
            }

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
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerProveedorPagosViajes",
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

            List<ProveedorPagoViajeViewModel> items = new List<ProveedorPagoViajeViewModel>();

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

                EventLogUtil.EscribirLogErrorWeb(json, 36, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerProveedorPagosViajes\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 36, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        items = JsonConvert.DeserializeObject<List<ProveedorPagoViajeViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 36, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 36, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerProveedorPagosViajes");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 36, sesion.OrigenLog);

            return items;
        }
    }
}
