using Common.Utilities;
using Common.ViewModels;
using Data.Entidades;
using Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de anulación de recepciones.
    /// </summary>
    public class AnulacionRecepcionBLL
    {
        /// <summary>
        /// Proceso para buscar recepciones.
        /// </summary>
        /// <param name="NumeroSolicitud">Filtro de número de solicitud.</param>
        /// <param name="Cabecera">Objeto que contiene los datos de la cabecera de la solicitud de compra.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<RecepcionViewModel></returns>
        public static List<RecepcionViewModel> BuscarRecepciones(
            long NumeroSolicitud, 
            SolicitudCompraCabecera Cabecera, 
            ContenedorVariablesSesion sesion)
        {
            return RecepcionDAL.BuscarRecepciones(NumeroSolicitud, Cabecera, sesion);
        }
    }
}
