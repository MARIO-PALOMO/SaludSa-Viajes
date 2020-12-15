using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para roles.
    /// </summary>
    public class RolDAL
    {
        /// <summary>
        /// Proceso para obtener los roles.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolViewModel></returns>
        public static List<RolViewModel> ObtenerRoles(ApplicationDbContext db)
        {
            List<RolViewModel> resultado = null;

            resultado = db.Roles.Select(x => new RolViewModel() {
                Id = x.Id,
                Nombre = x.Nombre,
                Tipo = x.Tipo
            }).OrderBy(x => new { x.Tipo, x.Id }).ToList();

            return resultado;
        }
    }
}
