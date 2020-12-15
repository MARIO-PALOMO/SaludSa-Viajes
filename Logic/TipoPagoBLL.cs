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
    /// Gestiona la lógica de negocio de tipos de pago.
    /// </summary>
    public class TipoPagoBLL
    {
        /// <summary>
        /// Proceso para obtener las cuentas contables de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<CuentaContableViewModel></returns>
        public static List<CuentaContableViewModel> ObtenerCuentasContable(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return CuentaContableRCL.ObtenerCuentasContable(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para buscar los tipos de pago.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los tipos de pago inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TipoPagoViewModel></returns>
        public static List<TipoPagoViewModel> BuscarItems(
            bool MostrarInactivos, 
            ApplicationDbContext db)
        {
            return TipoPagoDAL.BuscarItems(MostrarInactivos, db);
        }

        /// <summary>
        /// Proceso para buscar un tipo de pago.
        /// </summary>
        /// <param name="Id">Identificador del tipo de pago.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>TipoPagoViewModel</returns>
        public static TipoPagoViewModel BuscarItem(
            long Id, 
            ApplicationDbContext db)
        {
            return TipoPagoDAL.BuscarItem(Id, db);
        }

        /// <summary>
        /// Proceso para buscar los tipos de pago por compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <param name="EsReembolso">Bandera que indica si filtrar por reembolsos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TipoPagoViewModel></returns>
        public static List<TipoPagoViewModel> BuscarItemsPorEmpresa(
            string EmpresaCodigo,
            bool? EsReembolso, 
            ApplicationDbContext db)
        {
            return TipoPagoDAL.BuscarItemsPorEmpresa(EmpresaCodigo, EsReembolso, db);
        }
    }
}
