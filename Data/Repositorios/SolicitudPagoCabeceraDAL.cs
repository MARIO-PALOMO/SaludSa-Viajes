using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para cabecera de solicitudes de pago.
    /// </summary>
    public class SolicitudPagoCabeceraDAL
    {
        /// <summary>
        /// Proceso para obtener el número máximo de solicitud.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>long?</returns>
        public static long? ObtenerNumeroSolicitudMaximo(ApplicationDbContext db)
        {
            long? resultado = 0;
            
            resultado = db.SolicitudesPagoCabecera.Max(x => x.NumeroSolicitud);

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener las solicitudes de un usuario.
        /// </summary>
        /// <param name="Usuario">Nombre de usuario.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="MostrarContabilizados">Bandera que indica si se deben incluir las solicitudes contabilizadas.</param>
        /// <returns>List<SolicitudPagoCabeceraViewModel></returns>
        public static List<SolicitudPagoCabeceraViewModel> ObtenerSolicitudes(
            string Usuario,
            ApplicationDbContext db, 
            bool MostrarContabilizados)
        {
            var resultado = db.SolicitudesPagoCabecera.Where(x => x.SolicitanteUsuario == Usuario
                                                                && x.EstadoId != (int)EnumEstado.INACTIVO
                                                    ).Select(x => new SolicitudPagoCabeceraViewModel()
                                                    {
                                                        Id = x.Id,
                                                        NombreCorto = x.NombreCorto,
                                                        NumeroSolicitud = x.NumeroSolicitud,
                                                        BeneficiarioNombre = x.BeneficiarioNombre,
                                                        FechaSolicitud = x.FechaSolicitud,
                                                        CantTareasActivas = db.TareasPago.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.SolicitudPagoCabeceraId == x.Id).Count()
                                                    });

            if (!MostrarContabilizados)
            {
                resultado = resultado.Where(x => x.CantTareasActivas > 0 || x.NumeroSolicitud == null);
            }

            return resultado.ToList();
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
            SolicitudPagoCabeceraViewModel resultado = null;

            resultado = db.SolicitudesPagoCabecera.Where(x => x.Id == Id).Select(x => new SolicitudPagoCabeceraViewModel()
            {
                Id = x.Id,
                NombreCorto = x.NombreCorto,
                NumeroSolicitud = x.NumeroSolicitud,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                Observacion = x.Observacion,
                BeneficiarioIdentificacion = x.BeneficiarioIdentificacion,
                BeneficiarioTipoIdentificacion = x.BeneficiarioTipoIdentificacion,
                BeneficiarioNombre = x.BeneficiarioNombre,
                MontoTotal = x.MontoTotal,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                JsonOriginal = x.JsonOriginal,

                Facturas = db.FacturaCabecerasPago.Where(y => y.SolicitudPagoCabeceraId == x.Id && (FacturaId == null ? true : y.Id == FacturaId)).Select(y => new FacturaCabeceraPagoViewModel()
                {
                    Id = y.Id,
                    NoFactura = y.NoFactura,
                    NoAutorizacion = y.NoAutorizacion,
                    Concepto = y.Concepto,
                    FechaEmision = y.FechaEmision,
                    FechaVencimiento = y.FechaVencimiento,
                    Total = y.Total,
                    Tipo = y.Tipo,

                    TipoPagoId = y.TipoPagoId,
                    TipoPagoObj = db.TiposPago.Where(yy => yy.Id == y.TipoPagoId).Select(yy => new TipoPagoViewModel() {
                        Id = yy.Id,
                        CuentaContableCodigo = yy.CuentaContableCodigo,
                        CuentaContableNombre = yy.CuentaContableNombre,
                        CuentaContableTipo = yy.CuentaContableTipo,
                        Referencia = yy.Referencia,
                        EsReembolso = yy.EsReembolso,
                        EstadoId = yy.EstadoId,
                        EstadoNombre = yy.Estado.Descripcion,
                        EmpresaCodigo = yy.EmpresaCodigo
                    }).FirstOrDefault(),

                    ComprobanteElectronicoId = y.ComprobanteElectronicoId,
                    FacturaElectronica = db.ComprobantesElectronicos.Where(yy => yy.Id == y.ComprobanteElectronicoId).Select(yy => new ComprobanteElectronicoViewModel() {
                        Id = yy.Id,
                        baseImponibleCero = yy.baseImponibleCero,
                        baseImponibleIva = yy.baseImponibleIva,
                        baseSinCargos = yy.baseSinCargos,
                        claveAcceso = yy.claveAcceso,
                        codigoImpuestoIva = yy.codigoImpuestoIva,
                        establecimiento = yy.establecimiento,
                        estado = yy.estado,
                        fechaAutorizacion = yy.fechaAutorizacion,
                        fechaEmisionRetencion = yy.fechaEmisionRetencion,
                        iva = yy.iva,
                        numeroAutorizacion = yy.numeroAutorizacion,
                        observaciones = yy.observaciones,
                        porcentajeRetencion = yy.porcentajeRetencion,
                        puntoEmision = yy.puntoEmision,
                        razonSocial = yy.razonSocial,
                        ruc = yy.ruc,
                        secuencial = yy.secuencial,
                        tipoDocumento = yy.tipoDocumento,
                        valorRetencion = yy.valorRetencion,
                        valorTotal = yy.valorTotal,

                        infoAdicional = db.ComprobanteElectronicoInfosAdicional.Where(zz => zz.ComprobanteElectronicoId == yy.Id).Select(zz => new ComprobanteElectronicoInfoAdicionalViewModel()
                        {
                            Id = zz.Id,
                            nombre = zz.nombre,
                            valor = zz.valor
                        }).ToList()
                    }).FirstOrDefault(),

                    FacturaDetallesPago = db.FacturaDetallesPago.Where(w => w.FacturaCabeceraPagoId == y.Id).Select(w => new FacturaDetallePagoViewModel() {
                        Id = w.Id,
                        Descripcion = w.Descripcion,
                        Valor = w.Valor,
                        Subtotal = w.Subtotal,

                        ImpuestoPagoId = w.ImpuestoPagoId,
                        ImpuestoPagoObj = db.ImpuestosPago.Where(xx => xx.Id == w.ImpuestoPagoId).Select(xx => new ImpuestoPagoViewModel() {
                            Id = xx.Id,
                            Descripcion = xx.Descripcion,
                            Porcentaje = xx.Porcentaje,
                            Compensacion = xx.Compensacion,
                            EstadoId = xx.EstadoId,
                            EstadoNombre = xx.Estado.Descripcion
                        }).FirstOrDefault(),

                        PlantillaDistribucionDetalle = db.FacturaDetallePagoDistribuciones.Where(z => z.FacturaDetallePagoId == w.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
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
                }).ToList(),
                Tareas = db.TareasPago.Where(y => y.SolicitudPagoCabeceraId == x.Id).Select(y => new TareaPagoViewModel()
                {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    UsuarioResponsable = y.UsuarioResponsable,
                    NombreCompletoResponsable = y.NombreCompletoResponsable,
                    FechaCreacion = y.FechaCreacion,
                    FechaProcesamiento = y.FechaProcesamiento,
                    SolicitudPagoCabeceraId = y.SolicitudPagoCabeceraId,
                    EstadoId = y.EstadoId,
                    Accion = y.Accion,
                    Observacion = y.Observacion
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
            var originadores = db.SolicitudesPagoCabecera.Where(x => x.EmpresaCodigo == EmpresaCodigo).Select(x => new UsuarioViewModel()
            {
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
        /// <returns>List<SolicitudPagoCabeceraViewModel></returns>
        public static List<SolicitudPagoCabeceraViewModel> ObtenerSolicitudesSeguimientoProcesos(
            string EmpresaCodigo,
            List<string> Originadores,
            DateTime FechaDesde, 
            DateTime FechaHasta, 
            ApplicationDbContext db)
        {
            var solicitudes = db.SolicitudesPagoCabecera.Where(x =>
                    x.EmpresaCodigo == EmpresaCodigo
                    && x.FechaSolicitud >= FechaDesde
                    && x.FechaSolicitud <= FechaHasta
                    && Originadores.Contains(x.SolicitanteUsuario)
                ).Select(x => new SolicitudPagoCabeceraViewModel()
                {
                    Id = x.Id,
                    NombreCorto = x.NombreCorto,
                    NumeroSolicitud = x.NumeroSolicitud,
                    FechaSolicitud = x.FechaSolicitud,
                    CantTareasActivas = db.TareasPago.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.SolicitudPagoCabeceraId == x.Id).Count(),
                    SolicitanteObj = new UsuarioViewModel()
                    {
                        Usuario = x.SolicitanteUsuario,
                        NombreCompleto = x.SolicitanteNombreCompleto,
                        CiudadCodigo = x.SolicitanteCiudadCodigo,
                        CiudadDescripcion = db.Ciudades.Where(y => y.Codigo == x.SolicitanteCiudadCodigo).Select(y => y.Nombre).FirstOrDefault()
                    },
                }).Distinct();

            return solicitudes.ToList();
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
            SolicitudPagoCabeceraViewModel resultado = null;

            resultado = db.SolicitudesPagoCabecera.Where(x => x.Id == Id).Select(x => new SolicitudPagoCabeceraViewModel()
            {
                Id = x.Id,
                NombreCorto = x.NombreCorto,
                NumeroSolicitud = x.NumeroSolicitud,
                FechaSolicitud = x.FechaSolicitud,
                EstadoId = x.EstadoId,
                SolicitanteObjUsuario = x.SolicitanteUsuario,
                AprobacionJefeAreaUsuario = x.AprobacionJefeArea,
                AprobacionSubgerenteAreaUsuario = x.AprobacionSubgerenteArea,
                AprobacionGerenteAreaUsuario = x.AprobacionGerenteArea,
                AprobacionVicePresidenteFinancieroUsuario = x.AprobacionVicePresidenteFinanciero,
                AprobacionGerenteGeneralUsuario = x.AprobacionGerenteGeneral,
                Observacion = x.Observacion,
                BeneficiarioIdentificacion = x.BeneficiarioIdentificacion,
                BeneficiarioTipoIdentificacion = x.BeneficiarioTipoIdentificacion,
                BeneficiarioNombre = x.BeneficiarioNombre,
                MontoTotal = x.MontoTotal,
                EmpresaParaLaQueSeCompra = new EmpresaViewModel()
                {
                    Codigo = x.EmpresaCodigo,
                    Nombre = x.EmpresaNombre
                },
                JsonOriginal = x.JsonOriginal,

                Facturas = db.FacturaCabecerasPago.Where(y => y.Id == FacturaId).Select(y => new FacturaCabeceraPagoViewModel()
                {
                    Id = y.Id,
                    NoFactura = y.NoFactura,
                    NoAutorizacion = y.NoAutorizacion,
                    Concepto = y.Concepto,
                    FechaEmision = y.FechaEmision,
                    FechaVencimiento = y.FechaVencimiento,
                    Total = y.Total,
                    Tipo = y.Tipo,

                    NoLiquidacion = y.NoLiquidacion,

                    TipoPagoId = y.TipoPagoId,
                    TipoPagoObj = db.TiposPago.Where(yy => yy.Id == y.TipoPagoId).Select(yy => new TipoPagoViewModel()
                    {
                        Id = yy.Id,
                        CuentaContableCodigo = yy.CuentaContableCodigo,
                        CuentaContableNombre = yy.CuentaContableNombre,
                        CuentaContableTipo = yy.CuentaContableTipo,
                        Referencia = yy.Referencia,
                        EsReembolso = yy.EsReembolso,
                        EstadoId = yy.EstadoId,
                        EstadoNombre = yy.Estado.Descripcion,
                        EmpresaCodigo = yy.EmpresaCodigo
                    }).FirstOrDefault(),

                    ComprobanteElectronicoId = y.ComprobanteElectronicoId,
                    FacturaElectronica = db.ComprobantesElectronicos.Where(yy => yy.Id == y.ComprobanteElectronicoId).Select(yy => new ComprobanteElectronicoViewModel()
                    {
                        Id = yy.Id,
                        baseImponibleCero = yy.baseImponibleCero,
                        baseImponibleIva = yy.baseImponibleIva,
                        baseSinCargos = yy.baseSinCargos,
                        claveAcceso = yy.claveAcceso,
                        codigoImpuestoIva = yy.codigoImpuestoIva,
                        establecimiento = yy.establecimiento,
                        estado = yy.estado,
                        fechaAutorizacion = yy.fechaAutorizacion,
                        fechaEmisionRetencion = yy.fechaEmisionRetencion,
                        iva = yy.iva,
                        numeroAutorizacion = yy.numeroAutorizacion,
                        observaciones = yy.observaciones,
                        porcentajeRetencion = yy.porcentajeRetencion,
                        puntoEmision = yy.puntoEmision,
                        razonSocial = yy.razonSocial,
                        ruc = yy.ruc,
                        secuencial = yy.secuencial,
                        tipoDocumento = yy.tipoDocumento,
                        valorRetencion = yy.valorRetencion,
                        valorTotal = yy.valorTotal,

                        infoAdicional = db.ComprobanteElectronicoInfosAdicional.Where(zz => zz.ComprobanteElectronicoId == yy.Id).Select(zz => new ComprobanteElectronicoInfoAdicionalViewModel()
                        {
                            Id = zz.Id,
                            nombre = zz.nombre,
                            valor = zz.valor
                        }).ToList()
                    }).FirstOrDefault(),

                    FacturaDetallesPago = db.FacturaDetallesPago.Where(w => w.FacturaCabeceraPagoId == y.Id).Select(w => new FacturaDetallePagoViewModel()
                    {
                        Id = w.Id,
                        Descripcion = w.Descripcion,
                        Valor = w.Valor,
                        Subtotal = w.Subtotal,

                        ImpuestoPagoId = w.ImpuestoPagoId,
                        ImpuestoPagoObj = db.ImpuestosPago.Where(xx => xx.Id == w.ImpuestoPagoId).Select(xx => new ImpuestoPagoViewModel()
                        {
                            Id = xx.Id,
                            Descripcion = xx.Descripcion,
                            Porcentaje = xx.Porcentaje,
                            Compensacion = xx.Compensacion,
                            EstadoId = xx.EstadoId,
                            EstadoNombre = xx.Estado.Descripcion
                        }).FirstOrDefault(),

                        PlantillaDistribucionDetalle = db.FacturaDetallePagoDistribuciones.Where(z => z.FacturaDetallePagoId == w.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
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
                        }).ToList(),

                        GrupoImpuestoCodigo = w.GrupoImpuestoCodigo,
                        GrupoImpuestoDescripcion = w.GrupoImpuestoDescripcion,

                        GrupoImpuestoArticuloCodigo = w.GrupoImpuestoArticuloCodigo,
                        GrupoImpuestoArticuloDescripcion = w.GrupoImpuestoArticuloDescripcion,
                        GrupoImpuestoArticuloCodigoDescripcion = w.GrupoImpuestoArticuloCodigoDescripcion,

                        SustentoTributarioCodigo = w.SustentoTributarioCodigo,
                        SustentoTributarioDescripcion = w.SustentoTributarioDescripcion,
                        SustentoTributarioCodigoDescripcion = w.SustentoTributarioCodigoDescripcion,

                        ImpuestoRentaGrupoImpuestoArticuloCodigo = w.ImpuestoRentaGrupoImpuestoArticuloCodigo,
                        ImpuestoRentaGrupoImpuestoArticuloDescripcion = w.ImpuestoRentaGrupoImpuestoArticuloDescripcion,
                        ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion = w.ImpuestoRentaGrupoImpuestoArticuloCodigoDescripcion,

                        IvaGrupoImpuestoArticuloCodigo = w.IvaGrupoImpuestoArticuloCodigo,
                        IvaGrupoImpuestoArticuloDescripcion = w.IvaGrupoImpuestoArticuloDescripcion,
                        IvaGrupoImpuestoArticuloCodigoDescripcion = w.IvaGrupoImpuestoArticuloCodigoDescripcion
                    }).ToList(),
                }).ToList(),
                Tareas = db.TareasPago.Where(y => y.SolicitudPagoCabeceraId == x.Id).Select(y => new TareaPagoViewModel()
                {
                    Id = y.Id,
                    Actividad = y.Actividad,
                    UsuarioResponsable = y.UsuarioResponsable,
                    NombreCompletoResponsable = y.NombreCompletoResponsable,
                    FechaCreacion = y.FechaCreacion,
                    FechaProcesamiento = y.FechaProcesamiento,
                    SolicitudPagoCabeceraId = y.SolicitudPagoCabeceraId,
                    EstadoId = y.EstadoId,
                    Accion = y.Accion,
                    Observacion = y.Observacion
                }).OrderBy(y => y.Id).ToList()
            }).FirstOrDefault();

            return resultado;
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
            var count = 0;

            if(SolicitudId == null)
            {
                count = db.FacturaCabecerasPago.Where(x =>
                            x.NoFactura == NoFactura
                            && x.SolicitudPagoCabecera.BeneficiarioIdentificacion == RUC
                            && x.TipoPago.EsReembolso == false).Count();
            }
            else
            {
                count = db.FacturaCabecerasPago.Where(x =>
                            x.NoFactura == NoFactura
                            && x.SolicitudPagoCabecera.BeneficiarioIdentificacion == RUC
                            && x.TipoPago.EsReembolso == false
                            && x.SolicitudPagoCabeceraId != SolicitudId).Count();
            }

            return count > 0;
        }
    }
}
