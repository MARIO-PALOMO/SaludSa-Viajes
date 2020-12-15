using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para roles adminsitrativos.
    /// </summary>
    public class RolAdministrativoDAL
    {
        /// <summary>
        /// Proceso para obtener los roles administrativos.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolAdministrativoViewModel></returns>
        public static List<RolAdministrativoViewModel> ObtenerRolesAdministrativos(ApplicationDbContext db)
        {
            List<RolAdministrativoViewModel> resultado = null;

            resultado = db.RolesAdministrativo.Select(x => new RolAdministrativoViewModel() {
                Id = x.Id,
                CreadoPor = x.CreadoPor,
                FechaCreacion = x.FechaCreacion,
                ColaboradorObj = new UsuarioViewModel() {
                    Usuario = x.ColaboradorUsuario,
                    NombreCompleto = x.ColaboradorNombreCompleto
                },
                EmpresaObj = x.EmpresaCodigo != null ? (new EmpresaViewModel() {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                }) : null,
                RolObj = new RolViewModel() {
                    Id = x.Rol.Id,
                    Nombre = x.Rol.Nombre,
                    Tipo = x.Rol.Tipo
                },
                CiudadObj = x.CiudadId != null ? (new CiudadViewModel() {
                    Id = x.Ciudad.Id,
                    Nombre = x.Ciudad.Nombre,
                    Codigo = x.Ciudad.Codigo,
                    Provincia = x.Ciudad.Provincia,
                    Direccion = x.Ciudad.Direccion
                }) : null
            }).ToList();

            return resultado;
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
            RolAdministrativoViewModel resultado = null;

            resultado = db.RolesAdministrativo.Where(x => x.Id == Id).Select(x => new RolAdministrativoViewModel()
            {
                Id = x.Id,
                CreadoPor = x.CreadoPor,
                FechaCreacion = x.FechaCreacion,
                ColaboradorObj = new UsuarioViewModel()
                {
                    Usuario = x.ColaboradorUsuario,
                    NombreCompleto = x.ColaboradorNombreCompleto
                },
                EmpresaObj = x.EmpresaCodigo != null ? (new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                }) : null,
                RolObj = new RolViewModel()
                {
                    Id = x.Rol.Id,
                    Nombre = x.Rol.Nombre,
                    Tipo = x.Rol.Tipo
                },
                CiudadObj = x.CiudadId != null ? (new CiudadViewModel()
                {
                    Id = x.Ciudad.Id,
                    Nombre = x.Ciudad.Nombre,
                    Codigo = x.Ciudad.Codigo,
                    Provincia = x.Ciudad.Provincia,
                    Direccion = x.Ciudad.Direccion
                }) : null
            }).FirstOrDefault();

            return resultado;
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
            List<long> resultado = null;

            resultado = db.RolesAdministrativo.Where(x => x.ColaboradorUsuario == Usuario).Select(x => x.RolId).ToList();

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Solicitud"></param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerUsuarioVerificacionPresupuesto(
            SolicitudCompraCabecera Solicitud, 
            ApplicationDbContext db)
        {
            string resultado = string.Empty;

            resultado = db.RolesAdministrativo.Where(x =>
                    x.EmpresaCodigo == Solicitud.EmpresaCodigo
                    && x.RolId == (int)EnumRol.PROC_COMPRA_VERIFICAR_PRESUPUESTO
                    && x.Ciudad.Codigo == Solicitud.SolicitanteCiudadCodigo
                ).Select(x => x.ColaboradorUsuario).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener el nombre de usuario con rol de adjuntar factura.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de solicitud de compra.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerUsuarioAdjuntarFactura(
            SolicitudCompraCabecera Solicitud, 
            ApplicationDbContext db)
        {
            string resultado = string.Empty;

            resultado = db.RolesAdministrativo.Where(x =>
                    x.EmpresaCodigo == Solicitud.EmpresaCodigo
                    && x.RolId == (int)EnumRol.PROC_COMPRA_ADJUNTAR_FACTURA
                    && x.Ciudad.Codigo == Solicitud.SolicitanteCiudadCodigo
                ).Select(x => x.ColaboradorUsuario).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener el nombre de usuario con rol de contabilizar recepción.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de solicitud de compra.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerUsuarioContabilizarRecepcion(
            SolicitudCompraCabecera Solicitud,
            ApplicationDbContext db)
        {
            string resultado = string.Empty;

            resultado = db.RolesAdministrativo.Where(x =>
                    x.EmpresaCodigo == Solicitud.EmpresaCodigo
                    && x.RolId == (int)EnumRol.PROC_COMPRA_CONTABILIZAR_RECEPCION
                    && x.Ciudad.Codigo == Solicitud.SolicitanteCiudadCodigo
                ).Select(x => x.ColaboradorUsuario).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener el nombre de usuario con rol de contabilizar pago.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de solicitud de pago.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerUsuarioContabilizarPago(
            SolicitudPagoCabecera Solicitud, 
            ApplicationDbContext db)
        {
            string resultado = string.Empty;

            resultado = db.RolesAdministrativo.Where(x =>
                    x.EmpresaCodigo == Solicitud.EmpresaCodigo
                    && x.RolId == (int)EnumRol.PROC_PAGO_CONTABILIZAR
                    && x.Ciudad.Codigo == Solicitud.SolicitanteCiudadCodigo
                ).Select(x => x.ColaboradorUsuario).FirstOrDefault();

            return resultado;
        }
    }
}
