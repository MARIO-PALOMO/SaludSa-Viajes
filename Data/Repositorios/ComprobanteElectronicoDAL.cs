using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para comprobantes electrónicos.
    /// </summary>
    public class ComprobanteElectronicoDAL
    {
        /// <summary>
        /// Proceso para obtener los comprobantes por recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de la recepción.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<ComprobanteElectronicoViewModel></returns>
        public static List<ComprobanteElectronicoViewModel> ObtenerComprobantesPorRecepcion(
            long RecepcionId, 
            ApplicationDbContext db)
        {
            List<ComprobanteElectronicoViewModel> resultado = null;

            resultado = db.ComprobantesElectronicos.Where(y => y.RecepcionId == RecepcionId && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new ComprobanteElectronicoViewModel()
            {
                Id = y.Id,
                baseImponibleCero = y.baseImponibleCero,
                baseImponibleIva = y.baseImponibleIva,
                baseSinCargos = y.baseSinCargos,
                claveAcceso = y.claveAcceso,
                codigoImpuestoIva = y.codigoImpuestoIva,
                establecimiento = y.establecimiento,
                estado = y.estado,
                fechaAutorizacion = y.fechaAutorizacion,
                fechaEmisionRetencion = y.fechaEmisionRetencion,
                iva = y.iva,
                numeroAutorizacion = y.numeroAutorizacion,
                observaciones = y.observaciones,
                porcentajeRetencion = y.porcentajeRetencion,
                puntoEmision = y.puntoEmision,
                razonSocial = y.razonSocial,
                ruc = y.ruc,
                secuencial = y.secuencial,
                tipoDocumento = y.tipoDocumento,
                valorRetencion = y.valorRetencion,
                valorTotal = y.valorTotal,
                numeroDocumento = y.numeroDocumento,
                tipoDocumentoNombre = y.tipoDocumentoNombre,
                estadoNombre = y.estadoNombre,

                infoAdicional = db.ComprobanteElectronicoInfosAdicional.Where(z => z.ComprobanteElectronicoId == y.Id).Select(z => new ComprobanteElectronicoInfoAdicionalViewModel()
                {
                    Id = z.Id,
                    nombre = z.nombre,
                    valor = z.valor
                }).ToList()
            }).ToList();

            return resultado;
        }
    }
}
