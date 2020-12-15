using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
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
    /// Gestiona la lógica de negocio de solicitudes de compra.
    /// </summary>
    public class SolicitudCompraBLL
    {
        /// <summary>
        /// Proceso para obtener un nuevo número de solicitud.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>long?</returns>
        public static long? ObtenerNumeroSolicitud(ApplicationDbContext db)
        {
            var NumeroMaximo = SolicitudCompraCabeceraDAL.ObtenerNumeroSolicitudMaximo(db);

            NumeroMaximo = ((NumeroMaximo == null) ? 0 : NumeroMaximo);

            return NumeroMaximo + 1;
        }

        /// <summary>
        /// Proceso para obtener un nuevo número de recepción.
        /// </summary>
        /// <param name="OrdenMadreId">Identificador de la orden madre a la que pertenece la recepción.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>int?</returns>
        public static int? ObtenerNumeroRecepcion(
            long OrdenMadreId, 
            ApplicationDbContext db)
        {
            var NumeroMaximo = RecepcionDAL.ObtenerNumeroRecepcionMaximo(OrdenMadreId, db);

            NumeroMaximo = ((NumeroMaximo == null) ? 0 : NumeroMaximo);

            return NumeroMaximo + 1;
        }

        /// <summary>
        /// Proceso para obtener los productos de un tipo y de una compañía.
        /// </summary>
        /// <param name="Tipo">Tipo de producto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<ProductoViewModel></returns>
        public static List<ProductoViewModel> ObtenerProductosPorTipo(
            string Tipo, ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            var productos = ProductoRCL.ObtenerProductosPorTipo(Tipo, sesion, CompaniaCodigo);

            if (Tipo == "0")
            {
                productos.Insert(0, new ProductoViewModel()
                {
                    Nombre = "OTRO",
                    Tipo = "BIEN",
                    Grupo = "OTRO_BIEN",
                    CodigoArticulo = "BIEN_OTRO_P123",
                    CodigoGrupoArticulo = "OTRO_BIEN_123"
                });
            }
            else if (Tipo == "2")
            {
                productos.Insert(0, new ProductoViewModel()
                {
                    Nombre = "OTRO",
                    Tipo = "SERVICIO",
                    Grupo = "OTRO_SERVICIO",
                    CodigoArticulo = "SERVICIO_OTRO_P123",
                    CodigoGrupoArticulo = "OTRO_SERVICIO_123"
                });
            }

            return productos;
        }

        /// <summary>
        /// Proceso para obtener los productos de mercadeo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<ProductoMercadeoViewModel></returns>
        public static List<ProductoMercadeoViewModel> ObtenerProductosMercadeo(ContenedorVariablesSesion sesion)
        {
            return ProductoRCL.ObtenerProductosMercadeo(sesion);
        }

        /// <summary>
        /// Proceso para obtener usuarios de determinados grupos.
        /// </summary>
        /// <param name="gruposJefesAreas">Listado de grupos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns></returns>
        public static List<UsuarioViewModel> ObtenerUsuariosPorGrupo(
            string[] gruposJefesAreas, 
            ContenedorVariablesSesion sesion)
        {
            return UsuarioRCL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);
        }

        /// <summary>
        /// Proceso para obtener usuarios subgerentes.
        /// </summary>
        /// <param name="gruposJefesAreas">Listado de grupos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<UsuarioViewModel></returns>
        public static List<UsuarioViewModel> ObtenerSubgerentes(
            string[] gruposJefesAreas, 
            ContenedorVariablesSesion sesion)
        {
            List<UsuarioViewModel> Subgerentes = UsuarioRCL.ObtenerUsuariosPorGrupo(gruposJefesAreas, sesion);

            Subgerentes.Insert(0, new UsuarioViewModel()
            {
                NombreCompleto = "N/A",
                Usuario = "noaplica_0123"
            });

            return Subgerentes;
        }

        /// <summary>
        /// Proceso para obtener las compañías.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<EmpresaViewModel></returns>
        public static List<EmpresaViewModel> ObtenerEmpresas(ContenedorVariablesSesion sesion)
        {
            return EmpresaRCL.ObtenerEmpresas(sesion);
        }

        /// <summary>
        /// Proceso para obtener los proveedores de una compañía.
        /// </summary>
        /// <param name="sesion"></param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<ProveedorViewModel></returns>
        public static List<ProveedorViewModel> ObtenerProveedores(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return ProveedorRCL.ObtenerProveedores(sesion, CompaniaCodigo);
        }

        /// <summary>
        /// Proceso para obtener los impuestos vigentes de una compañía.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="CompaniaCodigo">Identificador de la compañía.</param>
        /// <returns>List<ImpuestoVigenteViewModel></returns>
        public static List<ImpuestoVigenteViewModel> ObtenerImpuestosVigente(
            ContenedorVariablesSesion sesion, 
            string CompaniaCodigo)
        {
            return ImpuestoVigenteRCL.ObtenerImpuestosVigente(sesion, CompaniaCodigo);

            //return new List<ImpuestoVigenteViewModel>() { new ImpuestoVigenteViewModel()
            //{
            //    Codigo = "2",
            //    Porcentaje = 12,
            //    Descripcion = "12 %"
            //} };
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de un usuario.
        /// </summary>
        /// <param name="Usuario">Nombre de usuario.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="SaldoCero">Bandera que indica si se deben incluir las solicitudes con saldo cero.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudCompraCabeceraViewModel> ObtenerSolicitudes(
            string Usuario, 
            ApplicationDbContext db, 
            bool SaldoCero)
        {
            return SolicitudCompraCabeceraDAL.ObtenerSolicitudes(Usuario, db, SaldoCero);
        }

        /// <summary>
        /// Proceso para obtener una solicitud.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraCabeceraViewModel</returns>
        public static SolicitudCompraCabeceraViewModel ObtenerSolicitud(
            long Id, 
            ApplicationDbContext db)
        {
            return SolicitudCompraCabeceraDAL.ObtenerSolicitud(Id, db);
        }

        /// <summary>
        /// Proceso para obtener una solicitud con saldo en sus detalles.
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraCabeceraViewModel</returns>
        public static SolicitudCompraCabeceraViewModel ObtenerSolicitudConSaldosEnDetalles(
            long Id, 
            ApplicationDbContext db)
        {
            return SolicitudCompraCabeceraDAL.ObtenerSolicitudConSaldosEnDetalles(Id, db);
        }

        /// <summary>
        /// roceso para obtener un usuario.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>UsuarioViewModel</returns>
        public static UsuarioViewModel ObtenerUsuario(
            string username, 
            ContenedorVariablesSesion sesion)
        {
            return UsuarioRCL.ObtenerUsuario(username, sesion);
        }

        /// <summary>
        /// Proceso para obtener un producto.
        /// </summary>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="codigoArticulo">Identificador del producto.</param>
        /// <param name="codigoGrupo">Identificador del grupo del producto.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>ProductoViewModel</returns>
        public static ProductoViewModel ObtenerProducto(
            string codigoCompania, 
            string codigoArticulo, 
            string codigoGrupo, 
            ContenedorVariablesSesion sesion)
        {
            if(codigoArticulo == null || codigoGrupo == null)
            {
                return null;
            }
            else if(codigoArticulo == "BIEN_OTRO_P123")
            {
                return new ProductoViewModel()
                {
                    Nombre = "OTRO",
                    Tipo = "BIEN",
                    Grupo = null,
                    CodigoArticulo = "BIEN_OTRO_P123",
                    CodigoGrupoArticulo = null
                };
            }
            else if(codigoArticulo == "SERVICIO_OTRO_P123")
            {
                return new ProductoViewModel()
                {
                    Nombre = "OTRO",
                    Tipo = "SERVICIO",
                    Grupo = null,
                    CodigoArticulo = "SERVICIO_OTRO_P123",
                    CodigoGrupoArticulo = null
                };
            }
            else
            {
                return ProductoRCL.ObtenerProducto(codigoCompania, codigoArticulo, codigoGrupo, sesion);
            }
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
        /// Proceso para descargar un adjunto.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase del adjunto.</param>
        /// <param name="PropiedadesBusqueda">Listado con propiedades de búsqueda.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>AdjuntoViewModel</returns>
        public static AdjuntoViewModel DescargarAdjunto(
            string IdClase, 
            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda, 
            ContenedorVariablesSesion sesion)
        {
            return MFilesRCL.DescargarAdjunto(IdClase, PropiedadesBusqueda, sesion);
        }

        /// <summary>
        /// Proceso para buscar adjuntos.
        /// </summary>
        /// <param name="IdClase">Identificador de la clase de los adjuntos.</param>
        /// <param name="PropiedadesBusqueda">Listado con propiedades de búsqueda.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<AdjuntoViewModel></returns>
        public static List<AdjuntoViewModel> BuscarAdjuntos(
            string IdClase, 
            List<PropiedadAdjuntoViewModel> PropiedadesBusqueda, 
            ContenedorVariablesSesion sesion)
        {
            return MFilesRCL.BuscarAdjuntos(IdClase, PropiedadesBusqueda, sesion);
        }

        /// <summary>
        /// Proceso para modificar en MFiles las propiedades de un adjunto.
        /// </summary>
        /// <param name="idTipoObjeto">Identificador de tipo de objeto en MFiles.</param>
        /// <param name="idObjeto">Identificador de objeto en MFiles.</param>
        /// <param name="PropiedadesModificar">Listado de propiedades a modificar.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>AdjuntoViewModel</returns>
        public static AdjuntoViewModel ModificarPropiedadesAdjunto(
            string idTipoObjeto, 
            string idObjeto, 
            List<PropiedadAdjuntoViewModel> PropiedadesModificar, 
            ContenedorVariablesSesion sesion)
        {
            return MFilesRCL.ModificarPropiedadesAdjunto(idTipoObjeto, idObjeto, PropiedadesModificar, sesion);
        }

        /// <summary>
        /// Proceso para obtener las tareas de una solicitud.
        /// </summary>
        /// <param name="SolicitudId">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaViewModel></returns>
        public static List<TareaViewModel> ObtenerTareas(
            long SolicitudId, 
            ApplicationDbContext db)
        {
            return TareaDAL.ObtenerTareas(SolicitudId, db);
        }

        /// <summary>
        /// Proceso para validar que el grupo de productos de la solicitud tenga asignado un gestor de compras.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<string></returns>
        public static List<string> ValidarRolesGestoresCompra(
            SolicitudCompraCabecera Solicitud, 
            ApplicationDbContext db)
        {
            List<string> validacion = new List<string>();

            foreach (var detalle in Solicitud.Detalles)
            {
                if(detalle.Producto != "BIEN_OTRO_P123" && detalle.Producto != "SERVICIO_OTRO_P123")
                {
                    if (!RolGestorCompraDAL.ValidarRol(detalle.GrupoProducto, Solicitud.EmpresaCodigo, db))
                    {
                        validacion.Add("El grupo de productos " + detalle.GrupoProductoNombre + " no tiene configurado un gestor de compras.");
                    }
                }
            }

            return validacion;
        }

        /// <summary>
        /// Proceso para obtener el historial de recepciones de una orden madre.
        /// </summary>
        /// <param name="TareaId">Identificador de la tarea relacionada con la orden madre.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RecepcionViewModel></returns>
        public static List<RecepcionViewModel> ObtenerHistorialRecepciones(
            long TareaId, 
            ApplicationDbContext db)
        {
            return RecepcionDAL.ObtenerHistorialRecepciones(TareaId, db);
        }
    }
}
