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
using System.Xml;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de comprobantes electrónicos.
    /// </summary>
    public class ComprobanteElectronicoBLL
    {
        /// <summary>
        /// Proceso para buscar comprobantes electrónicos.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="ruc">Filtro de ruc.</param>
        /// <param name="establecimiento">Filtro de establecimiento.</param>
        /// <param name="puntoEmision">Filtro de punto de emisión.</param>
        /// <param name="secuencial">Filtro de secuencial.</param>
        /// <param name="fechaInicio">Filtro de fecha de inicio.</param>
        /// <param name="fechaFin">Filtro de fecha de fin.</param>
        /// <returns>List<ComprobanteElectronicoViewModel></returns>
        public static List<ComprobanteElectronicoViewModel> Buscar(
            ContenedorVariablesSesion sesion, 
            string ruc, 
            string establecimiento, 
            string puntoEmision, 
            string secuencial, 
            string fechaInicio, 
            string fechaFin)
        {
            return ComprobanteElectronicoRCL.Buscar(
                sesion,
                "0" + (int)EnumCatalogoTipoDocumento.FACTURA,
                fechaInicio,
                fechaFin,
                ruc,
                establecimiento,
                puntoEmision,
                secuencial
            ).Where(x => x.estado == 1).ToList();
        }

        /// <summary>
        /// Proceso para obtener los comprobantes de una recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de la recepción.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<ComprobanteElectronicoViewModel></returns>
        public static List<ComprobanteElectronicoViewModel> ObtenerComprobantesPorRecepcion(
            long RecepcionId, 
            ApplicationDbContext db)
        {
            return ComprobanteElectronicoDAL.ObtenerComprobantesPorRecepcion(RecepcionId, db);
        }

        /// <summary>
        /// Proceso para obtener la fecha de emisión de un comprobate electrónico.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="claveAcceso">Clave de acceso del comprobante.</param>
        /// <returns>string</returns>
        public static string ObtenerFechaEmision(
            ContenedorVariablesSesion sesion, 
            string claveAcceso)
        {
            var json = ComprobanteElectronicoRCL.ObtenerFactura(sesion, claveAcceso);
            var fecha = json.InfoFactura.FechaEmision.ToString("dd/MM/yyyy");

            return fecha;
        }

        /// <summary>
        /// Proceso para cambiar el estado en el repositorio de un comprobante electrónico.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="documento">Objeto que contiene la información del comprobante a modificar.</param>
        public static void CambiarEstado(
            ContenedorVariablesSesion sesion, 
            ComprobanteElectronicoDocumentoViewModel documento)
        {
            ComprobanteElectronicoRCL.CambiarEstado(
                sesion, 
                documento.ruc, 
                documento.establecimiento, 
                documento.puntoEmision, 
                documento.secuencial, 
                documento.tipoDocumento, 
                documento.estadoDocumento, 
                documento.codigoSistema, 
                sesion.usuario.Usuario);
        }
    }
}
