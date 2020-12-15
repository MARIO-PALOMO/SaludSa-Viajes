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
    /// Cliente rest para contabilización de pagos.
    /// </summary>
    public class ContabilidadPagoRCL
    {
        /// <summary>
        /// Proceso para crear un diario.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="nombreDiario">Nombre del diario.</param>
        /// <param name="descripcionDiario">Descripción del diario.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string CrearDiario(
            ContenedorVariablesSesion sesion, 
            string nombreDiario, 
            string descripcionDiario, 
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/CrearDiario", Method.POST);

            reques.AddQueryParameter("nombreDiario", nombreDiario);
            reques.AddQueryParameter("descripcionDiario", descripcionDiario);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/CrearDiario",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/CrearDiario\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 45, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 45, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if(respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 45, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/CrearDiario";
                        EventLogUtil.EscribirLogErrorWeb(json, 45, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 45, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para crear una línea de proveedor de reembolso.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="numeroDiario">Número de diario.</param>
        /// <param name="valor">Valor.</param>
        /// <param name="fechaTransaccion">Fecha de transacción.</param>
        /// <param name="proveedor">Proveedor.</param>
        /// <param name="descripcion">Descripción de la línea.</param>
        /// <param name="referencia">Referencia.</param>
        /// <param name="departameto">Departamento.</param>
        /// <param name="perfilAsiento">Perfil de asiento.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string LineaProveedorReembolso(
            ContenedorVariablesSesion sesion, 
            string numeroDiario, 
            decimal valor, 
            DateTime fechaTransaccion,
            string proveedor,
            string descripcion,
            string referencia,
            string departameto,
            string perfilAsiento,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/LineaProveedorReembolso", Method.POST);

            reques.AddQueryParameter("numeroDiario", numeroDiario);
            reques.AddQueryParameter("valor", valor.ToString().Replace(',', '.'));
            reques.AddQueryParameter("fechaTransaccion", fechaTransaccion.ToString("yyyy/MM/dd"));
            reques.AddQueryParameter("proveedor", proveedor);
            reques.AddQueryParameter("descripcion", descripcion);
            reques.AddQueryParameter("referencia", referencia);
            reques.AddQueryParameter("departameto", departameto);
            reques.AddQueryParameter("perfilAsiento", perfilAsiento);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/LineaProveedorReembolso",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/LineaProveedorReembolso\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 46, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 46, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 46, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/LineaProveedorReembolso";
                        EventLogUtil.EscribirLogErrorWeb(json, 46, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 46, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para crear una línea de proveedor de factura.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="numeroDiario">Número de diario.</param>
        /// <param name="valor">Valor.</param>
        /// <param name="fechaFactura">Fecha de la factura.</param>
        /// <param name="proveedor">Proveedor.</param>
        /// <param name="descripcion">Descripción de la línea.</param>
        /// <param name="numeroFactura">Número de factura.</param>
        /// <param name="perfilAsiento">Perfil de asiento.</param>
        /// <param name="referencia">Referencia.</param>
        /// <param name="departameto">Departamento.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string LineaProveedorFactura(
            ContenedorVariablesSesion sesion,
            string numeroDiario,
            decimal valor,
            DateTime fechaFactura,
            string proveedor,
            string descripcion,
            string numeroFactura,
            string perfilAsiento,
            string referencia,
            string departameto,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/LineaProveedorFactura", Method.POST);

            reques.AddQueryParameter("numeroDiario", numeroDiario);
            reques.AddQueryParameter("valor", valor.ToString().Replace(',', '.'));
            reques.AddQueryParameter("fechaFactura", fechaFactura.ToString("yyyy/MM/dd"));
            reques.AddQueryParameter("proveedor", proveedor);
            reques.AddQueryParameter("descripcion", descripcion);
            reques.AddQueryParameter("numeroFactura", numeroFactura);
            reques.AddQueryParameter("perfilAsiento", perfilAsiento);
            reques.AddQueryParameter("referencia", referencia);
            reques.AddQueryParameter("departameto", departameto);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/LineaProveedorFactura",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/LineaProveedorFactura\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 47, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 47, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 47, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/LineaProveedorFactura";
                        EventLogUtil.EscribirLogErrorWeb(json, 47, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 47, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para crear una línea de reembolso.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="numeroDiario">Número de diario.</param>
        /// <param name="valor">Valor.</param>
        /// <param name="cuentaContable">Cuenta contable seleccionada.</param>
        /// <param name="descripcion">Descripción de la línea.</param>
        /// <param name="departamento">Departamento.</param>
        /// <param name="centroCosto">Centro de costo.</param>
        /// <param name="proposito">Propósito.</param>
        /// <param name="codigoProyecto">Código de proyecto.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string LineaReembolso(
            ContenedorVariablesSesion sesion,
            string numeroDiario,
            decimal valor,
            string cuentaContable,
            string descripcion,
            string departamento,
            string centroCosto,
            string proposito,
            string codigoProyecto,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/LineaReembolso", Method.POST);

            reques.AddQueryParameter("numeroDiario", numeroDiario);
            reques.AddQueryParameter("valor", valor.ToString().Replace(',', '.'));
            reques.AddQueryParameter("cuentaContable", cuentaContable);
            reques.AddQueryParameter("descripcion", descripcion);
            reques.AddQueryParameter("departamento", departamento);
            reques.AddQueryParameter("centroCosto", centroCosto);
            reques.AddQueryParameter("proposito", proposito);
            reques.AddQueryParameter("codigoProyecto", codigoProyecto);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/LineaReembolso",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/LineaReembolso\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 48, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 48, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 48, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/LineaReembolso";
                        EventLogUtil.EscribirLogErrorWeb(json, 48, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 48, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para crear una línea de factura.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="parametros">Cadena construida con los parámetros requeridos.</param>
        /// <param name="valor">Valor.</param>
        /// <param name="descripcion">Descripción de la línea.</param>
        /// <param name="credito">Crédito.</param>
        /// <returns>string</returns>
        public static string LineaFactura(
            ContenedorVariablesSesion sesion,
            string parametros,
            decimal valor,
            string descripcion,
            bool credito)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/LineaFactura", Method.POST);

            reques.AddQueryParameter("parametros", parametros);
            reques.AddQueryParameter("valor", valor.ToString().Replace(',', '.'));
            reques.AddQueryParameter("descripcion", descripcion);
            reques.AddQueryParameter("credito", credito.ToString());

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/LineaFactura",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/LineaFactura\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 49, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 49, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 49, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/LineaFactura";
                        EventLogUtil.EscribirLogErrorWeb(json, 49, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 49, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para registrar el diario.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="numeroDiario">Número de diario.</param>
        /// <param name="autorizacion">Número de autorización para comrpobantes físicos.</param>
        /// <param name="fechaVigencia">Fecha de vigencia.</param>
        /// <param name="autorizacionElectronica">Número de autorización para comrpobantes electrónicos.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string RegistraDiario(
            ContenedorVariablesSesion sesion,
            string numeroDiario,
            string autorizacion,
            DateTime fechaVigencia,
            string autorizacionElectronica,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/RegistraDiario", Method.POST);

            reques.AddQueryParameter("numeroDiario", numeroDiario);
            reques.AddQueryParameter("autorizacion", autorizacion);
            reques.AddQueryParameter("fechaVigencia", fechaVigencia.ToString("yyyy/MM/dd"));
            reques.AddQueryParameter("autorizacionElectronica", autorizacionElectronica);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/RegistraDiario",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/RegistraDiario\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 50, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 50, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta.Length > 15)
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 50, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/RegistraDiario";
                        EventLogUtil.EscribirLogErrorWeb(json, 50, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 50, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para borrar una línea de diario.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="idRegistro">Identificador de la línea.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string BorrarLineaDiario(
            ContenedorVariablesSesion sesion,
            string idRegistro,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/BorrarLineaDiario", Method.POST);

            reques.AddQueryParameter("idRegistro", idRegistro);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/BorrarLineaDiario",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/BorrarLineaDiario\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 51, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 51, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta != "Eliminación de diario contable")
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 51, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/BorrarLineaDiario";
                        EventLogUtil.EscribirLogErrorWeb(json, 51, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 51, sesion.OrigenLog);

            return respuesta;
        }

        /// <summary>
        /// Proceso para borrar un diario.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="numeroDiario">Número de diario.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <returns>string</returns>
        public static string BorrarDiario(
            ContenedorVariablesSesion sesion,
            string numeroDiario,
            string codigoCompania)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioAdministracion/api/DynamicsAx/BorrarDiario", Method.POST);

            reques.AddQueryParameter("numeroDiario", numeroDiario);
            reques.AddQueryParameter("codigoCompania", codigoCompania);

            reques.AddHeader("Authorization", "Bearer " + sesion.token.access_token);
            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            IRestResponse response = cliente.Execute(reques);

            string respuesta = string.Empty;

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAdministracion/api/DynamicsAx/BorrarDiario",
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

                var error = string.Format("Error al intentar consumir el servicio web \"ServicioAdministracion/api/DynamicsAx/BorrarDiario\". Código de error: {0} - Descripción: {1}. " + mensaje, response.StatusCode, response.StatusDescription);

                EventLogUtil.EscribirLogErrorWeb(json, 52, sesion.OrigenLog);

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

                        EventLogUtil.EscribirLogErrorWeb(json, 52, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        respuesta = Content.Datos.ToString();

                        if (respuesta != "Eliminación de diario contable")
                        {
                            throw new Exception(respuesta);
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

                        EventLogUtil.EscribirLogErrorWeb(json, 52, sesion.OrigenLog);

                        throw new Exception(mensaje);
                    }
                    else
                    {
                        var error = "Error en la respuesta obtenida al consultar el servicio: /ServicioAdministracion/api/DynamicsAx/BorrarDiario";
                        EventLogUtil.EscribirLogErrorWeb(json, 52, sesion.OrigenLog);
                        throw new Exception(error);
                    }
                }
            }

            EventLogUtil.EscribirLogInfoWeb(json, 52, sesion.OrigenLog);

            return respuesta;
        }
    }
}
