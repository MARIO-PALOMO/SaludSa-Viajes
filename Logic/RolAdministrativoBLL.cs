using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de roles administrativos.
    /// </summary>
    public class RolAdministrativoBLL
    {
        /// <summary>
        /// Proceso para obtener los roles administrativos.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolAdministrativoViewModel></returns>
        public static List<RolAdministrativoViewModel> ObtenerRolesAdministrativos(ApplicationDbContext db)
        {
            return RolAdministrativoDAL.ObtenerRolesAdministrativos(db);
        }

        /// <summary>
        /// Proceso para obtener los roles.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolViewModel></returns>
        public static List<RolViewModel> ObtenerRoles(ApplicationDbContext db)
        {
            return RolDAL.ObtenerRoles(db);
        }

        /// <summary>
        /// Proceso para obtener ciudades.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<CiudadViewModel></returns>
        public static List<CiudadViewModel> ObtenerCiudades(ApplicationDbContext db)
        {
            return CiudadDAL.ObtenerCiudades(db);
        }

        /// <summary>
        /// Proceso para obtener un rol administrativo.
        /// </summary>
        /// <param name="Id">Identificador del rol administrativo.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>RolAdministrativoViewModel</returns>
        public static RolAdministrativoViewModel ObtenerRolAdministrativo(
            long Id, 
            ApplicationDbContext db)
        {
            return RolAdministrativoDAL.ObtenerRolAdministrativo(Id, db);
        }

        /// <summary>
        /// Proceso para obtener los roles administrativos de un usuario.
        /// </summary>
        /// <param name="Usuario">Nombre del usuario.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<long></returns>
        public static List<long> ObtenerRolesAdministrativosPorUsuario(
            string Usuario, 
            ApplicationDbContext db)
        {
            return RolAdministrativoDAL.ObtenerRolesAdministrativosPorUsuario(Usuario, db);
        }
    }
}
