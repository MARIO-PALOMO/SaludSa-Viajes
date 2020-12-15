using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para ciudades.
    /// </summary>
    public class CiudadDAL
    {
        /// <summary>
        /// Proceso para obtener las ciudades.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<CiudadViewModel></returns>
        public static List<CiudadViewModel> ObtenerCiudades(ApplicationDbContext db)
        {
            List<CiudadViewModel> resultado = null;

            resultado = db.Ciudades.Select(x => new CiudadViewModel() {
                Id = x.Id,
                Nombre = x.Nombre,
                Codigo = x.Codigo,
                Direccion = x.Direccion,
                Provincia = x.Provincia
            }).ToList();

            return resultado;
        }
    }
}
