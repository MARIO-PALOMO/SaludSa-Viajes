using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para estados.
    /// </summary>
    public class EstadoDAL
    {
        /// <summary>
        /// Proceso para obtener los estados.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<EstadoViewModel></returns>
        public static List<EstadoViewModel> ObtenerEstados(ApplicationDbContext db)
        {
            List<EstadoViewModel> resultado = null;

            resultado = db.Estados.Select(x => new EstadoViewModel() {
                Id = x.Id,
                Descripcion = x.Descripcion
            }).ToList();

            return resultado;
        }
    }
}
