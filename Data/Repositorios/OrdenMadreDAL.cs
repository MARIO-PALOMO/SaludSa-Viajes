using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para ordenes madre.
    /// </summary>
    public class OrdenMadreDAL
    {
        /// <summary>
        /// Proceso para salvar ordenes madre.
        /// </summary>
        /// <param name="OrdenesMadre">Listado de ordenes madre a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarOrdenesMadre(
            List<OrdenMadre> OrdenesMadre,
            ApplicationDbContext db)
        {
            foreach(var OrdenMadre in OrdenesMadre)
            {
                db.OrdenesMadre.Add(OrdenMadre);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Proceso para validar si una orden madre tiene saldo.
        /// </summary>
        /// <param name="OrdenMadreId">Identificador de la orden madre.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>bool</returns>
        public static bool ValidarSaldo(
            long OrdenMadreId, 
            ApplicationDbContext db)
        {
            decimal CantidadOrdenMadre = 0;
            decimal CantidadRecepcion = 0;

            CantidadOrdenMadre = db.OrdenMadreLineas.Where(x => x.OrdenMadreId == OrdenMadreId).Sum(x => x.Cantidad);
            CantidadRecepcion = db.RecepcionLineas.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.Recepcion.OrdenMadreId == OrdenMadreId).Sum(x => x.Cantidad);

            return CantidadOrdenMadre > CantidadRecepcion;
        }
    }
}
