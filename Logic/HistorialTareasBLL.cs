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
    /// Gestiona la lógica de negocio de reportes de historial de tareas para los subsistemas de compras y pagos.
    /// </summary>
    public class HistorialTareasBLL
    {
        /// <summary>
        /// Buscar las tareas de una solicitud de compra.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud.</param>
        /// <param name="Empresa">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaViewModel></returns>
        public static List<TareaViewModel> BuscarTareasPorSolicitud(
            long NumeroSolicitud, 
            string Empresa, 
            ApplicationDbContext db)
        {
            return TareaDAL.BuscarTareasPorSolicitud(NumeroSolicitud, Empresa, db);
        }

        /// <summary>
        /// Buscar las tareas de una solicitud de pago.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud.</param>
        /// <param name="Empresa">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaPagoViewModel></returns>
        public static List<TareaPagoViewModel> BuscarTareasPagoPorSolicitud(
            long NumeroSolicitud, 
            string Empresa, 
            ApplicationDbContext db)
        {
            return TareaPagoDAL.BuscarTareasPorSolicitud(NumeroSolicitud, Empresa, db);
        }
    }
}
