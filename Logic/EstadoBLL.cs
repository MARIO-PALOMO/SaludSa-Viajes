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
    /// Gestiona la lógica de negocio de los estados.
    /// </summary>
    public class EstadoBLL
    {
        /// <summary>
        /// Proceso para obtener los estados válidos en el sistema.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<EstadoViewModel></returns>
        public static List<EstadoViewModel> ObtenerEstados(ApplicationDbContext db)
        {
            return EstadoDAL.ObtenerEstados(db);
        }
    }
}
