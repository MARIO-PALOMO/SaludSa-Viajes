using Common.Utilities;
using Common.ViewModels;
using CrystalDecisions.CrystalReports.Engine;
using Data.Context;
using Data.Entidades;
using Data.Repositorios;
using Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// Gestiona la lógica de negocio de tareas del subsistema de compras.
    /// </summary>
    public class TareaBLL
    {
        /// <summary>
        /// Proceso para crear tareas.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud a la que pertenece la tarea.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="tipo">Tipo de tarea a crear.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="GerenteArea">Nombre de usuario de gerente de área.</param>
        public static void CrearTarea(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            int tipo, 
            ApplicationDbContext db, 
            long TareaPadreId, 
            string GerenteArea = null)
        {
            switch(tipo)
            {
                case (int)EnumTipoTarea.CREACION_SOLICITUD:
                    CrearCreacionSolicitud(Solicitud, sesion, db);
                    break;
                case (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO:
                    CrearAprobacionJefeInmediato(Solicitud, sesion, db, string.Empty, 0);
                    break;
                case (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA:
                    CrearAprobacionGestorCompra(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO:
                    CrearAprobacionJefeCompraProductoOtro(Solicitud, sesion, db, string.Empty, 0);
                    break;
                case (int)EnumTipoTarea.APROBACION_JEFE_COMPRA:
                    CrearAprobacionJefeCompra(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE:
                    CrearDevolverSolicitante(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.DEVOLVER_A_GESTOR_COMPRA:
                    CrearAprobacionGestorCompra(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO:
                    CrearVerificacionGestorPresupuesto(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO:
                    CrearRetornoAJefeInmediato(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA:
                    CrearAprobacionPorMontoJefeArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    CrearAprobacionPorMontoSubgerenteArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA:
                    CrearAprobacionPorMontoGerenteArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    CrearAprobacionPorMontoVicepresidenteFinanciero(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                //case (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL:
                //    CrearAprobacionPorMontoGerenteGeneral(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                //    break;
                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA:
                    CrearAprobacionFueraPresupuestoGerenteArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId, GerenteArea);
                    break;
                case (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO:
                    CrearAprobacionFueraPresupuestoVicepresidenteFinanciero(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                //case (int)EnumTipoTarea.RECEPCION:
                //    CrearRecepcion(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                //    break;
                case (int)EnumTipoTarea.APROBACION_DESEMBOLSO:
                    CrearAprobacionDesembolso(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.ADJUNTAR_FACTURA:
                    CrearAdjuntarFactura(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTarea.CONTABILIZAR_RECEPCION:
                    CrearContabilizarRecepcion(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
            }
        }

        /// <summary>
        /// Proceso para crear la tarea inicial de creación de una solicitud.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        private static void CrearCreacionSolicitud(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            var UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

            Tarea tarea = new Tarea()
            {
                Actividad = "Registro de solicitud de compra",
                UsuarioResponsable = Solicitud.SolicitanteUsuario,
                NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                FechaCreacion = DateTime.Now,
                FechaProcesamiento = DateTime.Now,
                TiempoColor = (int)EnumTiempoColor.VERDE,
                SolicitudCompraCabeceraId = Solicitud.Id,
                EstadoId = (int)EnumEstado.INACTIVO,
                TipoTarea = (int)EnumTipoTarea.CREACION_SOLICITUD,
                Accion = "Registro",
                EmailResponsable = UsuarioObj.Email
            };

            TareaDAL.SalvarTarea(tarea, db);
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de jefe inmediato.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        private static void CrearAprobacionJefeInmediato(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe inmediato (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionJefeArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe inmediato",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_INMEDIATO,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionJefeInmediato(Solicitud, sesion, db, UsuarioReasignarTem, iteracion);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionJefeInmediato;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación de jefe inmediato", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de gestor de compra.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionGestorCompra(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            string UsuarioGestorCompra = string.Empty;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Cotización y selección de proveedores (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioGestorCompra = RolGestorCompraDAL.ObtenerGestorDeUnaCompra(Solicitud, db);

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioGestorCompra, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Cotización y selección de proveedores",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_GESTOR_COMPRA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionGestorCompra(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGestorCompra;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Cotización y selección de proveedores", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de devolución a solicitante.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearDevolverSolicitante(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Solicitud devuelta para edición (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Solicitud devuelta para edición",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.DEVOLVER_A_SOLICITANTE,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearDevolverSolicitante(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

                    string body = EmailTemplatesResource.EmailDevolucionSolicitante;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{RESPONSABLE_ANTERIOR}", TareaPadre.NombreCompletoResponsable);
                    body = body.Replace("{COMENTARIO}", TareaPadre.Observacion);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Devolución a solicitante", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de jefe de compra.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionJefeCompra(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe de compra (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_COMPRA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                string[] grupo = new string[] { "GR Jefe de Compras" };

                UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioObj.Usuario, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe de compra",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_COMPRA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionJefeCompra(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

                    string body = EmailTemplatesResource.EmailAprobacionJefeCompra;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{GESTOR_COMPRA}", TareaPadre.NombreCompletoResponsable);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación de jefe de compra", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de jefe de compra para selección de producto "Otro".
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        private static void CrearAprobacionJefeCompraProductoOtro(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Corrección de producto \"Otro\" (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                string[] grupo = new string[] { "GR Jefe de Compras" };

                UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioObj.Usuario, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Corrección de producto \"Otro\"",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_JEFE_COMPRA_PRODUCTO_OTRO,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionJefeCompraProductoOtro(Solicitud, sesion, db, UsuarioReasignarTem, iteracion);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailCorreccionJefeCompraProductoOtro;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Corrección de producto \"Otro\"", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de gestor de presupuesto.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearVerificacionGestorPresupuesto(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Verificación de presupuesto (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                //string[] grupo = new string[] { "GR_Validador_Presupuesto" };
                //UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();

                string UsuarioVerificacionPresupuesto = RolAdministrativoDAL.ObtenerUsuarioVerificacionPresupuesto(Solicitud, db);

                if(UsuarioVerificacionPresupuesto == null || UsuarioVerificacionPresupuesto == string.Empty)
                {
                    throw new Exception("No se ha configurado un verificador de presupuesto para la empresa \"" + Solicitud.EmpresaNombre + "\" y la ciudad \"" + Solicitud.SolicitanteCiudadCodigo + "\".");
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioVerificacionPresupuesto, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Verificación de presupuesto",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.VERIFICACION_PRESUPUESTO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearVerificacionGestorPresupuesto(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailVerificarPresupuesto;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Verificación de presupuesto", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de retorno a jefe inmediato.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearRetornoAJefeInmediato(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe inmediato (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionJefeArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación del jefe inmediato",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.RETORNO_A_JEFE_INMEDIATO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearRetornoAJefeInmediato(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionJefeInmediatoMontoMayor;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto mayor al estimado", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por monto de jefe de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoJefeArea(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Jefe de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionJefeArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Jefe de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_JEFE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoJefeArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionJefeArea;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto Jefe de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por monto de subgerente de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoSubgerenteArea(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Subgerente de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionSubgerenteArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Subgerente de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_SUBGERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoSubgerenteArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionSubgerenteArea;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto Subgerente de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por monto de gerente de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoGerenteArea(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Gerente de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionGerenteArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Gerente de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoGerenteArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGerenteArea;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto Gerente de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por monto de vicepresidente financiero.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoVicepresidenteFinanciero(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Vicepresidente Financiero (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionVicePresidenteFinanciero, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Vicepresidente Financiero",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoVicepresidenteFinanciero(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionVicepresidenteFinanciero;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto Vicepresidente Financiero", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por monto de gerente general.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="path">Url base de la aplicación.</param>
        public static void CrearAprobacionPorMontoGerenteGeneral(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId, 
            string path)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Gerente General (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionGerenteGeneral, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación por monto Gerente General",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_POR_MONTO_GERENTE_GENERAL,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoGerenteGeneral(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId, path);
            }
            else
            {
                var enviarCorreo = true;
                var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

                if (TareaPadre.UsuarioResponsable == tarea.UsuarioResponsable)
                {
                    tarea.Accion = "Aprobar";
                    tarea.Observacion = "Aprobada de forma automática.";
                    tarea.EstadoId = (int)EnumEstado.INACTIVO;
                    tarea.FechaProcesamiento = DateTime.Now;

                    enviarCorreo = false;
                }

                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty && enviarCorreo)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGerenteGeneral;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación por monto Gerente General", destinatarios, null, tarea.Id, db);
                }
                else if(enviarCorreo == false)
                {
                    TareaBLL.GenerarOrdenesMadre(Solicitud, sesion, db, path, tarea.Id);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de fuera de presupuesto por gerente de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="GerenteArea"></param>
        private static void CrearAprobacionFueraPresupuestoGerenteArea(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId, 
            string GerenteArea)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación fuera de presupuesto Gerente de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(GerenteArea, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación fuera de presupuesto Gerente de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionFueraPresupuestoGerenteArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId, GerenteArea);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGerenteAreaFueraPresupuesto;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación fuera de presupuesto Gerente de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación de fuera de presupuesto por vicepresidente financiero.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionFueraPresupuestoVicepresidenteFinanciero(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación fuera de presupuesto Vicepresidente Financiero (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);
                var TareaAbuela = TareaDAL.ObtenerTarea((long)TareaPadre.TareaPadreId, db);

                UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaAbuela.UsuarioVicepresidenteFinanciero, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación fuera de presupuesto Vicepresidente Financiero",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_FUERA_PRESUPUESTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionFueraPresupuestoVicepresidenteFinanciero(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionVicepresidenteFinancieroFueraPresupuesto;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación fuera de presupuesto Vicepresidente Financiero", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de crear recepción.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="OrdenMadreId">Identificador de la orden madre.</param>
        public static void CrearRecepcion(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId, 
            long OrdenMadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Recepción de Bienes/Servicios (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.RECEPCION,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = OrdenMadreId
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Recepción de Bienes/Servicios",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.RECEPCION,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = OrdenMadreId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearRecepcion(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId, OrdenMadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailRecepcionBienServicio;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Recepción de Bienes/Servicios", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobar desembolso.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionDesembolso(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            string UsuarioAprobadorDesembolso = string.Empty;

            var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación de desembolso (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_DESEMBOLSO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }
            else
            {
                UsuarioAprobadorDesembolso = TareaPadre.UsuarioAprobadorDesembolso;

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioAprobadorDesembolso, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobación de desembolso",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_DESEMBOLSO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionDesembolso(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionRecepcion;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobación de desembolso", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de adjuntar factura.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAdjuntarFactura(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion,
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Adjuntar Factura (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.ADJUNTAR_FACTURA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }
            else
            {
                /*var region = TareaDAL.ObtenerRegionDeSolicitud(Solicitud, db);

                string[] grupo = null;

                if(region == "Costa")
                {
                    grupo = new string[] { "GR_Adjuntar_Facturas_Costa" };
                }
                else
                {
                    grupo = new string[] { "GR_Adjuntar_Facturas_Sierra" };
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();*/

                string UsuarioAdjuntarFactura = RolAdministrativoDAL.ObtenerUsuarioAdjuntarFactura(Solicitud, db);

                if (UsuarioAdjuntarFactura == null || UsuarioAdjuntarFactura == string.Empty)
                {
                    throw new Exception("No se ha configurado un usuario para adjuntar factura en la empresa \"" + Solicitud.EmpresaNombre + "\" y la ciudad \"" + Solicitud.SolicitanteCiudadCodigo + "\".");
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioAdjuntarFactura, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Adjuntar Factura",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.ADJUNTAR_FACTURA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAdjuntarFactura(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAdjuntarFacturaRecepcion;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Adjuntar Factura", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de contabilizar recepción.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearContabilizarRecepcion(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            var TareaPadre = TareaDAL.ObtenerTarea(TareaPadreId, db);

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Contabilizar recepción (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.CONTABILIZAR_RECEPCION,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }
            else
            {
                /*var region = TareaDAL.ObtenerRegionDeSolicitud(Solicitud, db);

                string[] grupo = null;

                if (region == "Costa")
                {
                    grupo = new string[] { "GR_Registro Facturas_Costa" };
                }
                else
                {
                    grupo = new string[] { "GR_Registro_Facturas_Sierra" };
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();*/

                string UsuarioContabilizarRecepcion = RolAdministrativoDAL.ObtenerUsuarioContabilizarRecepcion(Solicitud, db);

                if (UsuarioContabilizarRecepcion == null || UsuarioContabilizarRecepcion == string.Empty)
                {
                    throw new Exception("No se ha configurado un usuario para contabilizar recepciones en la empresa \"" + Solicitud.EmpresaNombre + "\" y la ciudad \"" + Solicitud.SolicitanteCiudadCodigo + "\".");
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioContabilizarRecepcion, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Contabilizar recepción",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.CONTABILIZAR_RECEPCION,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = TareaPadre.OrdenMadreId,
                    RecepcionId = TareaPadre.RecepcionId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearContabilizarRecepcion(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailContabilizarRecepcion;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Contabilizar recepción", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de anulación de recepción.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="Recepcion">Objeto que contiene la recepción.</param>
        /// <param name="Motivo">Motivo de anulación.</param>
        /// <returns>Tarea</returns>
        public static Tarea CrearAnulacionRecepcion(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            Recepcion Recepcion, 
            string Motivo)
        {
            var UsuarioObj = UsuarioRCL.ObtenerUsuario(sesion.usuario.Usuario, sesion);

            Tarea tarea = new Tarea()
            {
                Actividad = "Anulación de recepción",
                UsuarioResponsable = UsuarioObj.Usuario,
                NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                FechaCreacion = DateTime.Now,
                FechaProcesamiento = DateTime.Now,
                TiempoColor = (int)EnumTiempoColor.VERDE,
                SolicitudCompraCabeceraId = Solicitud.Id,
                EstadoId = (int)EnumEstado.INACTIVO,
                TipoTarea = (int)EnumTipoTarea.ANULACION_RECEPCION,
                Accion = "Solicitud anulación",
                Observacion = Motivo,
                EmailResponsable = UsuarioObj.Email,
                OrdenMadreId = Recepcion.OrdenMadreId,
                RecepcionId = Recepcion.Id
            };

            TareaDAL.SalvarTarea(tarea, db);

            return tarea;
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobar anulación de recepción.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="Recepcion">Objeto que contiene la recepción.</param>
        /// <param name="Motivo">Motivo de anulación.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        public static void CrearAprobarAnulacionRecepcion(
            SolicitudCompraCabecera Solicitud,
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion,
            Recepcion Recepcion, 
            string Motivo, 
            long TareaPadreId)
        {
            Tarea tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobar anulación de recepción (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_ANULACION_RECEPCION,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = Recepcion.OrdenMadreId,
                    RecepcionId = Recepcion.Id,
                    TareaPadreId = TareaPadreId
                };
            }
            else
            {
                string[] grupo = new string[] { "GR Jefe de Compras" };

                UsuarioObj = UsuarioRCL.ObtenerUsuariosPorGrupo(grupo, sesion).FirstOrDefault();
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioObj.Usuario, sesion);

                tarea = new Tarea()
                {
                    Actividad = "Aprobar anulación de recepción",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    TiempoColor = (int)EnumTiempoColor.VERDE,
                    SolicitudCompraCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTarea.APROBACION_ANULACION_RECEPCION,
                    EmailResponsable = UsuarioObj.Email,
                    OrdenMadreId = Recepcion.OrdenMadreId,
                    RecepcionId = Recepcion.Id,
                    TareaPadreId = TareaPadreId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobarAnulacionRecepcion(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, Recepcion, Motivo, TareaPadreId);
            }
            else
            {
                TareaDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionAnularRecepcion;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{RECEPCION}", Solicitud.NumeroSolicitud + "-" + Recepcion.NumeroRecepcion);
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{ANULADOR}", sesion.usuario.NombreCompleto);
                    body = body.Replace("{MOTIVO}", Motivo);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.Enviar(sesion, body, "Aprobar anulación de recepción", destinatarios, null, tarea.Id, db);
                }
            }
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
            return TareaDAL.ObtenerTareas(sesion, db);
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
            return TareaDAL.ObtenerCantidadTareasPendientes(sesion, db);
        }

        /// <summary>
        /// Proceso para actualizar el contador de iteraciones cada 10 minutos de las tareas pendientes.
        /// </summary>
        /// <param name="db">Contexto de base de datos.</param>
        public static void ActualizarIteraciones10Minutos(ApplicationDbContext db)
        {
            TareaDAL.ActualizarIteraciones10Minutos(db);
        }

        /// <summary>
        /// Proceso par actualizar el estado de las tareas de color amarillo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void ActualizarEstadoAAmarillo(
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            var tareas = TareaDAL.ActualizarEstadoAAmarillo(db);

            foreach(var tarea in tareas)
            {
                try
                {
                    if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                    {
                        string body = EmailTemplatesResource.EmailRecordatorioEscalamiento;

                        body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                        body = body.Replace("{TAREA}", "\"" + tarea.Actividad + "\"");
                        body = body.Replace("{SOLICITUD}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                        body = body.Replace("{ESTADO}", "\"Amarillo\"");
                        body = body.Replace("{HORAS}", "2");
                        body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                        List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                        {
                            new EmailDestinatarioViewModel()
                            {
                                Nombre = tarea.NombreCompletoResponsable,
                                Direccion = tarea.EmailResponsable
                            }
                        };

                        EmailBLL.Enviar(sesion, body, "Recordatorio de tarea pendiente (Estado Amarillo)", destinatarios, null, tarea.Id, db);
                    }
                }
                catch(Exception)
                {

                }
            }
        }

        /// <summary>
        /// Proceso par actualizar el estado de las tareas de color rojo.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void ActualizarEstadoARojo(
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            var tareas = TareaDAL.ActualizarEstadoARojo(db);

            foreach (var tarea in tareas)
            {
                try
                {
                    if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                    {
                        string body = EmailTemplatesResource.EmailRecordatorioEscalamiento;

                        body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                        body = body.Replace("{TAREA}", "\"" + tarea.Actividad + "\"");
                        body = body.Replace("{SOLICITUD}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                        body = body.Replace("{ESTADO}", "\"Rojo\"");
                        body = body.Replace("{HORAS}", "4");
                        body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                        List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                        {
                            new EmailDestinatarioViewModel()
                            {
                                Nombre = tarea.NombreCompletoResponsable,
                                Direccion = tarea.EmailResponsable
                            }
                        };

                        EmailBLL.Enviar(sesion, body, "Recordatorio de tarea pendiente (Estado Rojo)", destinatarios, null, tarea.Id, db);
                    }
                }
                catch(Exception)
                {

                }
            }
        }

        /// <summary>
        /// Proceso para notificar a responsables previo escalamiento.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void NotificarAntesDeEscalar(
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            var tareas = TareaDAL.TiempoAviso(db);

            foreach (var tarea in tareas)
            {
                try
                {
                    if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                    {
                        string body = EmailTemplatesResource.EmailPrevioEscalamiento;

                        body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                        body = body.Replace("{TAREA}", "\"" + tarea.Actividad + "\"");
                        body = body.Replace("{SOLICITUD}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                        body = body.Replace("{HORAS}", "6");
                        body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudCompraCabeceraId);

                        List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                        {
                            new EmailDestinatarioViewModel()
                            {
                                Nombre = tarea.NombreCompletoResponsable,
                                Direccion = tarea.EmailResponsable
                            }
                        };

                        EmailBLL.Enviar(sesion, body, "Notificación previa a escalamiento", destinatarios, null, tarea.Id, db);
                    }
                }
                catch(Exception)
                {

                }                
            }
        }

        /// <summary>
        /// Proceso para escalar tareas.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        public static void EscalarTareas(
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            var tareas = TareaDAL.ObtenerTareasAEscalar(db);

            if(tareas != null)
            {
                foreach (var tarea in tareas)
                {
                    var ResponsableActual = UsuarioRCL.ObtenerUsuario(tarea.UsuarioResponsable, sesion);

                    if(ResponsableActual != null && ResponsableActual.JefeInmediato != null && ResponsableActual.JefeInmediato.Usuario != null)
                    {
                        var NuevoResponsable = UsuarioRCL.ObtenerUsuario(ResponsableActual.JefeInmediato.Usuario, sesion);

                        if(NuevoResponsable != null)
                        {
                            var NuevaTarea = TareaDAL.EscalarTarea(tarea, NuevoResponsable, db);

                            if(NuevaTarea != null)
                            {
                                // NOTIFICACIÓN AL RESPONSABLE ANTERIOR
                                if(ResponsableActual.Email != null && ResponsableActual.Email != string.Empty)
                                {
                                    string body = EmailTemplatesResource.EmailEscalamientoAnteriorResponsable;

                                    body = body.Replace("{RESPONSABLE_ANTERIOR}", ResponsableActual.NombreCompleto);
                                    body = body.Replace("{TAREA}", "\"" + tarea.Actividad + "\"");
                                    body = body.Replace("{SOLICITUD}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                                    body = body.Replace("{HORAS}", "7");

                                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                                    {
                                        new EmailDestinatarioViewModel()
                                        {
                                            Nombre = ResponsableActual.NombreCompleto,
                                            Direccion = ResponsableActual.Email
                                        }
                                    };

                                    EmailBLL.Enviar(sesion, body, "Notificación de escalamiento realizado", destinatarios, null, tarea.Id, db);
                                }

                                // NOTIFICACIÓN AL NUEVO RESPONSABLE
                                if(NuevoResponsable.Email != null && NuevoResponsable.Email != string.Empty)
                                {
                                    string body = EmailTemplatesResource.EmailEscalamientoNuevoResponsable;

                                    body = body.Replace("{RESPONSABLE_ANTERIOR}", ResponsableActual.NombreCompleto);
                                    body = body.Replace("{RESPONSABLE}", "\"" + NuevoResponsable.NombreCompleto + "\"");
                                    body = body.Replace("{TAREA}", "\"" + tarea.Actividad + "\"");
                                    body = body.Replace("{SOLICITUD}", tarea.SolicitudCompraCabecera.NumeroSolicitud.ToString());
                                    body = body.Replace("{HORAS}", "7");
                                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + NuevaTarea.TipoTarea + "&tareaId=" + NuevaTarea.Id + "&solicitudId=" + NuevaTarea.SolicitudCompraCabeceraId);

                                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                                    {
                                        new EmailDestinatarioViewModel()
                                        {
                                            Nombre = NuevoResponsable.NombreCompleto,
                                            Direccion = NuevoResponsable.Email
                                        }
                                    };

                                    EmailBLL.Enviar(sesion, body, "Notificación de tarea asignada por escalamiento", destinatarios, null, tarea.Id, db);
                                }
                            }
                        }
                    }
                }
            }
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
            return TareaDAL.ObtenerTarea(Id, db);
        }

        /// <summary>
        /// Proceso para generar ordenes madre.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="path">Url base de la aplicación.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        public static void GenerarOrdenesMadre(
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string path, 
            long TareaPadreId)
        {
            List<OrdenMadreViewModel> OrdenesMadre = new List<OrdenMadreViewModel>();

            List<SolicitudCompraDetalleViewModel> Detalles = SolicitudCompraDetalleDAL.ObtenerDetalles(Solicitud.Id, db);

            for (int i = 0; i < Detalles.Count(); i++)
            {
                SolicitudCompraDetalleViewModel DetalleI = Detalles.ElementAt(i);

                if (!DetalleI.ProcesadoEnOrdenMadre)
                {
                    OrdenMadreViewModel OrdenMadre = new OrdenMadreViewModel()
                    {
                        Ruc = DetalleI.Proveedor.Identificacion,
                        NumeroOrdenMadre = "",
                        LineasOrdenMadre = new List<OrdenMadreLineaViewModel>()
                    };

                    OrdenMadre.LineasOrdenMadre.Add(new OrdenMadreLineaViewModel()
                    {
                        Tipo = DetalleI.Tipo == "0" ? "BIEN" : "SERVICIO",
                        Secuencial = DetalleI.Id,
                        CodigoLinea = "",
                        Observacion = DetalleI.ProductoNombre + " | " + DetalleI.Observacion,
                        CodigoArticulo = DetalleI.ProductoCodigoArticulo,
                        Valor = DetalleI.Valor + (DetalleI.Valor * (decimal)DetalleI.Impuesto.Porcentaje / 100),
                        Cantidad = DetalleI.Tipo == "0" ? ((int)DetalleI.Cantidad).ToString() : DetalleI.Cantidad.ToString().Replace(',', '.'),
                        Iva = DetalleI.Impuesto.Codigo == "0" ? (double)0 : (double)1
                    }) ;

                    DetalleI.ProcesadoEnOrdenMadre = true;

                    for (int j = i + 1; j < Detalles.Count(); j++)
                    {
                        SolicitudCompraDetalleViewModel DetalleJ = Detalles.ElementAt(j);

                        if (!DetalleJ.ProcesadoEnOrdenMadre)
                        {
                            if(DetalleJ.Proveedor.Identificacion == DetalleI.Proveedor.Identificacion)
                            {
                                OrdenMadre.LineasOrdenMadre.Add(new OrdenMadreLineaViewModel()
                                {
                                    Tipo = DetalleJ.Tipo == "0" ? "BIEN" : "SERVICIO",
                                    Secuencial = DetalleJ.Id,
                                    CodigoLinea = "",
                                    Observacion = DetalleJ.ProductoNombre + " | " + DetalleJ.Observacion,
                                    CodigoArticulo = DetalleJ.ProductoCodigoArticulo,
                                    Valor = DetalleJ.Valor + (DetalleJ.Valor * (decimal)DetalleJ.Impuesto.Porcentaje / 100),
                                    Cantidad = DetalleJ.Tipo == "0" ? ((int)DetalleJ.Cantidad).ToString() : DetalleJ.Cantidad.ToString().Replace(',', '.'),
                                    Iva = DetalleJ.Impuesto.Codigo == "0" ? (double)0 : (double)1
                                });

                                DetalleJ.ProcesadoEnOrdenMadre = true;
                            }
                        }
                    }

                    OrdenesMadre.Add(OrdenMadre);
                }
            }

            var OrdenesMadreObj = OrdenMadreRCL.CrearOrdenesCompraMadre(sesion, OrdenesMadre, Solicitud.EmpresaCodigo);

            foreach(var ord in OrdenesMadreObj)
            {
                ord.SolicitudCompraCabeceraId = Solicitud.Id;
            }

            OrdenMadreDAL.SalvarOrdenesMadre(OrdenesMadreObj, db);

            foreach (var ord in OrdenesMadreObj)
            {
                TareaBLL.CrearRecepcion(Solicitud, sesion, db, string.Empty, 0, TareaPadreId, ord.Id);
            }

            NotificarOrdenCompraEmitida(Solicitud, OrdenesMadreObj, path, sesion, db);
        }

        /// <summary>
        /// Proceso para enviar notificaciones por ordenes de compra emitidas.
        /// </summary>
        /// <param name="SolicitudOriginal">Objeto que contiene los datos de la solicitud de compra.</param>
        /// <param name="OrdenesMadre">Listado de ordenes madre generadas.</param>
        /// <param name="path">Url base de la aplicación.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        private static void NotificarOrdenCompraEmitida(
            SolicitudCompraCabecera SolicitudOriginal, 
            List<OrdenMadre> OrdenesMadre, 
            string path,
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            SolicitudCompraCabeceraViewModel Solicitud = SolicitudCompraCabeceraDAL.ObtenerSolicitudCamposBasicos(OrdenesMadre.ElementAt(0).SolicitudCompraCabeceraId, db);
            var Solicitante = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteObjUsuario, sesion);

            foreach (var OrdenMadre in OrdenesMadre)
            {
                List<OrdenCompraLineaViewModel> ListaOrdenCompraLineas = new List<OrdenCompraLineaViewModel>();

                SolicitudCompraDetalleViewModel DetalleTem = null;

                decimal Subtotal = 0;
                decimal Iva = 0;
                decimal Total = 0;

                foreach (var li in OrdenMadre.LineasOrdenMadre)
                {
                    DetalleTem = SolicitudCompraDetalleDAL.ObtenerDetalle(li.Id, db);

                    var subtot = DetalleTem.Cantidad * DetalleTem.Valor;
                    var iv = DetalleTem.Total - subtot;
                    var tot = DetalleTem.Total;

                    Subtotal += subtot;
                    Iva += iv;
                    Total += tot;

                    var aux = li.Observacion.Split('|');

                    OrdenCompraLineaViewModel Linea = new OrdenCompraLineaViewModel()
                    {
                        Codigo = li.CodigoArticulo,
                        Descripcion = aux[0].Trim(),
                        Observacion = aux[1].Trim(),
                        Cantidad = DetalleTem.Cantidad,
                        PrecioUnitario = DetalleTem.Valor,
                        Iva = iv,
                        Importe = tot
                    };
                    ListaOrdenCompraLineas.Add(Linea);
                }

                OrdenCompraCabeceraViewModel model = new OrdenCompraCabeceraViewModel()
                {
                    NombreProveedor = DetalleTem.Proveedor.Nombre,
                    DireccionProveedor = DetalleTem.Proveedor.Direccion,
                    EmailProveedor = DetalleTem.Proveedor.Correo,
                    NumeroSolicitud = Solicitud.NumeroSolicitud.ToString(),
                    NumeroOrdenMadre = OrdenMadre.NumeroOrdenMadre,
                    FechaOrdenCompra = DateTime.Now,
                    NombreSolicitante = Solicitante.NombreCompleto,
                    Subtotal = Subtotal,
                    Iva = Iva,
                    Total = Total
                };

                List<OrdenCompraCabeceraViewModel> ListaOrdenCompra = new List<OrdenCompraCabeceraViewModel>();
                ListaOrdenCompra.Add(model);                

                ReportDocument rd = new ReportDocument();
                string direccion = path + "/Reportes";

                if(SolicitudOriginal.EmpresaCodigo == "VIT")
                {
                    rd.Load(Path.Combine(direccion, "OrdenCompraVitality.rpt"));
                }
                else if (SolicitudOriginal.EmpresaCodigo == "SRA")
                {
                    rd.Load(Path.Combine(direccion, "OrdenCompraServialamo.rpt"));
                }
                else
                {
                    rd.Load(Path.Combine(direccion, "OrdenCompra.rpt"));
                }

                rd.Database.Tables[0].SetDataSource(ListaOrdenCompra);
                rd.Database.Tables[1].SetDataSource(ListaOrdenCompraLineas);

                var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                byte[] AdjuntoArray = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    AdjuntoArray = ms.ToArray();
                }

                var UsuarioGestorCompra = RolGestorCompraDAL.ObtenerGestorDeUnaCompra(SolicitudOriginal, db);

                var UsuarioGestorCompraObj = UsuarioRCL.ObtenerUsuario(UsuarioGestorCompra, sesion);

                if (DetalleTem.Proveedor.Correo != null && DetalleTem.Proveedor.Correo != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailEnviarOrdenAlProveedor;

                    body = body.Replace("{DESTINATARIO}", DetalleTem.Proveedor.Nombre);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitante.NombreCompleto);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = DetalleTem.Proveedor.Nombre,
                            Direccion = DetalleTem.Proveedor.Correo
                        }
                    };

                    List<EmailDestinatarioViewModel> destinatarios_copia = new List<EmailDestinatarioViewModel>();

                    if (Solicitante.Email != null && Solicitante.Email != string.Empty)
                    {
                        destinatarios_copia.Add(new EmailDestinatarioViewModel()
                        {
                            Nombre = Solicitante.NombreCompleto,
                            Direccion = Solicitante.Email
                        });
                    }

                    if (UsuarioGestorCompraObj.Email != null && UsuarioGestorCompraObj.Email != string.Empty)
                    {
                        destinatarios_copia.Add(new EmailDestinatarioViewModel()
                        {
                            Nombre = UsuarioGestorCompraObj.NombreCompleto,
                            Direccion = UsuarioGestorCompraObj.Email
                        });
                    }

                    EmailBLL.EnviarConAdjunto(sesion, body, "Orden de compra emitida. Solicitud: " + SolicitudOriginal.NumeroSolicitud + ". Proveedor: " + DetalleTem.Proveedor.Nombre, destinatarios, destinatarios_copia, AdjuntoArray, db);
                }
                else if(Solicitante.Email != null && Solicitante.Email != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailEnviarOrdenAlSolicitante;

                    body = body.Replace("{DESTINATARIO}", Solicitante.NombreCompleto);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = Solicitante.NombreCompleto,
                            Direccion = Solicitante.Email
                        }
                    };

                    List<EmailDestinatarioViewModel> destinatarios_copia = new List<EmailDestinatarioViewModel>();

                    if (UsuarioGestorCompraObj.Email != null && UsuarioGestorCompraObj.Email != string.Empty)
                    {
                        destinatarios_copia.Add(new EmailDestinatarioViewModel()
                        {
                            Nombre = UsuarioGestorCompraObj.NombreCompleto,
                            Direccion = UsuarioGestorCompraObj.Email
                        });
                    }

                    EmailBLL.EnviarConAdjunto(sesion, body, "Orden de compra emitida. Solicitud: " + SolicitudOriginal.NumeroSolicitud + ". Proveedor: " + DetalleTem.Proveedor.Nombre, destinatarios, destinatarios_copia, AdjuntoArray, db);
                }

                rd.Dispose();
                rd.Close();
            }
        }

        /// <summary>
        /// Proceso para obtener una recepción.
        /// </summary>
        /// <param name="RecepcionId">Identificador de la recepción.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>RecepcionViewModel</returns>
        public static RecepcionViewModel ObtenerRecepcion(
            long RecepcionId, 
            ApplicationDbContext db)
        {
            return RecepcionDAL.ObtenerRecepcion(RecepcionId, db);
        }

        /// <summary>
        /// Proceso para obtener una solicitud con los detalles con saldo.
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
            return SolicitudCompraCabeceraDAL.ObtenerSolicitudConSaldosEnDetallesParaRecepcion(Id, OrdenMadreId, db);
        }

        /// <summary>
        /// Proceso para validar si una orden madre tiene saldo.
        /// </summary>
        /// <param name="OrdenMadreId">Identificador de la orden madre.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>bool</returns>
        public static bool ValidarSaldo(
            long OrdenMadreId, 
            ApplicationDbContext db)
        {
            return OrdenMadreDAL.ValidarSaldo(OrdenMadreId, db);
        }

        /// <summary>
        /// Proceso para generar una orden hija.
        /// </summary>
        /// <param name="recepcion">Objeto que contiene los datos de la recepción.</param>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string GenerarOrdenHija(
            Recepcion recepcion, 
            SolicitudCompraCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db)
        {
            var NumeroOrdenHija = string.Empty;
            var remisionRealizada = false;

            try
            {
                OrdenMadre OrdenMadre = recepcion.OrdenMadre;

                OrdenHija OrdenHija = new OrdenHija()
                {
                    Id = recepcion.Id,
                    NumeroOrdenHija = null,
                    OrdenMadreId = recepcion.OrdenMadreId
                };

                foreach (var RecepcionLinea in recepcion.RecepcionLineas)
                {
                    foreach (var Distribucion in RecepcionLinea.Distribuciones)
                    {
                        decimal cantidad = RecepcionLinea.Cantidad * Distribucion.Porcentaje / 100;
                        decimal precio_total = RecepcionLinea.Valor * Distribucion.Porcentaje / 100;
                        decimal precio_unitario = precio_total / cantidad;

                        OrdenHija.OrdenHijaLineas.Add(new OrdenHijaLinea()
                        {
                            Cantidad = cantidad,
                            PrecioUnitario = precio_unitario,
                            PrecioTotal = precio_total,
                            Departamento = Distribucion.DepartamentoCodigo,
                            CentroCosto = Distribucion.CentroCostoCodigo,
                            Proposito = Distribucion.PropositoCodigo,
                            OrdenMadreLineaId = RecepcionLinea.SolicitudCompraDetalleId,
                            OrdenMadreLinea = RecepcionLinea.SolicitudCompraDetalle.OrdenMadreLinea
                        });
                    }
                }

                OrdenHijaDAL.SalvarOrdenHija(OrdenHija, db);

                foreach (var OrdenHijaLinea in OrdenHija.OrdenHijaLineas)
                {
                    OrdenHijaLineaRCL.CrearLineaOrdenHija(OrdenMadre, OrdenHijaLinea, Solicitud.EmpresaCodigo, sesion);
                }

                NumeroOrdenHija = OrdenHijaRCL.CrearOrdenHija(OrdenMadre, OrdenHija, Solicitud.EmpresaCodigo, sesion);

                OrdenHija.NumeroOrdenHija = NumeroOrdenHija;

                OrdenHijaDAL.ActualizarOrdenHija(OrdenHija, db);

                string RespuestaRemisionOrdenHija = null;
                string NumeroRemision = string.Empty;

                try
                {
                    NumeroRemision = Solicitud.NumeroSolicitud + "-" + recepcion.NumeroRecepcion + "-" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    RespuestaRemisionOrdenHija = OrdenHijaRemisionRCL.CrearOrdenHijaRemision(OrdenHija, NumeroRemision, Solicitud.EmpresaCodigo, sesion);
                    remisionRealizada = true;
                }
                catch (Exception ex)
                {
                    OrdenHijaRCL.EliminarOrdenHija(OrdenHija.NumeroOrdenHija, Solicitud.EmpresaCodigo, sesion);

                    throw ex;
                }

                OrdenHijaRemision OrdenHijaRemision = new OrdenHijaRemision()
                {
                    Id = OrdenHija.Id,
                    NumeroRemisionOrdenHija = NumeroRemision,
                    RespuestaRemisionOrdenHija = RespuestaRemisionOrdenHija
                };

                OrdenHijaRemisionDAL.SalvarOrdenHijaRemision(OrdenHijaRemision, db);
            }
            catch(Exception ex2)
            {
                if(NumeroOrdenHija != string.Empty && remisionRealizada == true)
                {
                    TareaBLL.AnularRecepcion(NumeroOrdenHija, Solicitud.EmpresaCodigo, sesion);
                }

                throw ex2;
            }

            return NumeroOrdenHija;
        }

        /// <summary>
        /// Proceso para actualizar el estado de un comprobante electrónico.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="documento">Objeto que contiene los datos del comprobante electrónico.</param>
        public static void ActualizarEstadoComprobanteElectronico(
            ContenedorVariablesSesion sesion,
            ComprobanteElectronicoDocumentoViewModel documento)
        {
            ComprobanteElectronicoBLL.CambiarEstado(sesion, documento);
        }

        /// <summary>
        /// Proceso para borrar una factura de AX.
        /// </summary>
        /// <param name="NumeroOrdenHija">Número de orden hija.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void BorrarFacturaAX(
            string NumeroOrdenHija, 
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            OrdenHijaRCL.BorrarFactura(NumeroOrdenHija, codigoCompania, sesion);
        }

        /// <summary>
        /// Proceso para validar una factura en AX.
        /// </summary>
        /// <param name="numeroFactura">Número de la factura.</param>
        /// <param name="rucProveedor">RUC del proveedor.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>string</returns>
        public static string ValidarFacturaAX(
            string numeroFactura, 
            string rucProveedor, 
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            return OrdenHijaRCL.ValidarFactura(numeroFactura, rucProveedor, codigoCompania, sesion);
        }

        /// <summary>
        /// Proceso para insertar una factura en AX.
        /// </summary>
        /// <param name="numeroAutorizacion">Número de autorización.</param>
        /// <param name="numeroFactura">Número de la factura.</param>
        /// <param name="fechaDocumento">Fecha del documento.</param>
        /// <param name="fechaVigencia">Fecha de vigencia.</param>
        /// <param name="ordenCompraHija">Código de la orden de compra hija.</param>
        /// <param name="rucProveedor">RUC del proveedor.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <returns>string</returns>
        public static string InsertarFacturaAX(
            string numeroAutorizacion, 
            string numeroFactura, 
            string fechaDocumento, 
            string fechaVigencia, 
            string ordenCompraHija,
            string rucProveedor, 
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            return OrdenHijaRCL.InsertarFactura(numeroAutorizacion, numeroFactura, fechaDocumento, fechaVigencia, ordenCompraHija, rucProveedor, codigoCompania, sesion);
        }

        /// <summary>
        /// Proceso para anular una recepción.
        /// </summary>
        /// <param name="NumeroOrdenHija">Número de la orden hija.</param>
        /// <param name="codigoCompania">Identificador de la compañía.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        public static void AnularRecepcion(
            string NumeroOrdenHija,
            string codigoCompania, 
            ContenedorVariablesSesion sesion)
        {
            RecepcionRCL.AnularRecepcion(NumeroOrdenHija, codigoCompania, sesion);
        }
    }
}
