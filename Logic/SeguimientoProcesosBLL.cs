using Common.ViewModels;
using Data.Context;
using Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de los reportes de gestión de procesos del subsistema de compras.
    /// </summary>
    public class SeguimientoProcesosBLL
    {
        /// <summary>
        /// Proceso para obtener los originadores de una compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<UsuarioViewModel></returns>
        public static List<UsuarioViewModel> ObtenerOriginadores(
            string EmpresaCodigo, 
            ApplicationDbContext db)
        {
            return SolicitudCompraCabeceraDAL.ObtenerOriginadores(EmpresaCodigo, db);
        }

        /// <summary>
        /// Proceso para obtener las solicitudes filtradas.
        /// </summary>
        /// <param name="EmpresaCodigo">Filtro de compañía.</param>
        /// <param name="Originadores">Filtro de originadores.</param>
        /// <param name="FechaDesde">Filtro fecha desde.</param>
        /// <param name="FechaHasta">Filtro fecha hasta.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudCompraCabeceraViewModel> ObtenerSolicitudes(
            string EmpresaCodigo, 
            List<string> Originadores, 
            DateTime FechaDesde, 
            DateTime FechaHasta, 
            ApplicationDbContext db)
        {
            return SolicitudCompraCabeceraDAL.ObtenerSolicitudesSeguimientoProcesos(EmpresaCodigo, Originadores, FechaDesde, FechaHasta, db);
        }
    }
}
