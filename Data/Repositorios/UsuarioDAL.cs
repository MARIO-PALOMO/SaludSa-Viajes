using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para configuración de usuarios.
    /// </summary>
    public class UsuarioDAL
    {
        /// <summary>
        /// Proceso para obtener el nombre de nuevo usuario para reasignación.
        /// </summary>
        /// <param name="UsuarioResponsable">Nombre de usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerUsuarioParaReasignacion(
            string UsuarioResponsable, 
            ApplicationDbContext db)
        {
            string result = string.Empty;

            var obj = db.Usuarios.Where(x => x.NombreUsuario == UsuarioResponsable && x.EnLaOficina == false).FirstOrDefault();

            if(obj != null)
            {
                result = obj.UsuarioReasignar;
            }

            return result;
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
            if (sesion != null)
            {
                return db.Usuarios.Where(x => x.NombreUsuario == sesion.usuario.Usuario).Count() > 0;
            }

            return false;
        }
    }
}
