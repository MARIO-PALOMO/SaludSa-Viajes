using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para tipos de pago.
    /// </summary>
    public class TipoPagoDAL
    {
        /// <summary>
        /// Proceso para buscar los tipos de pago.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los tipos de pago inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TipoPagoViewModel></returns>
        public static List<TipoPagoViewModel> BuscarItems(
            bool MostrarInactivos,
            ApplicationDbContext db)
        {
            List<TipoPagoViewModel> resultado = null;

            if (MostrarInactivos)
            {
                resultado = db.TiposPago.Select(x => new TipoPagoViewModel()
                {
                    Id = x.Id,
                    CuentaContableCodigo = x.CuentaContableCodigo,
                    CuentaContableNombre = x.CuentaContableNombre,
                    CuentaContableTipo = x.CuentaContableTipo,
                    Referencia = x.Referencia,
                    EsReembolso = x.EsReembolso,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion,
                    EmpresaCodigo = x.EmpresaCodigo
                }).ToList();
            }
            else
            {
                resultado = db.TiposPago.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO).Select(x => new TipoPagoViewModel()
                {
                    Id = x.Id,
                    CuentaContableCodigo = x.CuentaContableCodigo,
                    CuentaContableNombre = x.CuentaContableNombre,
                    CuentaContableTipo = x.CuentaContableTipo,
                    Referencia = x.Referencia,
                    EsReembolso = x.EsReembolso,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion,
                    EmpresaCodigo = x.EmpresaCodigo
                }).ToList();
            }

            return resultado;
        }

        /// <summary>
        /// Proceso para buscar un tipo de pago.
        /// </summary>
        /// <param name="Id">Identificador del tipo de pago.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>TipoPagoViewModel</returns>
        public static TipoPagoViewModel BuscarItem(
            long Id,
            ApplicationDbContext db)
        {
            var resultado = db.TiposPago.Where(x => x.Id == Id).Select(x => new TipoPagoViewModel()
            {
                Id = x.Id,
                Referencia = x.Referencia,
                EsReembolso = x.EsReembolso,

                CuentaContable = new CuentaContableViewModel()
                {
                    Codigo = x.CuentaContableCodigo,
                    Nombre = x.CuentaContableNombre,
                    Tipo = x.CuentaContableTipo
                },
                Estado = new EstadoViewModel()
                {
                    Id = x.EstadoId,
                    Descripcion = x.Estado.Descripcion
                },
                EmpresaCodigo = x.EmpresaCodigo
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para buscar los tipos de pago por compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <param name="EsReembolso">Bandera que indica si filtrar por reembolsos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TipoPagoViewModel></returns>
        public static List<TipoPagoViewModel> BuscarItemsPorEmpresa(
            string EmpresaCodigo, 
            bool? EsReembolso,
            ApplicationDbContext db)
        {
            List<TipoPagoViewModel> resultado = null;

            if (EsReembolso == null)
            {
                resultado = db.TiposPago.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.EmpresaCodigo == EmpresaCodigo).Select(x => new TipoPagoViewModel()
                {
                    Id = x.Id,
                    CuentaContableCodigo = x.CuentaContableCodigo,
                    CuentaContableNombre = x.CuentaContableNombre,
                    CuentaContableTipo = x.CuentaContableTipo,
                    Referencia = x.Referencia,
                    EsReembolso = x.EsReembolso,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion,
                    EmpresaCodigo = x.EmpresaCodigo
                }).ToList();
            }
            else
            {
                if(EsReembolso == true)
                {
                    resultado = db.TiposPago.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.EmpresaCodigo == EmpresaCodigo && x.EsReembolso == true).Select(x => new TipoPagoViewModel()
                    {
                        Id = x.Id,
                        CuentaContableCodigo = x.CuentaContableCodigo,
                        CuentaContableNombre = x.CuentaContableNombre,
                        CuentaContableTipo = x.CuentaContableTipo,
                        Referencia = x.Referencia,
                        EsReembolso = x.EsReembolso,
                        EstadoId = x.EstadoId,
                        EstadoNombre = x.Estado.Descripcion,
                        EmpresaCodigo = x.EmpresaCodigo
                    }).ToList();
                }
                else
                {
                    resultado = db.TiposPago.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.EmpresaCodigo == EmpresaCodigo && x.EsReembolso == false).Select(x => new TipoPagoViewModel()
                    {
                        Id = x.Id,
                        CuentaContableCodigo = x.CuentaContableCodigo,
                        CuentaContableNombre = x.CuentaContableNombre,
                        CuentaContableTipo = x.CuentaContableTipo,
                        Referencia = x.Referencia,
                        EsReembolso = x.EsReembolso,
                        EstadoId = x.EstadoId,
                        EstadoNombre = x.Estado.Descripcion,
                        EmpresaCodigo = x.EmpresaCodigo
                    }).ToList();
                }
            }

            return resultado;
        }
    }
}
