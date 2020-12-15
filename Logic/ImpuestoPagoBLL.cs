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
    /// Gestiona la lógica de negocio de los impuestos del subsistema de pagos.
    /// </summary>
    public class ImpuestoPagoBLL
    {
        /// <summary>
        /// Proceso para buscar los impuestos.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los impuestos inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<ImpuestoPagoViewModel></returns>
        public static List<ImpuestoPagoViewModel> BuscarItems(
            bool MostrarInactivos, 
            ApplicationDbContext db)
        {
            return ImpuestoPagoDAL.BuscarItems(MostrarInactivos, db);
        }

        /// <summary>
        /// Proceso para buscar un impuesto.
        /// </summary>
        /// <param name="Id">Identificador del impuesto.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>ImpuestoPagoViewModel</returns>
        public static ImpuestoPagoViewModel BuscarItem(
            long Id, 
            ApplicationDbContext db)
        {
            return ImpuestoPagoDAL.BuscarItem(Id, db);
        }
    }
}
