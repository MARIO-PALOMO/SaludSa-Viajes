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
    /// Gestiona la lógica de negocio de roles de gestores de compra del subsistema de compras.
    /// </summary>
    public class RolGestorCompraBLL
    {
        /// <summary>
        /// Proceso para obtener obtener las compañías.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<EmpresaViewModel></returns>
        public static List<EmpresaViewModel> ObtenerEmpresas(ContenedorVariablesSesion sesion)
        {
            return EmpresaRCL.ObtenerEmpresas(sesion);
        }

        /// <summary>
        /// Proceso para obtener los usuarios que pertenecen a determinados grupos.
        /// </summary>
        /// <param name="GrupoGestoresCompra">Listado de grupos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<UsuarioViewModel></returns>
        public static List<UsuarioViewModel> ObtenerUsuariosPorGrupo(
            string[] GrupoGestoresCompra, 
            ContenedorVariablesSesion sesion)
        {
            return UsuarioRCL.ObtenerUsuariosPorGrupo(GrupoGestoresCompra, sesion);
        }

        /// <summary>
        /// Proceso para obtener los roles de gestores de compra.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los roles inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolGestorCompraViewModel></returns>
        public static List<RolGestorCompraViewModel> ObtenerRolesGestorCompra(
            bool MostrarInactivos, 
            ApplicationDbContext db)
        {
            return RolGestorCompraDAL.ObtenerRolesGestorCompra(MostrarInactivos, db);
        }

        /// <summary>
        /// Proceso para obtener los tipos de compra de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<TipoCompraViewModel></returns>
        public static List<TipoCompraViewModel> ObtenerTiposCompra(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return TipoCompraRCL.ObtenerTiposCompra(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener un rol de gestor de compra.
        /// </summary>
        /// <param name="Id">Identificador del rol de gestor de compra.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>RolGestorCompraViewModel</returns>
        public static RolGestorCompraViewModel ObtenerRolGestorCompra(
            long Id, 
            ApplicationDbContext db)
        {
            return RolGestorCompraDAL.ObtenerRolGestorCompra(Id, db);
        }

        /// <summary>
        /// Proceso para obtener un usuario gestor.
        /// </summary>
        /// <param name="username">Nombre de usuario del gestor.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>UsuarioViewModel</returns>
        public static UsuarioViewModel ObtenerGestor(
            string username, 
            ContenedorVariablesSesion sesion)
        {
            return UsuarioRCL.ObtenerUsuario(username, sesion);
        }
    }
}
