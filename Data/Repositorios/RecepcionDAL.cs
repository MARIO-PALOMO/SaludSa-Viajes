using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para recepciones.
    /// </summary>
    public class RecepcionDAL
    {
        /// <summary>
        /// Proceso para obtener el mayor número de recepción de una orden madre.
        /// </summary>
        /// <param name="OrdenMadreId">Identificador de orden madre.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>int?</returns>
        public static int? ObtenerNumeroRecepcionMaximo(
            long OrdenMadreId, 
            ApplicationDbContext db)
        {
            int? resultado = null;
            List<Recepcion> recepciones = new List<Recepcion>();

            var OrdenesMadre = db.OrdenesMadre.Find(OrdenMadreId).SolicitudCompraCabecera.OrdenesMadre;

            foreach(var orden in OrdenesMadre)
            {
                foreach (var recep in orden.Recepciones)
                {
                    recepciones.Add(recep);
                }
            }

            if(recepciones.Count() > 0)
            {
                resultado = recepciones.Max(x => x.NumeroRecepcion);
            }

            //var tem = db.Recepciones.Where(x => x.OrdenMadreId == OrdenMadreId).ToList();

            //if(tem != null && tem.Count() > 0)
            //{
            //    resultado = tem.Max(x => x.NumeroRecepcion);
            //}

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de recepción.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>RecepcionViewModel</returns>
        public static RecepcionViewModel ObtenerRecepcion(
            long RecepcionId, 
            ApplicationDbContext db)
        {
            RecepcionViewModel resultado = null;
            
            resultado = db.Recepciones.Where(x => x.Id == RecepcionId).Select(x => new RecepcionViewModel()
            {
                Id = x.Id,
                NumeroRecepcion = x.NumeroRecepcion,
                FechaRecepcion = x.FechaRecepcion,
                Aprobada = x.Aprobada,
                OrdenMadreId = x.OrdenMadreId,
                EstadoId = x.EstadoId,
                NumeroOrdenMadre = x.OrdenMadre.NumeroOrdenMadre,
                NumeroSolicitud = x.OrdenMadre.SolicitudCompraCabecera.NumeroSolicitud,
                EmpresaParaLaQueSeCompra = x.OrdenMadre.SolicitudCompraCabecera.EmpresaCodigo,

                RecepcionLineas = db.RecepcionLineas.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.RecepcionId == x.Id).Select(y => new RecepcionLineaViewModel()
                {
                    Id = y.Id,
                    Cantidad = y.Cantidad,
                    Valor = y.Valor,
                    RecepcionId = y.RecepcionId,
                    SolicitudCompraDetalleId = y.SolicitudCompraDetalleId,
                    EstadoId = y.EstadoId,

                    PlantillaDistribucionDetalle = db.SolicitudCompraDetalleDistribuciones.Where(z => z.RecepcionLineaId == y.Id && z.EstadoId == (int)EnumEstado.ACTIVO).Select(z => new PlantillasDistribucionDetalleViewModel()
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

                ComprobantesElectronicos = db.ComprobantesElectronicos.Where(y => y.RecepcionId == x.Id && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new ComprobanteElectronicoViewModel() {
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

                    infoAdicional = db.ComprobanteElectronicoInfosAdicional.Where(z => z.ComprobanteElectronicoId == y.Id).Select(z => new ComprobanteElectronicoInfoAdicionalViewModel() {
                        Id = z.Id,
                        nombre = z.nombre,
                        valor = z.valor
                    }).ToList()
                }).ToList()

            }).FirstOrDefault();

            return resultado;
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
            List<RecepcionViewModel> resultado = null;
            
            var tarea = db.Tareas.Find(TareaId);

            resultado = db.Recepciones.Where(x => x.EstadoId == (int)EnumEstado.ACTIVO && x.OrdenMadreId == tarea.OrdenMadreId).Select(x => new RecepcionViewModel()
            {
                Id = x.Id,
                NumeroRecepcion = x.NumeroRecepcion,
                FechaRecepcion = x.FechaRecepcion,
                Aprobada = x.Aprobada,
                OrdenMadreId = x.OrdenMadreId,
                EstadoId = x.EstadoId,

                NumeroOrdenHija = x.OrdenHija.NumeroOrdenHija,

                RecepcionLineas = db.RecepcionLineas.Where(y => y.EstadoId == (int)EnumEstado.ACTIVO && y.RecepcionId == x.Id).Select(y => new RecepcionLineaViewModel()
                {
                    Id = y.Id,
                    Cantidad = y.Cantidad,
                    Valor = y.Valor,
                    RecepcionId = y.RecepcionId,
                    SolicitudCompraDetalleId = y.SolicitudCompraDetalleId,
                    EstadoId = y.EstadoId,

                    ProductoNombre = y.SolicitudCompraDetalle.ProductoNombre,
                    PorcentajeImpuestoVigente = y.SolicitudCompraDetalle.PorcentajeImpuestoVigente,
                    Saldo = y.Saldo,
                    ValorUnitario = y.SolicitudCompraDetalle.Valor
                }).OrderBy(y => y.Id).ToList()

            }).OrderBy(x => x.Id).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para buscar recepciones.
        /// </summary>
        /// <param name="NumeroSolicitud">Filtro de número de solicitud.</param>
        /// <param name="Cabecera">Objeto que contiene los datos de la cabecera de la solicitud de compra.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>List<RecepcionViewModel></returns>
        public static List<RecepcionViewModel> BuscarRecepciones(
            long NumeroSolicitud, 
            SolicitudCompraCabecera Cabecera,
            ContenedorVariablesSesion sesion)
        {
            List<RecepcionViewModel> resultado = new List<RecepcionViewModel>();

            foreach(var orden in Cabecera.OrdenesMadre)
            {
                foreach (var recepcion in orden.Recepciones)
                {
                    if(recepcion.EstadoId == (int)EnumEstado.ACTIVO 
                       && recepcion.Contabilizada == false
                       && recepcion.Aprobada == true
                       && recepcion.EsperandoAutorizacionAnulacion == false)
                    {
                        if(recepcion.UsuarioCreador == sesion.usuario.Usuario || sesion.RolesAdministrativos.Contains((long)EnumRol.ADMIN_COMPRA_ANULACION_RECEPCION))
                        {
                            resultado.Add(new RecepcionViewModel()
                            {
                                Id = recepcion.Id,
                                NumeroRecepcion = recepcion.NumeroRecepcion,
                                OrdenMadreId = recepcion.OrdenMadreId,
                                NumeroOrdenMadre = recepcion.OrdenMadre.NumeroOrdenMadre
                            });
                        }
                    }
                }
            }                

            return resultado.OrderBy(x => x.NumeroRecepcion).ToList();
        }
    }
}
