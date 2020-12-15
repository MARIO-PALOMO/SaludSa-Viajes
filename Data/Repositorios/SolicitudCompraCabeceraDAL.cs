using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para cabecera de solicitudes de compra.
    /// </summary>
    public class SolicitudCompraCabeceraDAL
    {
        /// <summary>
        /// Proceso para obtener el número máximo de solicitud.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>long?</returns>
        public static long? ObtenerNumeroSolicitudMaximo(ApplicationDbContext db)
        {
            long? resultado = 0;
            
            resultado = db.SolicitudesCompraCabecera.Max(x => x.NumeroSolicitud);

            return resultado;
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
            var resultado = db.SolicitudesCompraCabecera.Where(x => x.SolicitanteUsuario == Usuario 
                                                                && x.EstadoId != (int)EnumEstado.INACTIVO
                                                    ).Select(x => new SolicitudCompraCabeceraViewModel() {
                Id = x.Id,
                Descripcion = x.Descripcion,
                NumeroSolicitud = x.NumeroSolicitud,
                ProveedorSugerido = x.ProveedorSugerido,
                FechaSolicitud = x.FechaSolicitud,
                CantTareasActivas = db.Tareas.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.SolicitudCompraCabeceraId == x.Id).Count()
            });

            if(!SaldoCero)
            {
                resultado = resultado.Where(x => x.CantTareasActivas > 0 || x.NumeroSolicitud == null);
            }

            return resultado.ToList();
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
            SolicitudCompraCabeceraViewModel resultado = null;
            
            resultado = db.SolicitudesCompraCabecera.Where(x => x.Id == Id).Select(x => new SolicitudCompraCabeceraViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                NumeroSolicitud = x.NumeroSolicitud,
                ProveedorSugerido = x.ProveedorSugerido,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                ProductoMercadeo = new ProductoMercadeoViewModel() {
                    Codigo = x.ProductoMercadeoCodigo,
                    Nombre = x.ProductoMercadeoNombre
                },
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel() {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                Frecuencia = x.Frecuencia,
                MontoEstimado = x.MontoEstimado,
                RequerimientosAdjuntosPrevisualizar = null,
                JsonOriginal = x.JsonOriginal,
                Detalles = db.SolicitudesCompraDetalle.Where(y => y.SolicitudCompraCabeceraId == x.Id && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new SolicitudCompraDetalleViewModel()
                {
                    Id = y.Id,
                    SolicitudCompraCabeceraId = x.Id,
                    EstadoId = y.EstadoId,
                    CompraInternacional = y.CompraInternacional,
                    Tipo = y.Tipo,
                    ProductoCodigoArticulo = y.Producto,
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
                    },

                    PlantillaDistribucionDetalle = db.SolicitudCompraDetalleDistribuciones.Where(z => z.SolicitudCompraDetalleId == y.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
                    {
                        Id = z.Id,
                        Porcentaje = z.Porcentaje,
                        EstadoId = z.EstadoId,
                        Departamento = new DepartamentoViewModel
                        {
                            Codigo = z.DepartamentoCodigo,
                            Descripcion = z.DepartamentoDescripcion,
                            CodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        },
                        DepartamentoCodigo = z.DepartamentoCodigo,
                        DepartamentoDescripcion = z.DepartamentoDescripcion,
                        DepartamentoCodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        CentroCosto = new CentroCostoViewModel
                        {
                            Codigo = z.CentroCostoCodigo,
                            Descripcion = z.CentroCostoDescripcion,
                            CodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        },
                        CentroCostoCodigo = z.CentroCostoCodigo,
                        CentroCostoDescripcion = z.CentroCostoDescripcion,
                        CentroCostoCodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        Proposito = new PropositoViewModel
                        {
                            Codigo = z.PropositoCodigo,
                            Descripcion = z.PropositoDescripcion,
                            CodigoDescripcion = z.PropositoCodigoDescripcion,
                        },
                        PropositoCodigo = z.PropositoCodigo,
                        PropositoDescripcion = z.PropositoDescripcion,
                        PropositoCodigoDescripcion = z.PropositoCodigoDescripcion,
                            
                        //MetadatosCentrosCosto = new List<CentroCostoViewModel>(),
                        //MetadatosPropositos = new List<PropositoViewModel>()
                    }).ToList()
                }).ToList(),
                Tareas = db.Tareas.Where(y => y.SolicitudCompraCabeceraId == x.Id).Select(y => new TareaViewModel() {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    UsuarioResponsable = y.UsuarioResponsable,
                    NombreCompletoResponsable = y.NombreCompletoResponsable,
                    FechaCreacion = y.FechaCreacion,
                    FechaProcesamiento = y.FechaProcesamiento,
                    TiempoColor = y.TiempoColor,
                    SolicitudCompraCabeceraId = y.SolicitudCompraCabeceraId,
                    EstadoId = y.EstadoId,
                    Accion = y.Accion,
                    Observacion = y.Observacion,
                    NombreProveedor = (db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null),
                    NumeroRecepcion = db.Recepciones.Where(zz => zz.Id == y.RecepcionId).Select(zz => zz.NumeroRecepcion).FirstOrDefault()
                }).OrderBy(y => y.Id).ToList()
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una solicitud (Solo campos básicos).
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraCabeceraViewModel</returns>
        public static SolicitudCompraCabeceraViewModel ObtenerSolicitudCamposBasicos(
            long Id, 
            ApplicationDbContext db)
        {
            SolicitudCompraCabeceraViewModel resultado = null;
            
            resultado = db.SolicitudesCompraCabecera.Where(x => x.Id == Id).Select(x => new SolicitudCompraCabeceraViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                NumeroSolicitud = x.NumeroSolicitud,
                ProveedorSugerido = x.ProveedorSugerido,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                ProductoMercadeo = new ProductoMercadeoViewModel()
                {
                    Codigo = x.ProductoMercadeoCodigo,
                    Nombre = x.ProductoMercadeoNombre
                },
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                Frecuencia = x.Frecuencia,
                MontoEstimado = x.MontoEstimado,
                RequerimientosAdjuntosPrevisualizar = null
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una solicitud (Con saldos en los detalles).
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraCabeceraViewModel</returns>
        public static SolicitudCompraCabeceraViewModel ObtenerSolicitudConSaldosEnDetalles(
            long Id,
            ApplicationDbContext db)
        {
            SolicitudCompraCabeceraViewModel resultado = null;
            
            resultado = db.SolicitudesCompraCabecera.Where(x => x.Id == Id).Select(x => new SolicitudCompraCabeceraViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                NumeroSolicitud = x.NumeroSolicitud,
                ProveedorSugerido = x.ProveedorSugerido,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                ProductoMercadeo = new ProductoMercadeoViewModel()
                {
                    Codigo = x.ProductoMercadeoCodigo,
                    Nombre = x.ProductoMercadeoNombre
                },
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                Frecuencia = x.Frecuencia,
                MontoEstimado = x.MontoEstimado,
                RequerimientosAdjuntosPrevisualizar = null,
                Detalles = db.SolicitudesCompraDetalle.Where(y => y.SolicitudCompraCabeceraId == x.Id && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new SolicitudCompraDetalleViewModel()
                {
                    Id = y.Id,
                    SolicitudCompraCabeceraId = x.Id,
                    EstadoId = y.EstadoId,
                    CompraInternacional = y.CompraInternacional,
                    Tipo = y.Tipo,
                    ProductoCodigoArticulo = y.Producto,
                    ProductoCodigoGrupo = y.GrupoProducto,
                    Observacion = y.Observacion,
                    Cantidad = y.Cantidad,
                    Url = y.Url,

                    Valor = y.Valor,
                    Total = y.Total,

                    Saldo = y.Total - (db.RecepcionLineas.Where(zz => zz.SolicitudCompraDetalleId == y.Id && zz.EstadoId == (int)EnumEstado.ACTIVO).Count() > 0 ? db.RecepcionLineas.Where(zz => zz.SolicitudCompraDetalleId == y.Id && zz.EstadoId == (int)EnumEstado.ACTIVO).Sum(zz => zz.Valor) : 0),

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
                    },

                    PlantillaDistribucionDetalle = db.SolicitudCompraDetalleDistribuciones.Where(z => z.SolicitudCompraDetalleId == y.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
                    {
                        Id = z.Id,
                        Porcentaje = z.Porcentaje,
                        EstadoId = z.EstadoId,
                        Departamento = new DepartamentoViewModel
                        {
                            Codigo = z.DepartamentoCodigo,
                            Descripcion = z.DepartamentoDescripcion,
                            CodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        },
                        DepartamentoCodigo = z.DepartamentoCodigo,
                        DepartamentoDescripcion = z.DepartamentoDescripcion,
                        DepartamentoCodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        CentroCosto = new CentroCostoViewModel
                        {
                            Codigo = z.CentroCostoCodigo,
                            Descripcion = z.CentroCostoDescripcion,
                            CodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        },
                        CentroCostoCodigo = z.CentroCostoCodigo,
                        CentroCostoDescripcion = z.CentroCostoDescripcion,
                        CentroCostoCodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        Proposito = new PropositoViewModel
                        {
                            Codigo = z.PropositoCodigo,
                            Descripcion = z.PropositoDescripcion,
                            CodigoDescripcion = z.PropositoCodigoDescripcion,
                        },
                        PropositoCodigo = z.PropositoCodigo,
                        PropositoDescripcion = z.PropositoDescripcion,
                        PropositoCodigoDescripcion = z.PropositoCodigoDescripcion,

                        //MetadatosCentrosCosto = new List<CentroCostoViewModel>(),
                        //MetadatosPropositos = new List<PropositoViewModel>()
                    }).ToList()
                }).ToList(),
                Tareas = db.Tareas.Where(y => y.SolicitudCompraCabeceraId == x.Id).Select(y => new TareaViewModel()
                {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    UsuarioResponsable = y.UsuarioResponsable,
                    NombreCompletoResponsable = y.NombreCompletoResponsable,
                    FechaCreacion = y.FechaCreacion,
                    FechaProcesamiento = y.FechaProcesamiento,
                    TiempoColor = y.TiempoColor,
                    SolicitudCompraCabeceraId = y.SolicitudCompraCabeceraId,
                    EstadoId = y.EstadoId,
                    Accion = y.Accion,
                    Observacion = y.Observacion,
                    NombreProveedor = (db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null),
                    NumeroRecepcion = db.Recepciones.Where(zz => zz.Id == y.RecepcionId).Select(zz => zz.NumeroRecepcion).FirstOrDefault()
                }).OrderBy(y => y.Id).ToList()
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una solicitud (Con saldos en los detalles para recepción).
        /// </summary>
        /// <param name="Id">Identificador de la solicitud.</param>
        /// <param name="OrdenMadreId">Identificador de la orden madre.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>SolicitudCompraCabeceraViewModel</returns>
        public static SolicitudCompraCabeceraViewModel ObtenerSolicitudConSaldosEnDetallesParaRecepcion(
            long Id, 
            long OrdenMadreId, 
            ApplicationDbContext db)
        {
            SolicitudCompraCabeceraViewModel resultado = null;
            
            resultado = db.SolicitudesCompraCabecera.Where(x => x.Id == Id).Select(x => new SolicitudCompraCabeceraViewModel()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                NumeroSolicitud = x.NumeroSolicitud,
                ProveedorSugerido = x.ProveedorSugerido,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                ProductoMercadeo = new ProductoMercadeoViewModel()
                {
                    Codigo = x.ProductoMercadeoCodigo,
                    Nombre = x.ProductoMercadeoNombre
                },
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                Frecuencia = x.Frecuencia,
                MontoEstimado = x.MontoEstimado,
                RequerimientosAdjuntosPrevisualizar = null,
                Detalles = db.SolicitudesCompraDetalle.Where(y => y.SolicitudCompraCabeceraId == x.Id && y.EstadoId == (int)EnumEstado.ACTIVO && y.OrdenMadreLinea != null && y.OrdenMadreLinea.OrdenMadreId == OrdenMadreId).Select(y => new SolicitudCompraDetalleViewModel()
                {
                    Id = y.Id,
                    SolicitudCompraCabeceraId = x.Id,
                    EstadoId = y.EstadoId,
                    CompraInternacional = y.CompraInternacional,
                    Tipo = y.Tipo,
                    ProductoCodigoArticulo = y.Producto,
                    ProductoCodigoGrupo = y.GrupoProducto,
                    Observacion = y.Observacion,
                    Cantidad = y.Cantidad,
                    Url = y.Url,

                    Valor = y.Valor,
                    Total = y.Total,

                    Saldo = y.Total - (db.RecepcionLineas.Where(zz => 
                                                            zz.SolicitudCompraDetalleId == y.Id 
                                                            && zz.EstadoId == (int)EnumEstado.ACTIVO 
                                                            && zz.Recepcion.EstadoId == (int)EnumEstado.ACTIVO
                                                        ).Count() > 0 ? db.RecepcionLineas.Where(zz => 
                                                                                            zz.SolicitudCompraDetalleId == y.Id 
                                                                                            && zz.EstadoId == (int)EnumEstado.ACTIVO
                                                                                            && zz.Recepcion.EstadoId == (int)EnumEstado.ACTIVO
                                                                                        ).Sum(zz => zz.Valor) : 0),

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
                    },

                    PlantillaDistribucionDetalle = db.SolicitudCompraDetalleDistribuciones.Where(z => z.SolicitudCompraDetalleId == y.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
                    {
                        Id = z.Id,
                        Porcentaje = z.Porcentaje,
                        EstadoId = z.EstadoId,
                        Departamento = new DepartamentoViewModel
                        {
                            Codigo = z.DepartamentoCodigo,
                            Descripcion = z.DepartamentoDescripcion,
                            CodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        },
                        DepartamentoCodigo = z.DepartamentoCodigo,
                        DepartamentoDescripcion = z.DepartamentoDescripcion,
                        DepartamentoCodigoDescripcion = z.DepartamentoCodigoDescripcion,
                        CentroCosto = new CentroCostoViewModel
                        {
                            Codigo = z.CentroCostoCodigo,
                            Descripcion = z.CentroCostoDescripcion,
                            CodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        },
                        CentroCostoCodigo = z.CentroCostoCodigo,
                        CentroCostoDescripcion = z.CentroCostoDescripcion,
                        CentroCostoCodigoDescripcion = z.CentroCostoCodigoDescripcion,
                        Proposito = new PropositoViewModel
                        {
                            Codigo = z.PropositoCodigo,
                            Descripcion = z.PropositoDescripcion,
                            CodigoDescripcion = z.PropositoCodigoDescripcion,
                        },
                        PropositoCodigo = z.PropositoCodigo,
                        PropositoDescripcion = z.PropositoDescripcion,
                        PropositoCodigoDescripcion = z.PropositoCodigoDescripcion,

                        //MetadatosCentrosCosto = new List<CentroCostoViewModel>(),
                        //MetadatosPropositos = new List<PropositoViewModel>()
                    }).ToList()
                }).ToList(),
                Tareas = db.Tareas.Where(y => y.SolicitudCompraCabeceraId == x.Id).Select(y => new TareaViewModel()
                {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    UsuarioResponsable = y.UsuarioResponsable,
                    NombreCompletoResponsable = y.NombreCompletoResponsable,
                    FechaCreacion = y.FechaCreacion,
                    FechaProcesamiento = y.FechaProcesamiento,
                    TiempoColor = y.TiempoColor,
                    SolicitudCompraCabeceraId = y.SolicitudCompraCabeceraId,
                    EstadoId = y.EstadoId,
                    Accion = y.Accion,
                    Observacion = y.Observacion,
                    NombreProveedor = (db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null),
                    NumeroRecepcion = db.Recepciones.Where(zz => zz.Id == y.RecepcionId).Select(zz => zz.NumeroRecepcion).FirstOrDefault()
                }).OrderBy(y => y.Id).ToList()
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener los originadores de una compañía.
        /// </summary>
        /// <param name="EmpresaCodigo">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<UsuarioViewModel></returns>
        public static List<UsuarioViewModel> ObtenerOriginadores(
            string EmpresaCodigo, 
            ApplicationDbContext db)
        {
            var originadores = db.SolicitudesCompraCabecera.Where(x => x.EmpresaCodigo == EmpresaCodigo).Select(x => new UsuarioViewModel() {
                NombreCompleto = x.SolicitanteNombreCompleto,
                Usuario = x.SolicitanteUsuario,
                CiudadCodigo = x.SolicitanteCiudadCodigo
            }).Distinct();

            return originadores.ToList();
        }

        /// <summary>
        /// Proceso para obtener las solicitudes filtradas.
        /// </summary>
        /// <param name="EmpresaCodigo">Filtro de compañía.</param>
        /// <param name="Originadores">Filtro de originadores.</param>
        /// <param name="FechaDesde">Filtro fecha desde.</param>
        /// <param name="FechaHasta">Filtro fecha hasta.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudCompraCabeceraViewModel> ObtenerSolicitudesSeguimientoProcesos(
            string EmpresaCodigo,
            List<string> Originadores, 
            DateTime FechaDesde, 
            DateTime FechaHasta,
            ApplicationDbContext db)
        {
            var solicitudes = db.SolicitudesCompraCabecera.Where(x => 
                    x.EmpresaCodigo == EmpresaCodigo
                    && x.FechaSolicitud >= FechaDesde
                    && x.FechaSolicitud <= FechaHasta
                    && Originadores.Contains(x.SolicitanteUsuario)
                ).Select(x => new SolicitudCompraCabeceraViewModel()
            {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    NumeroSolicitud = x.NumeroSolicitud,
                    ProveedorSugerido = x.ProveedorSugerido,
                    FechaSolicitud = x.FechaSolicitud,
                    CantTareasActivas = db.Tareas.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.SolicitudCompraCabeceraId == x.Id).Count(),
                    SolicitanteObj = new UsuarioViewModel()
                    {
                        Usuario = x.SolicitanteUsuario,
                        NombreCompleto = x.SolicitanteNombreCompleto,
                        CiudadCodigo = x.SolicitanteCiudadCodigo,
                        CiudadDescripcion = db.Ciudades.Where(y => y.Codigo == x.SolicitanteCiudadCodigo).Select(y => y.Nombre).FirstOrDefault()
                    },
                    ProductoMercadeo = new ProductoMercadeoViewModel()
                    {
                        Codigo = x.ProductoMercadeoCodigo,
                        Nombre = x.ProductoMercadeoNombre
                    }
                }).Distinct();

            return solicitudes.ToList();
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de un responsable.
        /// </summary>
        /// <param name="Usuario">Nombre del usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudCompraCabeceraViewModel> ObtenerSolicitudesPorResponsable(
            string Usuario, 
            ApplicationDbContext db)
        {
            var resultado = db.SolicitudesCompraCabecera.Where(x => x.SolicitanteUsuario == Usuario
                                                                && x.EstadoId != (int)EnumEstado.INACTIVO
                                                    ).Select(x => new SolicitudCompraCabeceraViewModel()
                                                    {
                                                        Id = x.Id,
                                                        Descripcion = x.Descripcion,
                                                        NumeroSolicitud = x.NumeroSolicitud,
                                                        ProveedorSugerido = x.ProveedorSugerido,
                                                        FechaSolicitud = x.FechaSolicitud,
                                                        CantTareasActivas = db.Tareas.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.SolicitudCompraCabeceraId == x.Id).Count()
                                                    });

            resultado = resultado.Where(x => x.CantTareasActivas > 0 || x.NumeroSolicitud == null);

            return resultado.ToList();
        }
    }
}
