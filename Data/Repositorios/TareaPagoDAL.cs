using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Data.Repositorios
{
    /// <summary>
    /// Repositorio de consultas para tareas del subsistema de pagos.
    /// </summary>
    public class TareaPagoDAL
    {
        /// <summary>
        /// Proceso para salvar una tarea.
        /// </summary>
        /// <param name="tarea">Objeto que contiene los datos de la tarea a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarTarea(
            TareaPago tarea, 
            ApplicationDbContext db)
        {
            db.TareasPago.Add(tarea);
            db.SaveChanges();
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
            List<TareaPagoViewModel> resultado = null;
            
            resultado = db.TareasPago.Where(y => y.SolicitudPagoCabeceraId == SolicitudId).Select(y => new TareaPagoViewModel()
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
            }).OrderBy(y => y.FechaCreacion).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener las tareas pendientes del usuario autenticado.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaPagoViewModel></returns>
        public static List<TareaPagoViewModel> ObtenerTareas(
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            List<TareaPagoViewModel> resultado = null;

            if (sesion != null)
            {
                resultado = db.TareasPago.Where(y => y.SolicitudPagoCabecera.EstadoId == (int)EnumEstado.ACTIVO && y.UsuarioResponsable == sesion.usuario.Usuario && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new TareaPagoViewModel()
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
                    Observacion = y.Observacion,
                    SolicitudPagoCabeceraNumero = y.SolicitudPagoCabecera.NumeroSolicitud,
                    SolicitudPagoCabeceraDescripcion = y.SolicitudPagoCabecera.NombreCorto,
                    SolicitudPagoCabeceraSolicitante = y.SolicitudPagoCabecera.SolicitanteNombreCompleto,
                    TipoTarea = y.TipoTarea
                }).OrderBy(y => y.FechaCreacion).ToList();
            }

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una tarea.
        /// </summary>
        /// <param name="Id">Identificador de la tarea.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>TareaPagoViewModel</returns>
        public static TareaPagoViewModel ObtenerTarea(
            long Id, 
            ApplicationDbContext db)
        {
            TareaPagoViewModel resultado = null;

            resultado = db.TareasPago.Where(x => x.Id == Id).Select(x => new TareaPagoViewModel()
            {
                Id = x.Id,
                Actividad = x.Actividad,
                UsuarioResponsable = x.UsuarioResponsable,
                NombreCompletoResponsable = x.NombreCompletoResponsable,
                FechaCreacion = x.FechaCreacion,
                FechaProcesamiento = x.FechaProcesamiento,
                TipoTarea = x.TipoTarea,
                SolicitudPagoCabeceraId = x.SolicitudPagoCabeceraId,
                EstadoId = x.EstadoId,
                Accion = x.Accion,
                Observacion = x.Observacion,
                TareaPadreId = x.TareaPadreId,
                FacturaCabeceraPagoId = x.FacturaCabeceraPagoId,
                FacturaCabeceraPago = db.FacturaCabecerasPago.Where(y => y.Id == x.FacturaCabeceraPagoId).Select(y => new FacturaCabeceraPagoViewModel() {
                    Id = y.Id,
                    NoFactura = y.NoFactura,
                    AprobacionJefeArea = y.AprobacionJefeArea,
                    AprobacionSubgerenteArea = y.AprobacionSubgerenteArea,
                    AprobacionGerenteArea = y.AprobacionGerenteArea,
                    AprobacionVicePresidenteFinanciero = y.AprobacionVicePresidenteFinanciero,
                    AprobacionGerenteGeneral = y.AprobacionGerenteGeneral
                }).FirstOrDefault(),
                InformacionContabilidadPago = db.InformacionesContabilidadPago.Where(y => y.Id == x.Id).Select(y => new InformacionContabilidadPagoViewModel() {
                    Id = y.Id,
                    TipoDiarioCodigo = y.TipoDiarioCodigo,
                    TipoDiarioDescripcion = y.TipoDiarioDescripcion,
                    DiarioCodigo = y.DiarioCodigo,
                    DiarioDescripcion = y.DiarioDescripcion,
                    PerfilAsientoContableCodigo = y.PerfilAsientoContableCodigo,
                    PerfilAsientoContableDescripcion = y.PerfilAsientoContableDescripcion,
                    DepartamentoCodigo = y.DepartamentoCodigo,
                    DepartamentoDescripcion = y.DepartamentoDescripcion,
                    DepartamentoCodigoDescripcion = y.DepartamentoCodigoDescripcion,
                    CuentaContableCodigo = y.CuentaContableCodigo,
                    CuentaContableNombre = y.CuentaContableNombre,
                    CuentaContableTipo = y.CuentaContableTipo,
                    EsReembolso = y.EsReembolso
                }).FirstOrDefault()
            }).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener la cantidad de tareas pendientes del usuario autenticado.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>int</returns>
        public static int ObtenerCantidadTareasPendientes(
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            int resultado = 0;

            if (sesion != null)
            {
                resultado = db.TareasPago.Where(y => y.SolicitudPagoCabecera.EstadoId == (int)EnumEstado.ACTIVO && y.UsuarioResponsable == sesion.usuario.Usuario && y.EstadoId == (int)EnumEstado.ACTIVO).Count();
            }

            return resultado;
        }

        /// <summary>
        /// Buscar las tareas de una solicitud de compra.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud.</param>
        /// <param name="Empresa">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaPagoViewModel></returns>
        public static List<TareaPagoViewModel> BuscarTareasPorSolicitud(
            long NumeroSolicitud, 
            string Empresa,
            ApplicationDbContext db)
        {
            List<TareaPagoViewModel> resultado = null;

            resultado = db.TareasPago.Where(y => y.SolicitudPagoCabecera.NumeroSolicitud == NumeroSolicitud && y.SolicitudPagoCabecera.EmpresaCodigo == Empresa).Select(y => new TareaPagoViewModel()
            {
                Id = y.Id,
                SolicitudPagoCabeceraNumero = NumeroSolicitud,
                Actividad = y.Actividad,
                UsuarioResponsable = y.UsuarioResponsable,
                NombreCompletoResponsable = y.NombreCompletoResponsable,
                FechaCreacion = y.FechaCreacion,
                FechaProcesamiento = y.FechaProcesamiento,
                SolicitudPagoCabeceraId = y.SolicitudPagoCabeceraId,
                EstadoId = y.EstadoId,
                Accion = y.Accion,
                Observacion = y.Observacion,
                TipoTarea = y.TipoTarea
            }).OrderBy(y => y.Id).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener las tareas de un responsable.
        /// </summary>
        /// <param name="UsuarioResponsable">Nombre del usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaPagoViewModel></returns>
        public static List<TareaPagoViewModel> ObtenerTareasPorResponsable(
            string UsuarioResponsable,
            ApplicationDbContext db)
        {
            List<TareaPagoViewModel> resultado = null;

            resultado = db.TareasPago.Where(y =>
                        y.SolicitudPagoCabecera.EstadoId == (int)EnumEstado.ACTIVO
                        && y.UsuarioResponsable == UsuarioResponsable
                        && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new TareaPagoViewModel()
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
                            Observacion = y.Observacion,
                            SolicitudPagoCabeceraNumero = y.SolicitudPagoCabecera.NumeroSolicitud,
                            SolicitudPagoCabeceraDescripcion = y.SolicitudPagoCabecera.NombreCorto,
                            SolicitudPagoCabeceraSolicitante = y.SolicitudPagoCabecera.SolicitanteNombreCompleto,
                            TipoTarea = y.TipoTarea,
                            SolicitudPagoCabeceraObservacion = y.SolicitudPagoCabecera.Observacion
                        }).OrderBy(y => y.Id).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una tarea.
        /// </summary>
        /// <param name="Id">Identificador de la tarea a obtener.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>TareaPago</returns>
        public static TareaPago FindTarea(
            long Id,
            ApplicationDbContext db)
        {
            return db.TareasPago.Find(Id);
        }
    }
}
