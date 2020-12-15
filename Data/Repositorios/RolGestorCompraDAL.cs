using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para roles de gestores de compra.
    /// </summary>
    public class RolGestorCompraDAL
    {
        /// <summary>
        /// Proceso para obtener los roles de gestores de compra.
        /// </summary>
        /// <param name="MostrarInactivos">Bandera que indica si se deben incluir los roles inactivos.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<RolGestorCompraViewModel></returns>
        public static List<RolGestorCompraViewModel> ObtenerRolesGestorCompra(
            bool MostrarInactivos, 
            ApplicationDbContext db)
        {
            List<RolGestorCompraViewModel> resultado = null;
            
            if (MostrarInactivos)
            {
                resultado = db.RolesGestorCompra.Select(x => new RolGestorCompraViewModel() {
                    Id = x.Id,
                    CodigoEmpresa = x.CodigoEmpresa,
                    NombreEmpresa = x.NombreEmpresa,
                    CodigoTipoCompra = x.CodigoTipoCompra,
                    NombreTipoCompra = x.NombreTipoCompra,
                    UsuarioGestorSierra = x.UsuarioGestorSierra,
                    NombreGestorSierra = x.NombreGestorSierra,
                    UsuarioGestorCosta = x.UsuarioGestorCosta,
                    NombreGestorCosta = x.NombreGestorCosta,
                    UsuarioGestorAustro = x.UsuarioGestorAustro,
                    NombreGestorAustro = x.NombreGestorAustro,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion
                }).ToList();
            }
            else
            {
                resultado = db.RolesGestorCompra.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO).Select(x => new RolGestorCompraViewModel() {
                    Id = x.Id,
                    CodigoEmpresa = x.CodigoEmpresa,
                    NombreEmpresa = x.NombreEmpresa,
                    CodigoTipoCompra = x.CodigoTipoCompra,
                    NombreTipoCompra = x.NombreTipoCompra,
                    UsuarioGestorSierra = x.UsuarioGestorSierra,
                    NombreGestorSierra = x.NombreGestorSierra,
                    UsuarioGestorCosta = x.UsuarioGestorCosta,
                    NombreGestorCosta = x.NombreGestorCosta,
                    UsuarioGestorAustro = x.UsuarioGestorAustro,
                    NombreGestorAustro = x.NombreGestorAustro,
                    EstadoId = x.EstadoId,
                    EstadoNombre = x.Estado.Descripcion
                }).ToList();
            }

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener un rol de gestor de compra.
        /// </summary>
        /// <param name="Id">Identificador del rol de gestor de compra.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>RolGestorCompraViewModel</returns>
        public static RolGestorCompraViewModel ObtenerRolGestorCompra(
            long Id, 
            ApplicationDbContext db)
        {
            RolGestorCompraViewModel resultado = null;

            resultado = db.RolesGestorCompra.Where(x => x.Id == Id).Select(x => new RolGestorCompraViewModel()
            {
                Id = x.Id,
                Empresa = new EmpresaViewModel()
                {
                    Codigo = x.CodigoEmpresa,
                    Nombre = x.NombreEmpresa
                },
                TipoCompra = new TipoCompraViewModel()
                {
                    Codigo = x.CodigoTipoCompra,
                    Nombre = x.NombreTipoCompra
                },
                UsuarioGestorSierra = x.UsuarioGestorSierra,
                UsuarioGestorCosta = x.UsuarioGestorCosta,
                UsuarioGestorAustro = x.UsuarioGestorAustro,
                Estado = new EstadoViewModel()
                {
                    Id = x.EstadoId,
                    Descripcion = x.Estado.Descripcion
                }
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener el gestor de una compra.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de solicitud de compra.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerGestorDeUnaCompra(
            SolicitudCompraCabecera Solicitud, 
            ApplicationDbContext db)
        {
            string resultado = string.Empty;
            
            var detalle = db.SolicitudesCompraDetalle.Where(x => x.SolicitudCompraCabeceraId == Solicitud.Id).OrderBy(x => x.Id).FirstOrDefault();

            if(detalle != null)
            {
                var Rol = db.RolesGestorCompra.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.CodigoTipoCompra == detalle.GrupoProducto && x.CodigoEmpresa == Solicitud.EmpresaCodigo).FirstOrDefault();

                if(Rol != null)
                {
                    var ciudad = db.Ciudades.Where(x => x.Codigo == Solicitud.SolicitanteCiudadCodigo).FirstOrDefault();

                    if(ciudad != null)
                    {
                        var region = ciudad.Region;

                        if(region != null)
                        {
                            switch(region.Descripcion)
                            {
                                case "Costa":
                                    resultado = Rol.UsuarioGestorCosta;
                                    break;
                                case "Sierra":
                                    resultado = Rol.UsuarioGestorSierra;
                                    break;
                                case "Austro":
                                    resultado = Rol.UsuarioGestorAustro;
                                    break;
                            }
                        }
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Proceso para validar un rol.
        /// </summary>
        /// <param name="GrupoProducto">Identificador del grupo de producto.</param>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>bool</returns>
        public static bool ValidarRol(
            string GrupoProducto,
            string EmpresaCodigo, 
            ApplicationDbContext db)
        {
            bool result = false;
            
            var Rol = db.RolesGestorCompra.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.CodigoTipoCompra == GrupoProducto && x.CodigoEmpresa == EmpresaCodigo).FirstOrDefault();

            if (Rol != null)
            {
                result = true;
            }

            return result;
        }
    }
}
