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
    /// Cliente rest para líneas de órdenes hijas.
    /// </summary>
    public class OrdenHijaLineaRCL
    {
        /// <summary>
        /// Proceso para crear una línea de orden hija.
        /// </summary>
        /// <param name="OrdenMadre">Objeto que contiene los datos de la orden madre.</param>
        /// <param name="OrdenHijaLinea">Objeto que contiene los datos de la línea de orden hija.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void CrearLineaOrdenHija(
            OrdenMadre OrdenMadre, 
            OrdenHijaLinea OrdenHijaLinea, 
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/CrearLineaOrdenCompraHija", Method.POST);

            var cantidad = OrdenHijaLinea.Cantidad.ToString().Replace(',', '.');
            var precioUnitario = OrdenHijaLinea.PrecioUnitario.ToString().Replace(',', '.');
            var precioTotal = OrdenHijaLinea.PrecioTotal.ToString().Replace(',', '.');

            reques.AddQueryParameter("numeroOrdenMadre", OrdenMadre.NumeroOrdenMadre);
            reques.AddQueryParameter("codigoLineaOrdenMadre", OrdenHijaLinea.OrdenMadreLinea.CodigoLinea);
            reques.AddQueryParameter("cantidad", cantidad);
            reques.AddQueryParameter("departamento", OrdenHijaLinea.Departamento);
            reques.AddQueryParameter("centroCosto", OrdenHijaLinea.CentroCosto);
            reques.AddQueryParameter("proposito", OrdenHijaLinea.Proposito);
            reques.AddQueryParameter("precioUnitario", precioUnitario);
            reques.AddQueryParameter("precioTotal", precioTotal);
            reques.AddQueryParameter("codigoLineaInterno", OrdenHijaLinea.Id.ToString());
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
                origin = "ServicioAdministracion/api/DynamicsAx/CrearLineaOrdenCompraHija",
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

                EventLogUtil.EscribirLogErrorWeb(json, 18, sesion.OrigenLog);

                throw new Exception(string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/CrearLineaOrdenCompraHija\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription));
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

                        EventLogUtil.EscribirLogErrorWeb(json, 18, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        string resultado = Content.Datos.ToString();

                        if(resultado != "El registro se ha creado")
                        {
                            EventLogUtil.EscribirLogErrorWeb(json, 18, sesion.OrigenLog);

                            throw new Exception("Error en la creación de la línea de orden hija.");
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

                        EventLogUtil.EscribirLogErrorWeb(json, 18, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        EventLogUtil.EscribirLogErrorWeb(json, 18, sesion.OrigenLog);

                        throw new Exception("Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/CrearLineaOrdenCompraHija");
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 18, sesion.OrigenLog);
        }
    }
}
