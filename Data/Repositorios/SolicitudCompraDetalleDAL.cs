using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para detalles de solicitudes del compra.
    /// </summary>
    public class SolicitudCompraDetalleDAL
    {
        /// <summary>
        /// Proceso para obtener los detalles de una solicitud.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<SolicitudCompraDetalleViewModel></returns>
        public static List<SolicitudCompraDetalleViewModel> ObtenerDetalles(
            long SolicitudId, 
            ApplicationDbContext db)
        {
            List<SolicitudCompraDetalleViewModel> resultado = null;
            
            resultado = db.SolicitudesCompraDetalle.Where(y => y.SolicitudCompraCabeceraId == SolicitudId && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new SolicitudCompraDetalleViewModel()
            {
                Id = y.Id,
                SolicitudCompraCabeceraId = SolicitudId,
                EstadoId = y.EstadoId,
                CompraInternacional = y.CompraInternacional,
                Tipo = y.Tipo,
                ProductoCodigoArticulo = y.Producto,
                ProductoNombre = y.ProductoNombre,
                ProductoCodigoGrupo = y.GrupoProducto,

                Observacion = y.Observacion,
                Cantidad = y.Cantidad,
                Url = y.Url,

                Valor = y.Valor,
                Total = y.Total,

                Impuesto = new ImpuestoVigenteViewModel()
                {
                    Codigo = y.CodigoImpuestoVigente,
                    Porcentaje = y.PorcentajeImpuestoVigente,
                    Descripcion = y.DescripcionImpuestoVigente
                },

                Proveedor = new ProveedorViewModel()
                {
                    Identificacion = y.IdentificacionProveedor,
                    Nombre = y.NombreProveedor,
                    RazonSocial = y.RazonSocialProveedor,
                    TipoIdentificacion = y.TipoIdentificacionProveedor,
                    Bloqueado = y.BloqueadoProveedor,
                    Correo = y.CorreoProveedor,
                    Telefono = y.TelefonoProveedor,
                    Direccion = y.DireccionProveedor
                }
            }).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener un detalle de solicitud.
        /// </summary>
        /// <param name="Id">Identificador del detalle.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraDetalleViewModel</returns>
        public static SolicitudCompraDetalleViewModel ObtenerDetalle(
            long Id, 
            ApplicationDbContext db)
        {
            SolicitudCompraDetalleViewModel resultado = null;
            
            resultado = db.SolicitudesCompraDetalle.Where(y => y.Id == Id).Select(y => new SolicitudCompraDetalleViewModel()
            {
                Id = y.Id,
                SolicitudCompraCabeceraId = (long)y.SolicitudCompraCabeceraId,
                EstadoId = y.EstadoId,
                CompraInternacional = y.CompraInternacional,
                Tipo = y.Tipo,
                ProductoCodigoArticulo = y.Producto,
                ProductoNombre = y.ProductoNombre,
                ProductoCodigoGrupo = y.GrupoProducto,

                Observacion = y.Observacion,
                Cantidad = y.Cantidad,
                Url = y.Url,

                Valor = y.Valor,
                Total = y.Total,

                Impuesto = new ImpuestoVigenteViewModel()
                {
                    Codigo = y.CodigoImpuestoVigente,
                    Porcentaje = y.PorcentajeImpuestoVigente,
                    Descripcion = y.DescripcionImpuestoVigente
                },

                Proveedor = new ProveedorViewModel()
                {
                    Identificacion = y.IdentificacionProveedor,
                    Nombre = y.NombreProveedor,
                    RazonSocial = y.RazonSocialProveedor,
                    TipoIdentificacion = y.TipoIdentificacionProveedor,
                    Bloqueado = y.BloqueadoProveedor,
                    Correo = y.CorreoProveedor,
                    Telefono = y.TelefonoProveedor,
                    Direccion = y.DireccionProveedor
                }
            }).FirstOrDefault();

            return resultado;
        }
    }
}
