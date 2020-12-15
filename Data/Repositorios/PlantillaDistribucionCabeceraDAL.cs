using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para cabecera de plantillas de distribución.
    /// </summary>
    public class PlantillaDistribucionCabeceraDAL
    {
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
            PlantillasDistribucionCabeceraViewModel resultado = null;

            resultado = db.PlantillasDistribucionCabecera.Where(x => x.Id == Id).Select(x => new PlantillasDistribucionCabeceraViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                EstadoId = x.EstadoId,
                UsuarioPropietario = x.UsuarioPropietario,
                DescripcionDepartamentoPropietario = x.DescripcionDepartamentoPropietario,
                EmpresaCodigo = x.EmpresaCodigo,
                Detalles = db.PlantillasDistribucionDetalle.Where(y => y.DistribucionCabeceraId == x.Id && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new PlantillasDistribucionDetalleViewModel()
                {
                    Id = y.Id,
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
                    },
                }).ToList()
            }).FirstOrDefault();

            return resultado;
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
            var resultado = db.PlantillasDistribucionCabecera.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO).Select(x => new PlantillasDistribucionCabeceraViewModel() {
                Id = x.Id,
                Descripcion = x.Descripcion,
                UsuarioPropietario = x.UsuarioPropietario,
                DescripcionDepartamentoPropietario = x.DescripcionDepartamentoPropietario,
                EmpresaCodigo = x.EmpresaCodigo
            });

            if(EmpresaCodigo != null)
            {
                resultado = resultado.Where(x => x.EmpresaCodigo == EmpresaCodigo);
            }

            return resultado.ToList();
        }
    }
}
