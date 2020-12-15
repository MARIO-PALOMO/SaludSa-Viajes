﻿using Common.Utilities;
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
    /// Cliente rest para impuesto renta.
    /// </summary>
    public class ImpuestoRentaGrupoImpuestoArticuloPagoRCL
    {
        /// <summary>
        /// Proceso para obtener los impuesto renta de una compañía y de un grupo de impuesto de artículo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="codigoGrupoImpuestoArticulo">Identificador del grupo de impuesto de artículo.</param>
        /// <returns>List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel></returns>
        public static List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel> ObtenerImpuestoRentaGrupoImpuestoArticuloPago(
            ContenedorVariablesSesion sesion, 
            string codigoCompania, 
            string codigoGrupoImpuestoArticulo)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/ObtenerImpuestoRentaGrupoImpuestoArticulo", Method.GET);

            reques.AddParameter("codigoCompania", codigoCompania);
            reques.AddParameter("codigoGrupoImpuestoArticulo", codigoGrupoImpuestoArticulo);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel> items = new List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel>();

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/ObtenerImpuestoRentaGrupoImpuestoArticulo",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/ObtenerImpuestoRentaGrupoImpuestoArticulo\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 41, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 41, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        items = JsonConvert.DeserializeObject<List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel>>(Content.Datos.ToString());
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

                        EventLogUtil.EscribirLogErrorWeb(json, 41, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/ObtenerImpuestoRentaGrupoImpuestoArticulo";
                        EventLogUtil.EscribirLogErrorWeb(json, 41, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 41, sesion.OrigenLog);

            return items;
        }
    }
}
