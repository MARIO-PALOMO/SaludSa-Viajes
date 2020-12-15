using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Data.Entidades;
using Data.Repositorios;
using Rest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de reasignación de tareas del subsistema de compras.
    /// </summary>
    public class ReasignacionTareasBLL
    {
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
            return TareaDAL.ObtenerTareasPorResponsable(UsuarioResponsable, db);
        }

        /// <summary>
        /// Proceso para reasignar tareas.
        /// </summary>
        /// <param name="TareasReasignar">Listado de identificadores de las tareas a reasignar.</param>
        /// <param name="ResponsableNuevo">Nombre de usuario responsable nuevo.</param>
        /// <param name="ResponsableAnterior">Nombre de usuario responsable anterior.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void Reasignar(
            List<long> TareasReasignar, 
            string ResponsableNuevo, 
            string ResponsableAnterior, 
            ApplicationDbContext db, 
            ContenedorVariablesSesion sesion)
        {
            if(TareasReasignar == null || TareasReasignar.Count() == 0)
            {
                throw new Exception("No se han enviado tareas para reasignar.");
            }

            if(ResponsableNuevo == null || ResponsableNuevo == string.Empty)
            {
                throw new Exception("No se ha enviado un nuevo responsable.");
            }

            var Responsable = UsuarioRCL.ObtenerUsuario(BuscarResponsableRecursivo(ResponsableNuevo, db, 0), sesion);

            foreach (var TareaReasignar in TareasReasignar)
            {
                Tarea tarea = TareaDAL.FindTarea(TareaReasignar, db);

                if(tarea == null)
                {
                    throw new Exception("No se ha encontrado la tarea con Id = " + TareaReasignar + ".");
                }

                if (tarea.EstadoId == (int)EnumEstado.INACTIVO)
                {
                    throw new Exception("La tarea con Id = " + TareaReasignar + " está en estado inactivo.");
                }

                if(tarea.UsuarioResponsable == Responsable.Usuario)
                {
                    throw new Exception("La tarea con Id = " + TareaReasignar + " ya está asignada a ese responsable.");
                }

                if (tarea.UsuarioResponsable != ResponsableAnterior)
                {
                    throw new Exception("La tarea con Id = " + TareaReasignar + " no pertenece al responsable \"" + ResponsableAnterior + "\".");
                }

                tarea.Accion = "Reasignada";
                tarea.Observacion = "Reasignada por " + sesion.usuario.NombreCompleto + " a " + Responsable.NombreCompleto;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.EstadoId = (long)EnumEstado.INACTIVO;

                db.Entry(tarea).State = EntityState.Modified;

                Tarea tareaNueva = new Tarea()
                {
                    Id = 0,
                    Actividad = tarea.Actividad + " (Por reasignación)",
                    UsuarioResponsable = Responsable.Usuario,
                    NombreCompletoResponsable = Responsable.NombreCompleto,
                    EmailResponsable = Responsable.Email,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = 1,
                    TipoTarea = tarea.TipoTarea,
                    Accion = null,
                    Observacion = null,
                    Recordatorio1 = false,
                    Recordatorio2 = false,
                    Recordatorio3 = false,
                    TiempoAviso = false,
                    RetornaAJefeInmediato = tarea.RetornaAJefeInmediato,
                    UsuarioGerenteArea = tarea.UsuarioGerenteArea,
                    UsuarioVicepresidenteFinanciero = tarea.UsuarioVicepresidenteFinanciero,
                    UsuarioAprobadorDesembolso = tarea.UsuarioAprobadorDesembolso,
                    SolicitudCompraCabeceraId = tarea.SolicitudCompraCabeceraId,
                    TareaPadreId = tarea.TareaPadreId,
                    OrdenMadreId = tarea.OrdenMadreId,
                    RecepcionId = tarea.RecepcionId,
                    CantIteraciones10Minutos = 0,
                    EstadoId = (long)EnumEstado.ACTIVO,
                };

                TareaDAL.SalvarTarea(tareaNueva, db);
            }
        }

        /// <summary>
        /// Proceso para buscar recursivamente un responsable.
        /// </summary>
        /// <param name="ResponsableNuevo">Nombre de usuario del responsable actual.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="iteracion">Número de iteraciones recursivas.</param>
        /// <returns>string</returns>
        public static string BuscarResponsableRecursivo(
            string ResponsableNuevo, 
            ApplicationDbContext db, 
            int iteracion)
        {
            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(ResponsableNuevo, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                iteracion++;
                return BuscarResponsableRecursivo(UsuarioReasignarTem, db, iteracion);
            }
            
            return ResponsableNuevo;
        }
    }
}
