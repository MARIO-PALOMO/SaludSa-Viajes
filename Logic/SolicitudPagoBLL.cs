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
    /// Gestiona la lógica de negocio de solicitudes de pago.
    /// </summary>
    public class SolicitudPagoBLL
    {
        /// <summary>
        /// Proceso para obtener un nuevo número de solicitud.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>long?</returns>
        public static long? ObtenerNumeroSolicitud(ApplicationDbContext db)
        {
            var NumeroMaximo = SolicitudPagoCabeceraDAL.ObtenerNumeroSolicitudMaximo(db);

            NumeroMaximo = ((NumeroMaximo == null) ? 0 : NumeroMaximo);

            return NumeroMaximo + 1;
        }

        /// <summary>
        /// Proceso para obtener proveedor por criterio de búsqueda.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <param name="identificacion">Identificación del proveedor.</param>
        /// <param name="nombresApellidos">Nombre y apellidos del proveedor.</param>
        /// <returns>List<ProveedorPagoViajeViewModel></returns>
        public static List<ProveedorPagoViajeViewModel> ObtenerProveedoresPagoViaje(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo, 
            string identificacion, 
            string nombresApellidos)
        {
            return ProveedorPagoViajeRCL.ObtenerProveedoresPagoViaje(sesion, CompaniaCodigo, identificacion, nombresApellidos);
        }

        /// <summary>
        /// Proceso para obtener las tareas de una solicitud.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaPagoViewModel></returns>
        public static List<TareaPagoViewModel> ObtenerTareas(
            long SolicitudId, 
            ApplicationDbContext db)
        {
            return TareaPagoDAL.ObtenerTareas(SolicitudId, db);
        }

        /// <summary>
        /// Proceso para cargar a MFiles un adjunto.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase del adjunto.</param>
        /// <param name="NombreArchivo">Nombre del adjunto.</param>
        /// <param name="Adjunto">Objeto que contiene los datos del adjunto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void CargarAdjunto(
            string IdClase, 
            string NombreArchivo, 
            AdjuntoViewModel Adjunto, 
            ContenedorVariablesSesion sesion)
        {
            MFilesRCL.CargarAdjunto(IdClase, NombreArchivo, Adjunto, sesion);
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de un usuario.
        /// </summary>
        /// <param name="Usuario">Nombre de usuario.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="MostrarContabilizados">Bandera que indica si se deben incluir las solicitudes contabilizadas.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudPagoCabeceraViewModel> ObtenerSolicitudes(
            string Usuario, 
            ApplicationDbContext db, 
            bool MostrarContabilizados)
        {
            return SolicitudPagoCabeceraDAL.ObtenerSolicitudes(Usuario, db, MostrarContabilizados);
        }

        /// <summary>
        /// Proceso para obtener una solicitud con una factura.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="FacturaId">Identificador de la factura.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudPagoCabeceraViewModel</returns>
        public static SolicitudPagoCabeceraViewModel ObtenerSolicitud(
            long Id, 
            long? FacturaId, 
            ApplicationDbContext db)
        {
            return SolicitudPagoCabeceraDAL.ObtenerSolicitud(Id, FacturaId, db);
        }

        /// <summary>
        /// Proceso para obtener una solicitud con una factura ya contabilizada.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="FacturaId">Identificador de la factura.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudPagoCabeceraViewModel</returns>
        public static SolicitudPagoCabeceraViewModel ObtenerSolicitudContabilidad(
            long Id, 
            long FacturaId, 
            ApplicationDbContext db)
        {
            return SolicitudPagoCabeceraDAL.ObtenerSolicitudContabilidad(Id, FacturaId, db);
        }

        /// <summary>
        /// Proceso para obtener los perfiles de asiento contable para una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<PerfilAsientoContablePagoViewModel></returns>
        public static List<PerfilAsientoContablePagoViewModel> ObtenerPerfilesAsientosContablesPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return PerfilAsientoContablePagoRCL.ObtenerPerfilesAsientosContablesPago(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener los diarios de una campañía y un tipo de diario.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <param name="tipoDiario">Tipo de diario.</param>
        /// <returns>List<DiarioPagoViewModel></returns>
        public static List<DiarioPagoViewModel> ObtenerDiariosPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo, 
            int tipoDiario)
        {
            return DiarioPagoRCL.ObtenerDiariosPago(sesion, CompaniaCodigo, tipoDiario);
        }

        /// <summary>
        /// Proceso para obtener los grupos de impuesto para una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<GrupoImpuestoPagoViewModel></returns>
        public static List<GrupoImpuestoPagoViewModel> ObtenerGruposImpuestoPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return GrupoImpuestoPagoRCL.ObtenerGruposImpuestoPago(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener los grupos de impuesto de artículo para una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<GrupoImpuestoArticulosPagoViewModel></returns>
        public static List<GrupoImpuestoArticulosPagoViewModel> ObtenerGruposImpuestosArticulosPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return GrupoImpuestoArticulosPagoRCL.ObtenerGruposImpuestosArticulosPago(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener impuesto de renta para una compañía y un grupo de impuesto de artículo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <param name="codigoGrupoImpuestoArticulo">Identificador del grupo de impuesto de artículo.</param>
        /// <returns>List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel></returns>
        public static List<ImpuestoRentaGrupoImpuestoArticuloPagoViewModel> ObtenerImpuestoRentaGrupoImpuestoArticuloPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo, 
            string codigoGrupoImpuestoArticulo)
        {
            return ImpuestoRentaGrupoImpuestoArticuloPagoRCL.ObtenerImpuestoRentaGrupoImpuestoArticuloPago(sesion, CompaniaCodigo, codigoGrupoImpuestoArticulo);
        }

        /// <summary>
        /// Proceso para obtener impuesto IVA para una compañía y un grupo de impuesto de artículo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <param name="codigoGrupoImpuestoArticulo">Identificador del grupo de impuesto de artículo.</param>
        /// <returns>List<IvaGrupoImpuestoArticuloPagoViewModel></returns>
        public static List<IvaGrupoImpuestoArticuloPagoViewModel> ObtenerIvaGrupoImpuestosArticulosPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo, 
            string codigoGrupoImpuestoArticulo)
        {
            return IvaGrupoImpuestoArticuloPagoRCL.ObtenerIvaGrupoImpuestosArticulosPago(sesion, CompaniaCodigo, codigoGrupoImpuestoArticulo);
        }

        /// <summary>
        /// Proceso para obtener los sustentos tributario de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<SustentoTributarioPagoViewModel></returns>
        public static List<SustentoTributarioPagoViewModel> ObtenerSustentosTributariosPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return SustentoTributarioPagoRCL.ObtenerSustentosTributariosPago(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener las categorías de proyecto de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<CategoriaProyectoPagoViewModel></returns>
        public static List<CategoriaProyectoPagoViewModel> ObtenerCategoriasProyectosPago(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return CategoriaProyectoPagoRCL.ObtenerCategoriasProyectosPago(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para verificar si ya existe una factura registrada en una solicitud.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="NoFactura">Número de la factura.</param>
        /// <param name="RUC">RUC de la factura.</param>
        /// <param name="SolicitudId">Identificador de la solicitud.</param>
        /// <returns>bool</returns>
        public static bool VerificarSiExisteFactura(
            ApplicationDbContext db, 
            string NoFactura, 
            string RUC, 
            long? SolicitudId)
        {
            return SolicitudPagoCabeceraDAL.VerificarSiExisteFactura(db, NoFactura, RUC, SolicitudId);
        }
    }
}
