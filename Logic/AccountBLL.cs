using Common.Utilities;
using Common.ViewModels;
using Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de cuentas de usuario.
    /// </summary>
    public class AccountBLL
    {
        /// <summary>
        /// Proceso para obtener el token de autenticación.
        /// </summary>
        /// <param name="Url">Url de los servicios.</param>
        /// <param name="Usuario">Nombre de usuario para consumir los servicios de autenticación.</param>
        /// <param name="Clave">Contraseña de usuario para consumir los servicios de autenticación.</param>
        /// <param name="ClientId">Identificador de cliente.</param>
        /// <param name="OrigenLog">Origen para guardar logs en el visor de sucesos de windows.</param>
        /// <returns>TokenViewModel</returns>
        public static TokenViewModel ObtenerToken(
            string Url, 
            string Usuario, 
            string Clave, 
            string ClientId, 
            string OrigenLog)
        {
            return AutenticationRCL.ObtenerToken(Url, Usuario, Clave, ClientId, OrigenLog);
        }

        /// <summary>
        /// Proceso para validar que las credenciales del usuario sean correctas.
        /// </summary>
        /// <param name="Username">Nombre de usuario.</param>
        /// <param name="Password">Contraseña de usuario.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>RespuestaRestViewModel</returns>
        public static RespuestaRestViewModel ValidarCredenciales(
            string Username, 
            string Password, 
            ContenedorVariablesSesion sesion)
        {
            return AutenticationRCL.ValidarCredenciales(Username, Password, sesion);
        }

        /// <summary>
        /// Proceso para obtener los datos de un usuario.
        /// </summary>
        /// <param name="Username">Nombre de usuario.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>UsuarioViewModel</returns>
        public static UsuarioViewModel ObtenerUsuario(
            string Username, 
            ContenedorVariablesSesion sesion)
        {
            return UsuarioRCL.ObtenerUsuario(Username, sesion);
        }
    }
}
