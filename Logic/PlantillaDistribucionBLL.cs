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
    /// Gestiona la lógica de negocio de plantillas de distribución.
    /// </summary>
    public class PlantillaDistribucionBLL
    {
        /// <summary>
        /// Proceso para obtener los departamentos de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<DepartamentoViewModel></returns>
        public static List<DepartamentoViewModel> ObtenerDepartamentos(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return DepartamentoRCL.ObtenerDepartamentos(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener los centros de costro de un departamento y de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="DepartamentoCodigo">Identificador del departamento.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<CentroCostoViewModel></returns>
        public static List<CentroCostoViewModel> ObtenerCentrosCosto(
            ContenedorVariablesSesion sesion, 
            string DepartamentoCodigo, 
            string CompaniaCodigo)
        {
            return CentroCostoRCL.ObtenerCentrosCosto(sesion, DepartamentoCodigo, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener los propósitos de un centro de costo, un departamento y una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="DepartamentoCodigo">Identificador del departamento.</param>
        /// <param name="CentroCostoCodigo">Identificador del centro de costo.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<PropositoViewModel></returns>
        public static List<PropositoViewModel> ObtenerPropositos(
            ContenedorVariablesSesion sesion, 
            string DepartamentoCodigo, 
            string CentroCostoCodigo, 
            string CompaniaCodigo)
        {
            return PropositoRCL.ObtenerPropositos(sesion, DepartamentoCodigo, CentroCostoCodigo, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener las plantillas de distribución de una compañía.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <returns>List<PlantillasDistribucionCabeceraViewModel></returns>
        public static List<PlantillasDistribucionCabeceraViewModel> ObtenerPlantillasDistribucion(
            ApplicationDbContext db, 
            string EmpresaCodigo)
        {
            return PlantillaDistribucionCabeceraDAL.ObtenerPlantillasDistribucion(db, EmpresaCodigo);
        }

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
            return PlantillaDistribucionDetalleDAL.ObtenerDetallesPlantilla(Id, db);
        }

        /// <summary>
        /// Proceso para obtener una plantilla de distribución.
        /// </summary>
        /// <param name="Id">Identificador de la plantilla de distribución.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>PlantillasDistribucionCabeceraViewModel</returns>
        public static PlantillasDistribucionCabeceraViewModel ObtenerPlantilla(
            long Id, 
            ApplicationDbContext db)
        {
            return PlantillaDistribucionCabeceraDAL.ObtenerPlantilla(Id, db);
        }
    }
}
