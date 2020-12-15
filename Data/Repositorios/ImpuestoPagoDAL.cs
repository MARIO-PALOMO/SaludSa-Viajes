using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para impuestos de pagos.
    /// </summary>
    public class ImpuestoPagoDAL
    {
        /// <summary>
        /// Proceso para buscar los impuestos de pago.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los impuestos inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<ImpuestoPagoViewModel></returns>
        public static List<ImpuestoPagoViewModel> BuscarItems(
            bool MostrarInactivos, 
            ApplicationDbContext db)
        {
            List<ImpuestoPagoViewModel> resultado = null;

            if (MostrarInactivos)
            {
                resultado = db.ImpuestosPago.Select(x => new ImpuestoPagoViewModel()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    Porcentaje = x.Porcentaje,
                    Compensacion = x.Compensacion,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion
                }).ToList();
            }
            else
            {
                resultado = db.ImpuestosPago.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO).Select(x => new ImpuestoPagoViewModel()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    Porcentaje = x.Porcentaje,
                    Compensacion = x.Compensacion,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion
                }).ToList();
            }

            return resultado;
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
            var resultado = db.ImpuestosPago.Where(x => x.Id == Id).Select(x => new ImpuestoPagoViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Porcentaje = x.Porcentaje,
                Compensacion = x.Compensacion,
                Estado = new EstadoViewModel()
                {
                    Id = x.EstadoId,
                    Descripcion = x.Estado.Descripcion
                }
            }).FirstOrDefault();

            return resultado;
        }
    }
}
