using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Repositorios;
using Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de fuera de oficina.
    /// </summary>
    public class FueraOficinaBLL
    {
        /// <summary>
        /// Proceso para obtener el usuario encargado de sustituir a otro.
        /// </summary>
        /// <param name="Usuario">Usuario sustituido.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>UsuarioViewModel</returns>
        public static UsuarioViewModel ObtenerUsuarioReasignacion(
            string Usuario, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            UsuarioViewModel UsuarioObj = null;

            var UsuarioReasignar = UsuarioDAL.ObtenerUsuarioParaReasignacion(Usuario, db);

            if(UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);
            }

            return UsuarioObj;
        }

        /// <summary>
        /// Proceso para saber si el usuario autenticado está fuera de oficina.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>bool</returns>
        public static bool EstaFueraOficina(
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            return UsuarioDAL.EstaFueraOficina(sesion, db);
        }
    }
}
