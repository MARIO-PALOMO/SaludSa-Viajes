using Common.ViewModels;
using Common.Utilities;
using Data.Context;
using Data.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rest;
using Data.Entidades;
using System.Data.Entity;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de reasignación de originador en el subsistema de compras.
    /// </summary>
    public class ReasignacionOriginadorCompraBLL
    {
        /// <summary>
        /// Proceso para obtener las solicitudes de un responsable.
        /// </summary>
        /// <param name="UsuarioResponsable">Nombre del usuario responsable.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>List<SolicitudCompraCabeceraViewModel></returns>
        public static List<SolicitudCompraCabeceraViewModel> ObtenerSolicitudesPorResponsable(
            string UsuarioResponsable, 
            ApplicationDbContext db)
        {
            return SolicitudCompraCabeceraDAL.ObtenerSolicitudesPorResponsable(UsuarioResponsable, db);
        }

        /// <summary>
        /// Proceso para reasignar solicitudes.
        /// </summary>
        /// <param name="SolicitudesReasignar">Listado con los identificadores de las solicitudes a reasignar.</param>
        /// <param name="ResponsableNuevo">Nombre de usuario responsable nuevo.</param>
        /// <param name="ResponsableAnterior">Nombre de usuario responsable anterior.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void Reasignar(
            List<long> SolicitudesReasignar, 
            string ResponsableNuevo, 
            string ResponsableAnterior, 
            ApplicationDbContext db, 
            ContenedorVariablesSesion sesion)
        {
            if (SolicitudesReasignar == null || SolicitudesReasignar.Count() == 0)
            {
                throw new Exception("No se han enviado solicitudes para reasignar.");
            }

            if (ResponsableNuevo == null || ResponsableNuevo == string.Empty)
            {
                throw new Exception("No se ha enviado un nuevo originador.");
            }

            var Responsable = UsuarioRCL.ObtenerUsuario(ResponsableNuevo, sesion);

            foreach (var SolicitudReasignar in SolicitudesReasignar)
            {
                SolicitudCompraCabecera solicitud = db.SolicitudesCompraCabecera.Find(SolicitudReasignar);

                if (solicitud == null)
                {
                    throw new Exception("No se ha encontrado la solicitud con Id = " + SolicitudReasignar + ".");
                }

                if (solicitud.EstadoId == (int)EnumEstado.INACTIVO)
                {
                    throw new Exception("La solicitud con Id = " + SolicitudReasignar + " está en estado inactivo.");
                }

                if (solicitud.SolicitanteUsuario == Responsable.Usuario)
                {
                    throw new Exception("La solicitud con Id = " + SolicitudReasignar + " ya está asignada a ese responsable.");
                }

                if (solicitud.SolicitanteUsuario != ResponsableAnterior)
                {
                    throw new Exception("La solicitud con Id = " + SolicitudReasignar + " no pertenece al originador \"" + ResponsableAnterior + "\".");
                }

                solicitud.SolicitanteUsuario = Responsable.Usuario;
                solicitud.SolicitanteNombreCompleto = Responsable.NombreCompleto;
                solicitud.SolicitanteCiudadCodigo = Responsable.CiudadCodigo;

                db.Entry(solicitud).State = EntityState.Modified;
            }
        }
    }
}
