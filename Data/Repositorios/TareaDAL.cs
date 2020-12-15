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
    /// Repositorio de consultas para tareas del subsistema de compras.
    /// </summary>
    public class TareaDAL
    {
        /// <summary>
        /// Proceso para salvar una tarea.
        /// </summary>
        /// <param name="tarea">Objeto que contiene los datos de la tarea a salvar.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void SalvarTarea(
            Tarea tarea, 
            ApplicationDbContext db)
        {
            db.Tareas.Add(tarea);
            db.SaveChanges();
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
            List<TareaViewModel> resultado = null;
            
            resultado = db.Tareas.Where(y => y.SolicitudCompraCabeceraId == SolicitudId).Select(y => new TareaViewModel()
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
                NombreProveedor = (db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null)
            }).OrderBy(y => y.FechaCreacion).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener las tareas pendientes del usuario autenticado.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaViewModel></returns>
        public static List<TareaViewModel> ObtenerTareas(
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            List<TareaViewModel> resultado = null;

            if(sesion != null)
            {
                resultado = db.Tareas.Where(y => y.SolicitudCompraCabecera.EstadoId == (int)EnumEstado.ACTIVO && y.UsuarioResponsable == sesion.usuario.Usuario && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new TareaViewModel()
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
                    SolicitudCompraCabeceraNumero = y.SolicitudCompraCabecera.NumeroSolicitud,
                    SolicitudCompraCabeceraDescripcion = y.SolicitudCompraCabecera.Descripcion,
                    SolicitudCompraCabeceraSolicitante = y.SolicitudCompraCabecera.SolicitanteNombreCompleto,
                    TipoTarea = y.TipoTarea,
                    NombreProveedor = (db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null)
                }).OrderBy(y => y.FechaCreacion).ToList();
            }

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

            if(sesion != null)
            {
                resultado = db.Tareas.Where(y => y.SolicitudCompraCabecera.EstadoId == (int)EnumEstado.ACTIVO && y.UsuarioResponsable == sesion.usuario.Usuario && y.EstadoId == (int)EnumEstado.ACTIVO).Count();
            }

            return resultado;
        }

        /// <summary>
        /// Proceso para actualizar el contador de iteraciones cada 10 minutos de las tareas pendientes.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        public static void ActualizarIteraciones10Minutos(ApplicationDbContext db)
        {
            List<Tarea> tareas = db.Tareas.Where(t =>
                                    t.EstadoId == (int)EnumEstado.ACTIVO
                                    && t.TipoTarea != (int)EnumTipoTarea.CREACION_SOLICITUD
                                    && t.TipoTarea != (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA
                                    && t.TipoTarea != (int)EnumTipoTarea.RECEPCION
                                    && t.TipoTarea != (int)EnumTipoTarea.ADJUNTAR_FACTURA
                                    && t.TipoTarea != (int)EnumTipoTarea.CONTABILIZAR_RECEPCION)
                                .ToList();

            if (tareas != null)
            {
                tareas.ForEach(x => x.CantIteraciones10Minutos = (x.CantIteraciones10Minutos + 1));

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<Tarea></returns>
        public static List<Tarea> ActualizarEstadoAAmarillo(ApplicationDbContext db)
        {
            List<Tarea> tareas = null;
            
            tareas = db.Tareas.Where(t =>
                t.EstadoId == (int)EnumEstado.ACTIVO
                && t.Recordatorio1 == false
                && t.TipoTarea != (int)EnumTipoTarea.CREACION_SOLICITUD
                && t.TipoTarea != (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA
                && t.TipoTarea != (int)EnumTipoTarea.RECEPCION
                && t.TipoTarea != (int)EnumTipoTarea.ADJUNTAR_FACTURA
                && t.TipoTarea != (int)EnumTipoTarea.CONTABILIZAR_RECEPCION
                && t.CantIteraciones10Minutos >= 12
                && t.CantIteraciones10Minutos < 24)
            .ToList();

            if(tareas != null)
            {
                tareas.ForEach(x => x.TiempoColor = (int)EnumTiempoColor.AMARILLO);
                tareas.ForEach(x => x.Recordatorio1 = true);

                db.SaveChanges();
            }

            return tareas;
        }

        /// <summary>
        /// Proceso par actualizar el estado de las tareas de color amarillo.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<Tarea></returns>
        public static List<Tarea> ActualizarEstadoARojo(ApplicationDbContext db)
        {
            List<Tarea> tareas = null;
            
            tareas = db.Tareas.Where(t =>
                t.EstadoId == (int)EnumEstado.ACTIVO
                && t.Recordatorio2 == false
                && t.TipoTarea != (int)EnumTipoTarea.CREACION_SOLICITUD
                && t.TipoTarea != (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA
                && t.TipoTarea != (int)EnumTipoTarea.RECEPCION
                && t.TipoTarea != (int)EnumTipoTarea.ADJUNTAR_FACTURA
                && t.TipoTarea != (int)EnumTipoTarea.CONTABILIZAR_RECEPCION
                && t.CantIteraciones10Minutos >= 24
                && t.CantIteraciones10Minutos < 36)
            .ToList();

            if(tareas != null)
            {
                tareas.ForEach(x => x.TiempoColor = (int)EnumTiempoColor.ROJO);
                tareas.ForEach(x => x.Recordatorio2 = true);

                db.SaveChanges();
            }

            return tareas;
        }

        /// <summary>
        /// Proceso para notificar a responsables previo escalamiento.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<Tarea></returns>
        public static List<Tarea> TiempoAviso(ApplicationDbContext db)
        {
            List<Tarea> tareas = null;
            
            tareas = db.Tareas.Where(t =>
                t.EstadoId == (int)EnumEstado.ACTIVO
                && t.Recordatorio3 == false
                && t.TipoTarea != (int)EnumTipoTarea.CREACION_SOLICITUD
                && t.TipoTarea != (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA
                && t.TipoTarea != (int)EnumTipoTarea.RECEPCION
                && t.TipoTarea != (int)EnumTipoTarea.ADJUNTAR_FACTURA
                && t.TipoTarea != (int)EnumTipoTarea.CONTABILIZAR_RECEPCION
                && t.CantIteraciones10Minutos >= 36
                && t.CantIteraciones10Minutos < 42)
            .ToList();

            if (tareas != null)
            {
                tareas.ForEach(x => x.Recordatorio3 = true);

                db.SaveChanges();
            }

            return tareas;
        }

        /// <summary>
        /// Proceso para obtener las tareas a escalar.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<Tarea></returns>
        public static List<Tarea> ObtenerTareasAEscalar(ApplicationDbContext db)
        {
            List<Tarea> tareas = null;
            
            tareas = db.Tareas.Where(t =>
                t.EstadoId == (int)EnumEstado.ACTIVO
                && t.TipoTarea != (int)EnumTipoTarea.CREACION_SOLICITUD
                && t.TipoTarea != (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA
                && t.TipoTarea != (int)EnumTipoTarea.RECEPCION
                && t.TipoTarea != (int)EnumTipoTarea.ADJUNTAR_FACTURA
                && t.TipoTarea != (int)EnumTipoTarea.CONTABILIZAR_RECEPCION
                && t.CantIteraciones10Minutos >= 42)
            .ToList();

            return tareas;
        }

        /// <summary>
        /// Proceso para escalar tarea.
        /// </summary>
        /// <param name="tarea">Objeto de la tarea a escalar.</param>
        /// <param name="NuevoResponsable">Nombre del nuevo usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>Tarea</returns>
        public static Tarea EscalarTarea(
            Tarea tarea, 
            UsuarioViewModel NuevoResponsable, 
            ApplicationDbContext db)
        {                
            tarea.EstadoId = (int)EnumEstado.INACTIVO;
            tarea.FechaProcesamiento = DateTime.Now;
            tarea.Accion = "Escalada";
            db.Entry(tarea).State = EntityState.Modified;

            Tarea nueva = new Tarea()
            {
                Actividad = tarea.Actividad + " (Por escalamiento)",
                UsuarioResponsable = NuevoResponsable.Usuario,
                NombreCompletoResponsable = NuevoResponsable.NombreCompleto,
                FechaCreacion = DateTime.Now,
                FechaProcesamiento = null,
                TiempoColor = (int)EnumTiempoColor.VERDE,
                SolicitudCompraCabeceraId = tarea.SolicitudCompraCabeceraId,
                EstadoId = (int)EnumEstado.ACTIVO,
                TipoTarea = tarea.TipoTarea,
                TareaPadreId = tarea.TareaPadreId,
                EmailResponsable = NuevoResponsable.Email
            };

            db.Tareas.Add(nueva);

            db.SaveChanges();

            return nueva;
        }

        /// <summary>
        /// Proceso para obtener una tarea.
        /// </summary>
        /// <param name="Id">Identificador de la tarea.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>TareaViewModel</returns>
        public static TareaViewModel ObtenerTarea(
            long Id, 
            ApplicationDbContext db)
        {
            TareaViewModel resultado = null;

            resultado = db.Tareas.Where(x => x.Id == Id).Select(x => new TareaViewModel()
            {
                Id = x.Id,
                Actividad = x.Actividad,
                UsuarioResponsable = x.UsuarioResponsable,
                NombreCompletoResponsable = x.NombreCompletoResponsable,
                FechaCreacion = x.FechaCreacion,
                FechaProcesamiento = x.FechaProcesamiento,
                TiempoColor = x.TiempoColor,
                TipoTarea = x.TipoTarea,
                SolicitudCompraCabeceraId = x.SolicitudCompraCabeceraId,
                EstadoId = x.EstadoId,
                Accion = x.Accion,
                Observacion = x.Observacion,
                Recordatorio1 = x.Recordatorio1,
                Recordatorio2 = x.Recordatorio2,
                Recordatorio3 = x.Recordatorio3,
                TiempoAviso = x.TiempoAviso,
                RetornaAJefeInmediato = x.RetornaAJefeInmediato,
                TareaPadreId = x.TareaPadreId,
                UsuarioGerenteArea = x.UsuarioGerenteArea,
                UsuarioVicepresidenteFinanciero = x.UsuarioVicepresidenteFinanciero,
                UsuarioAprobadorDesembolso = x.UsuarioAprobadorDesembolso,
                OrdenMadreId = x.OrdenMadreId,
                NumeroOrdenMadre = (x.OrdenMadre != null ? x.OrdenMadre.NumeroOrdenMadre : null),
                RecepcionId = x.RecepcionId
            }).FirstOrDefault();            

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Solicitud"></param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string ObtenerRegionDeSolicitud(
            SolicitudCompraCabecera Solicitud,
            ApplicationDbContext db)
        {
            string resultado = null;
            
            var ciudad = db.Ciudades.Where(x => x.Codigo == Solicitud.SolicitanteCiudadCodigo).FirstOrDefault();

            if (ciudad != null)
            {
                resultado = ciudad.Region.Descripcion;
            }

            return resultado;
        }

        /// <summary>
        /// Buscar las tareas de una solicitud de compra.
        /// </summary>
        /// <param name="NumeroSolicitud">Número de la solicitud.</param>
        /// <param name="Empresa">Identificador de la compañía.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaViewModel></returns>
        public static List<TareaViewModel> BuscarTareasPorSolicitud(
            long NumeroSolicitud,
            string Empresa, 
            ApplicationDbContext db)
        {
            List<TareaViewModel> resultado = null;

            resultado = db.Tareas.Where(y => y.SolicitudCompraCabecera.NumeroSolicitud == NumeroSolicitud && y.SolicitudCompraCabecera.EmpresaCodigo == Empresa).Select(y => new TareaViewModel()
            {
                Id = y.Id,
                SolicitudCompraCabeceraNumero = NumeroSolicitud,
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
                TipoTarea = y.TipoTarea,
                NombreProveedor = (db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(z => z.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null),
                NumeroRecepcion = y.Recepcion.NumeroRecepcion
            }).OrderBy(y => y.Id).ToList();

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener las tareas de un responsable.
        /// </summary>
        /// <param name="UsuarioResponsable">Nombre del usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<TareaViewModel></returns>
        public static List<TareaViewModel> ObtenerTareasPorResponsable(
            string UsuarioResponsable, 
            ApplicationDbContext db)
        {
            List<TareaViewModel> resultado = null;
            
            resultado = db.Tareas.Where(y => 
                        y.SolicitudCompraCabecera.EstadoId == (int)EnumEstado.ACTIVO 
                        && y.UsuarioResponsable == UsuarioResponsable
                        && y.EstadoId == (int)EnumEstado.ACTIVO).Select(y => new TareaViewModel()
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
                SolicitudCompraCabeceraNumero = y.SolicitudCompraCabecera.NumeroSolicitud,
                SolicitudCompraCabeceraDescripcion = y.SolicitudCompraCabecera.Descripcion,
                SolicitudCompraCabeceraSolicitante = y.SolicitudCompraCabecera.SolicitanteNombreCompleto,
                TipoTarea = y.TipoTarea,
                NombreProveedor = (db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault() != null ? db.OrdenMadreLineas.Where(x => x.OrdenMadreId == y.OrdenMadreId).FirstOrDefault().SolicitudCompraDetalle.NombreProveedor : null)
            }).OrderBy(y => y.Id).ToList();            

            return resultado;
        }

        /// <summary>
        /// Proceso para obtener una tarea.
        /// </summary>
        /// <param name="Id">Identificador de la tarea a obtener.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>Tarea</returns>
        public static Tarea FindTarea(
            long Id, 
            ApplicationDbContext db)
        {
            return db.Tareas.Find(Id);
        }
    }
}
