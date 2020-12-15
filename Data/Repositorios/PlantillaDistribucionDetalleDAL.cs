using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para detalles de plantillas de distribución.
    /// </summary>
    public class PlantillaDistribucionDetalleDAL
    {
        /// <summary>
        /// Proceso para obtener los detalles de una plantilla de distribución.
        /// </summary>
        /// <param name="Id">Identificador de la plantilla de distribución.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<PlantillasDistribucionDetalleViewModel></returns>
        public static List<PlantillasDistribucionDetalleViewModel> ObtenerDetallesPlantilla(
            long Id,
            ApplicationDbContext db)
        {
            List<PlantillasDistribucionDetalleViewModel> resultado = null;
            
            resultado = db.PlantillasDistribucionDetalle.Where(y => y.DistribucionCabeceraId == Id && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new PlantillasDistribucionDetalleViewModel()
            {
                Porcentaje = y.Porcentaje,
                EstadoId = y.EstadoId,
                Departamento = new DepartamentoViewModel()
                {
                    Codigo = y.DepartamentoCodigo,
                    Descripcion = y.DepartamentoDescripcion,
                    CodigoDescripcion = y.DepartamentoCodigoDescripcion
                },
                CentroCosto = new CentroCostoViewModel()
                {
                    Codigo = y.CentroCostoCodigo,
                    Descripcion = y.CentroCostoDescripcion,
                    CodigoDescripcion = y.CentroCostoCodigoDescripcion
                },
                Proposito = new PropositoViewModel
                {
                    Codigo = y.PropositoCodigo,
                    Descripcion = y.PropositoDescripcion,
                    CodigoDescripcion = y.PropositoCodigoDescripcion
                }
            }).ToList();

            return resultado;
        }
    }
}
