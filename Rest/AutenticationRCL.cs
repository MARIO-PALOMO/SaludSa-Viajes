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
    /// Cliente rest para autenticación.
    /// </summary>
    public class AutenticationRCL
    {
        /// <summary>
        /// Proceso para obtener el token de autenticación.
        /// </summary>
        /// <param name="url">Url de los servicios.</param>
        /// <param name="username">Nombre de usuario para consumir los servicios de autenticación.</param>
        /// <param name="password">Contraseña de usuario para consumir los servicios de autenticación.</param>
        /// <param name="client_id">Identificador de cliente.</param>
        /// <param name="OrigenLog">Origen para guardar logs en el visor de sucesos de windows.</param>
        /// <returns>TokenViewModel</returns>
        public static TokenViewModel ObtenerToken(
            string url, 
            string username, 
            string password, 
            string client_id, 
            string OrigenLog)
        {
            RestClient cliente = new RestClient(url);

            var reques = new RestRequest("ServicioAutorizacion/oauth2/token", Method.POST);

            reques.AddParameter("username", username);
            reques.AddParameter("password", password);
            reques.AddParameter("grant_type", "password");
            reques.AddParameter("client_id", client_id);

            reques.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            reques.Timeout = 600000;

            var response = cliente.Execute<TokenViewModel>(reques);

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAutorizacion/oauth2/token",
                user = "N/A",
                request = reques.Parameters,
                response = new
                {
                    response.StatusCode,
                    response.StatusDescription,
                    response.ResponseStatus,
                    response.Content
                }
            });

            if (response.Data == null || response.Data.error != null)
            {
                EventLogUtil.EscribirLogErrorWeb(json, 1, OrigenLog);
            }
            else
            {
                EventLogUtil.EscribirLogInfoWeb(json, 1, OrigenLog);
            }

            return response.Data;
        }

        /// <summary>
        /// Proceso para refrescar el token de autenticación.
        /// </summary>
        /// <param name="url">Url de los servicios.</param>
        /// <param name="client_id">Identificador de cliente.</param>
        /// <param name="refresh_token">Token para refrescamiento.</param>
        /// <param name="OrigenLog">Origen para guardar logs en el visor de sucesos de windows.</param>
        /// <returns>TokenViewModel</returns>
        public static TokenViewModel RefrescarToken(
            string url, 
            string client_id, 
            string refresh_token, 
            string OrigenLog)
        {
            RestClient cliente = new RestClient(url);

            var reques = new RestRequest("ServicioAutorizacion/oauth2/token", Method.POST);

            reques.AddParameter("grant_type", "refresh_token");
            reques.AddParameter("client_id", client_id);
            reques.AddParameter("refresh_token", refresh_token);

            reques.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            reques.Timeout = 600000;

            var response = cliente.Execute<TokenViewModel>(reques);

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioAutorizacion/oauth2/token",
                user = "N/A",
                request = reques.Parameters,
                response = new
                {
                    response.StatusCode,
                    response.StatusDescription,
                    response.ResponseStatus,
                    response.Content
                }
            });

            if (response.Data == null || response.Data.error != null)
            {
                EventLogUtil.EscribirLogErrorWeb(json, 2, OrigenLog);
            }
            else
            {
                EventLogUtil.EscribirLogInfoWeb(json, 2, OrigenLog);
            }

            return response.Data;
        }

        /// <summary>
        /// Proceso para validar que las credenciales del usuario sean correctas.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña de usuario.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>RespuestaRestViewModel</returns>
        public static RespuestaRestViewModel ValidarCredenciales(
            string username, 
            string password, 
            ContenedorVariablesSesion sesion)
        {
            RestClient cliente = new RestClient(sesion.UrlServicios);

            var reques = new RestRequest("ServicioDirectorioActivo/api/ValidarCredenciales", Method.POST);

            reques.AddParameter("usuario", username);
            reques.AddParameter("contrasenia", password);

            reques.AddHeader("CodigoAplicacion", sesion.CodigoAplicacion);
            reques.AddHeader("CodigoPlataforma", "7");
            reques.AddHeader("SistemaOperativo", sesion.SistemaOperativo);
            reques.AddHeader("DispositivoNavegador", sesion.DispositivoNavegador);
            reques.AddHeader("DireccionIP", sesion.DireccionIP);

            reques.Timeout = 600000;

            var response = cliente.Execute<RespuestaRestViewModel>(reques);

            var json = JsonConvert.SerializeObject(new
            {
                origin = "ServicioDirectorioActivo/api/ValidarCredenciales",
                user = "N/A",
                request = reques.Parameters,
                response = new
                {
                    response.StatusCode,
                    response.StatusDescription,
                    response.ResponseStatus,
                    response.Content
                }
            });

            if (response.Data == null)
            {
                EventLogUtil.EscribirLogErrorWeb(json, 3, sesion.OrigenLog);
            }
            else {
                EventLogUtil.EscribirLogInfoWeb(json, 3, sesion.OrigenLog);
            }

            return response.Data;
        }
    }
}
