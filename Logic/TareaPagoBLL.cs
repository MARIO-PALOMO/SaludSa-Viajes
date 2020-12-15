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
    /// Gestiona la lógica de negocio de tareas del subsistema de pagos.
    /// </summary>
    public class TareaPagoBLL
    {
        /// <summary>
        /// Proceso para crear tareas.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="tipo">Tipo de tarea a crear.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        public static void CrearTarea(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            int tipo, 
            ApplicationDbContext db, 
            long TareaPadreId)
        {
            switch(tipo)
            {
                case (int)EnumTipoTareaPago.CREACION_SOLICITUD:
                    CrearCreacionSolicitud(Solicitud, sesion, db);
                    break;
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA:
                    CrearAprobacionMontoJefeArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA:
                    CrearAprobacionPorMontoSubgerenteArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA:
                    CrearAprobacionPorMontoGerenteArea(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO:
                    CrearAprobacionPorMontoVicepresidenteFinanciero(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE:
                    CrearDevolverASolicitantePorJefe(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.CONTABILIZAR:
                    foreach(var fac in Solicitud.Facturas)
                    {
                        CrearContabilizar(Solicitud, sesion, db, string.Empty, 0, TareaPadreId, fac.Id);
                    }
                    break;
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD:
                    CrearDevolverASolicitantePorContabilidad(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
                    break;
                case (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE_CONTABILIDAD:
                    CrearDevolverASolicitantePorJefeContabilidad(Solicitud, sesion, db, string.Empty, 0, TareaPadreId);
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
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db)
        {
            var UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

            TareaPago tarea = new TareaPago()
            {
                Actividad = "Registro de solicitud de pago",
                UsuarioResponsable = Solicitud.SolicitanteUsuario,
                NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                FechaCreacion = DateTime.Now,
                FechaProcesamiento = DateTime.Now,
                SolicitudPagoCabeceraId = Solicitud.Id,
                EstadoId = (int)EnumEstado.INACTIVO,
                TipoTarea = (int)EnumTipoTareaPago.CREACION_SOLICITUD,
                Accion = "Registro",
                EmailResponsable = UsuarioObj.Email
            };

            TareaPagoDAL.SalvarTarea(tarea, db);
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por montos para jefe de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionMontoJefeArea(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion,
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            TareaPagoViewModel TareaPadre = null;

            if (TareaPadreId != 0)
            {
                TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);
            }

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación del jefe de área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA,
                    EmailResponsable = UsuarioObj.Email

                };

                if(TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }
            else
            {
                if (TareaPadreId != 0)
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaPadre.FacturaCabeceraPago.AprobacionJefeArea, sesion);
                }
                else
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionJefeArea, sesion);
                }

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación del jefe de área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_JEFE_AREA,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionMontoJefeArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionJefeInmediatoPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Aprobación de jefe de área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por montos para subgerente de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoSubgerenteArea(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion,
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            TareaPagoViewModel TareaPadre = null;

            if (TareaPadreId != 0)
            {
                TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);
            }

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Subgerente de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }
            else
            {
                if (TareaPadreId != 0 && TareaPadre.FacturaCabeceraPago != null)
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaPadre.FacturaCabeceraPago.AprobacionSubgerenteArea, sesion);
                }
                else
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionSubgerenteArea, sesion);
                }

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Subgerente de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_SUBGERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoSubgerenteArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionSubgerenteAreaPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Aprobación por monto Subgerente de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por montos para gerente de área.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoGerenteArea(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            TareaPagoViewModel TareaPadre = null;

            if (TareaPadreId != 0)
            {
                TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);
            }

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Gerente de Área (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }
            else
            {
                if (TareaPadreId != 0 && TareaPadre.FacturaCabeceraPago != null)
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaPadre.FacturaCabeceraPago.AprobacionGerenteArea, sesion);
                }
                else
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionGerenteArea, sesion);
                }

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Gerente de Área",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_AREA,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoGerenteArea(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGerenteAreaPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Aprobación por monto Gerente de Área", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por montos para vicepresidente financiero.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearAprobacionPorMontoVicepresidenteFinanciero(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion,
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            TareaPagoViewModel TareaPadre = null;

            if (TareaPadreId != 0)
            {
                TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);
            }

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Vicepresidente Financiero (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }
            else
            {
                if (TareaPadreId != 0 && TareaPadre.FacturaCabeceraPago != null)
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaPadre.FacturaCabeceraPago.AprobacionVicePresidenteFinanciero, sesion);
                }
                else
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionVicePresidenteFinanciero, sesion);
                }

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Vicepresidente Financiero",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_VICEPRESIDENTE_FINANCIERO,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoVicepresidenteFinanciero(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailAprobacionVicepresidenteFinancieroPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Aprobación por monto Vicepresidente Financiero", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de aprobación por montos para gerente general.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="path">Url base de la aplicación.</param>
        public static void CrearAprobacionPorMontoGerenteGeneral(
            SolicitudPagoCabecera Solicitud,
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar, 
            int iteracion, 
            long TareaPadreId, 
            string path)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            TareaPagoViewModel TareaPadre = null;

            if (TareaPadreId != 0)
            {
                TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);
            }

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Gerente General (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }
            else
            {
                if (TareaPadreId != 0 && TareaPadre.FacturaCabeceraPago != null)
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(TareaPadre.FacturaCabeceraPago.AprobacionGerenteGeneral, sesion);
                }
                else
                {
                    UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.AprobacionGerenteGeneral, sesion);
                }

                tarea = new TareaPago()
                {
                    Actividad = "Aprobación por monto Gerente General",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.APROBACION_POR_MONTO_GERENTE_GENERAL,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };

                if (TareaPadreId != 0)
                {
                    tarea.TareaPadreId = TareaPadreId;
                    tarea.FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId;
                }
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearAprobacionPorMontoGerenteGeneral(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId, path);
            }
            else
            {
                var enviarCorreo = true;

                if (TareaPadre.UsuarioResponsable == tarea.UsuarioResponsable)
                {
                    tarea.Accion = "Aprobar";
                    tarea.Observacion = "Aprobada de forma automática.";
                    tarea.EstadoId = (int)EnumEstado.INACTIVO;
                    tarea.FechaProcesamiento = DateTime.Now;

                    enviarCorreo = false;
                }

                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty && enviarCorreo)
                {
                    string body = EmailTemplatesResource.EmailAprobacionGerenteGeneralPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{SOLICITANTE}", Solicitud.SolicitanteNombreCompleto);
                    body = body.Replace("{LINK}", sesion.UrlWeb + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Aprobación por monto Gerente General", destinatarios, null, tarea.Id, db);
                }
                else if (enviarCorreo == false)
                {
                    TareaPagoBLL.CrearTarea(Solicitud, sesion, (int)EnumTipoTareaPago.CONTABILIZAR, db, tarea.Id);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de devolver a solicitante por jefe inmediato antes de pasar por contabilidad.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearDevolverASolicitantePorJefe(
            SolicitudPagoCabecera Solicitud, 
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db,
            string UsuarioReasignar,
            int iteracion,
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta para edición (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta para edición",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE,
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
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearDevolverASolicitantePorJefe(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    var TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);

                    string body = EmailTemplatesResource.EmailDevolucionSolicitantePago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{RESPONSABLE_ANTERIOR}", TareaPadre.NombreCompletoResponsable);
                    body = body.Replace("{COMENTARIO}", TareaPadre.Observacion);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Devolución a solicitante", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de contabilizar.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        /// <param name="FacturaCabeceraPagoId">Identificador de la factura.</param>
        private static void CrearContabilizar(
            SolicitudPagoCabecera Solicitud,
            ContenedorVariablesSesion sesion, 
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId,
            long FacturaCabeceraPagoId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Contabilización (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.CONTABILIZAR,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = FacturaCabeceraPagoId
                };
            }
            else
            {
                string UsuarioContabilizacion= RolAdministrativoDAL.ObtenerUsuarioContabilizarPago(Solicitud, db);

                if (UsuarioContabilizacion == null || UsuarioContabilizacion == string.Empty)
                {
                    throw new Exception("No se ha configurado un usuario para contabilizar pagos en la empresa \"" + Solicitud.EmpresaNombre + "\" y la ciudad \"" + Solicitud.SolicitanteCiudadCodigo + "\".");
                }

                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioContabilizacion, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Contabilización",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.CONTABILIZAR,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = FacturaCabeceraPagoId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearContabilizar(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId, FacturaCabeceraPagoId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailContabilizarPago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Contabilizar pago", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de devolver a solicitante desde contabilidad.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearDevolverASolicitantePorContabilidad(
            SolicitudPagoCabecera Solicitud,
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            var TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta por contabilidad (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta por contabilidad",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearDevolverASolicitantePorContabilidad(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailDevolucionContabilidadSolicitantePago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{FACTURA}", TareaPadre.FacturaCabeceraPago.NoFactura);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{RESPONSABLE_ANTERIOR}", TareaPadre.NombreCompletoResponsable);
                    body = body.Replace("{COMENTARIO}", TareaPadre.Observacion);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Devolución a solicitante desde contabilidad", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para crear la tarea de devolver a solicitante por jefe inmediato despues de pasar por contabilidad.
        /// </summary>
        /// <param name="Solicitud">Objeto que contiene los datos de la cabecera de la solicitud.</param>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <param name="UsuarioReasignar">Nombre de usuario para reasignar la tarea.</param>
        /// <param name="iteracion">Contador de iteraciones recursivas.</param>
        /// <param name="TareaPadreId">Identificador de la tarea padre.</param>
        private static void CrearDevolverASolicitantePorJefeContabilidad(
            SolicitudPagoCabecera Solicitud,
            ContenedorVariablesSesion sesion,
            ApplicationDbContext db, 
            string UsuarioReasignar,
            int iteracion, 
            long TareaPadreId)
        {
            TareaPago tarea = null;

            UsuarioViewModel UsuarioObj = null;

            var TareaPadre = TareaPagoDAL.ObtenerTarea(TareaPadreId, db);

            if (UsuarioReasignar != null && UsuarioReasignar != string.Empty)
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(UsuarioReasignar, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta para edición (Por reasignación)",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD,
                    //TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE_CONTABILIDAD,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId
                };
            }
            else
            {
                UsuarioObj = UsuarioRCL.ObtenerUsuario(Solicitud.SolicitanteUsuario, sesion);

                tarea = new TareaPago()
                {
                    Actividad = "Solicitud devuelta para edición",
                    UsuarioResponsable = UsuarioObj.Usuario,
                    NombreCompletoResponsable = UsuarioObj.NombreCompleto,
                    FechaCreacion = DateTime.Now,
                    FechaProcesamiento = null,
                    SolicitudPagoCabeceraId = Solicitud.Id,
                    EstadoId = (int)EnumEstado.ACTIVO,
                    TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_CONTABILIDAD,
                    //TipoTarea = (int)EnumTipoTareaPago.DEVOLVER_A_SOLICITANTE_JEFE_CONTABILIDAD,
                    TareaPadreId = TareaPadreId,
                    EmailResponsable = UsuarioObj.Email,
                    FacturaCabeceraPagoId = TareaPadre.FacturaCabeceraPagoId
                };
            }

            var UsuarioReasignarTem = UsuarioDAL.ObtenerUsuarioParaReasignacion(UsuarioObj.Usuario, db);

            if (UsuarioReasignarTem != null && UsuarioReasignarTem != string.Empty && iteracion < 4)
            {
                tarea.EstadoId = (int)EnumEstado.INACTIVO;
                tarea.FechaProcesamiento = DateTime.Now;
                tarea.Accion = "Reasignada";
                TareaPagoDAL.SalvarTarea(tarea, db);
                iteracion++;
                CrearDevolverASolicitantePorJefeContabilidad(Solicitud, sesion, db, UsuarioReasignarTem, iteracion, TareaPadreId);
            }
            else
            {
                TareaPagoDAL.SalvarTarea(tarea, db);

                if (tarea.EmailResponsable != null && tarea.EmailResponsable != string.Empty)
                {
                    string body = EmailTemplatesResource.EmailDevolucionContabilidadSolicitantePago;

                    body = body.Replace("{RESPONSABLE}", tarea.NombreCompletoResponsable);
                    body = body.Replace("{FACTURA}", TareaPadre.FacturaCabeceraPago.NoFactura);
                    body = body.Replace("{COMPRA}", Solicitud.NumeroSolicitud.ToString());
                    body = body.Replace("{RESPONSABLE_ANTERIOR}", TareaPadre.NombreCompletoResponsable);
                    body = body.Replace("{COMENTARIO}", TareaPadre.Observacion);
                    body = body.Replace("{LINK}", sesion.UrlWebPago + "?tipo=" + tarea.TipoTarea + "&tareaId=" + tarea.Id + "&solicitudId=" + tarea.SolicitudPagoCabeceraId);

                    List<EmailDestinatarioViewModel> destinatarios = new List<EmailDestinatarioViewModel>()
                    {
                        new EmailDestinatarioViewModel()
                        {
                            Nombre = tarea.NombreCompletoResponsable,
                            Direccion = tarea.EmailResponsable
                        }
                    };

                    EmailBLL.EnviarPago(sesion, body, "Devolución a solicitante", destinatarios, null, tarea.Id, db);
                }
            }
        }

        /// <summary>
        /// Proceso para contabilizar.
        /// </summary>
        /// <param name="sesion">Objeto de la sesión.</param>
        /// <param name="tarea">Objeto que contiene los datos de la tarea.</param>
        /// <param name="InformacionContabilidadPago">Objeto que contiene la información de contabilización entrada por el usuario.</param>
        /// <param name="Detalles">Lista con los detalles de la factura actualizados con la información de contabilidad.</param>
        /// <param name="db">Contexto de base de datos.</param>
        /// <returns>string</returns>
        public static string Contabilizar(
            ContenedorVariablesSesion sesion,
            TareaPago tarea,
            InformacionContabilidadPago InformacionContabilidadPago,
            List<FacturaDetallePagoViewModel> Detalles, 
            ApplicationDbContext db)
        {
            var DetallesLineasPago = new List<string>();
            string codigoCompaniaTem = string.Empty;
            string numeroDiarioTem = string.Empty;

            try
            {
                var Factura = tarea.FacturaCabeceraPago;
                var Solicitud = tarea.SolicitudPagoCabecera;
                codigoCompaniaTem = Solicitud.EmpresaCodigo;

                var descripcionDiario = "F#" + Factura.NoFactura + " " + Solicitud.NombreCorto;

                // ****************************************** //
                // ******** Crear Cabecera de Diario ******** //

                var DiarioCabeceraPago = new DiarioCabeceraPago()
                {
                    Id = tarea.Id,
                    nombreDiario = InformacionContabilidadPago.DiarioCodigo,
                    descripcionDiario = descripcionDiario,
                    codigoCompania = Solicitud.EmpresaCodigo
                };

                var NumeroDiario = ContabilidadPagoRCL.CrearDiario(
                    sesion,
                    DiarioCabeceraPago.nombreDiario,
                    DiarioCabeceraPago.descripcionDiario,
                    DiarioCabeceraPago.codigoCompania
                );

                numeroDiarioTem = NumeroDiario;
                DiarioCabeceraPago.numeroDiario = NumeroDiario;

                // ******** Crear Cabecera de Diario ******** //
                // ****************************************** //


                // *************************************** //
                // ******** Crear Línea Proveedor ******** //

                var DiarioLineaProveedorPago = new DiarioLineaProveedorPago()
                {
                    Id = DiarioCabeceraPago.Id,
                    tipoDiario = InformacionContabilidadPago.TipoDiarioCodigo,
                    numeroDiario = NumeroDiario,
                    valor = Factura.Total,
                    fecha = Factura.FechaEmision,
                    proveedor = Solicitud.BeneficiarioIdentificacion,
                    descripcion = descripcionDiario,
                    referencia = "Ref.",
                    departameto = InformacionContabilidadPago.DepartamentoCodigo,
                    perfilAsiento = InformacionContabilidadPago.PerfilAsientoContableCodigo,
                    codigoCompania = Solicitud.EmpresaCodigo,
                    numeroFactura = ((Factura.NoLiquidacion != null) ? Factura.NoLiquidacion : Factura.NoFactura)
                };

                if (codigoCompaniaTem == "VIT")
                {
                    decimal temTotal = 0;

                    foreach (var detalle in Factura.FacturaDetallesPago)
                    {
                        temTotal += detalle.Valor;
                    }

                    DiarioLineaProveedorPago.valor = temTotal;
                }

                string LineaId = string.Empty;

                if (DiarioLineaProveedorPago.tipoDiario == "0")
                {
                    LineaId = ContabilidadPagoRCL.LineaProveedorReembolso(
                        sesion,
                        DiarioLineaProveedorPago.numeroDiario,
                        DiarioLineaProveedorPago.valor,
                        DiarioLineaProveedorPago.fecha,
                        DiarioLineaProveedorPago.proveedor,
                        DiarioLineaProveedorPago.descripcion,
                        DiarioLineaProveedorPago.referencia,
                        DiarioLineaProveedorPago.departameto,
                        DiarioLineaProveedorPago.perfilAsiento,
                        DiarioLineaProveedorPago.codigoCompania
                    );
                }
                else if (DiarioLineaProveedorPago.tipoDiario == "10")
                {
                    LineaId = ContabilidadPagoRCL.LineaProveedorFactura(
                        sesion,
                        DiarioLineaProveedorPago.numeroDiario,
                        DiarioLineaProveedorPago.valor,
                        DiarioLineaProveedorPago.fecha,
                        DiarioLineaProveedorPago.proveedor,
                        DiarioLineaProveedorPago.descripcion,
                        DiarioLineaProveedorPago.numeroFactura,
                        DiarioLineaProveedorPago.perfilAsiento,
                        DiarioLineaProveedorPago.referencia,
                        DiarioLineaProveedorPago.departameto,
                        DiarioLineaProveedorPago.codigoCompania
                    );
                }

                DiarioLineaProveedorPago.lineaProveedorId = LineaId;
                DiarioCabeceraPago.DiarioLineaProveedorPago = DiarioLineaProveedorPago;

                // ******** Crear Línea Proveedor ******** //
                // *************************************** //


                // ************************************* //
                // ******** Crear Detalle Línea ******** //

                if (InformacionContabilidadPago.TipoDiarioCodigo == "0")
                {
                    foreach (var detalle in Factura.FacturaDetallesPago)
                    {
                        foreach (var distribucion in detalle.Distribuciones)
                        {
                            decimal Valor = detalle.Subtotal * distribucion.Porcentaje / 100;

                            var DiarioDetalleLineaPago = new DiarioDetalleLineaPago()
                            {
                                tipoDiario = InformacionContabilidadPago.TipoDiarioCodigo,
                                numeroDiario = NumeroDiario,
                                valor = Valor,
                                cuentaContable = InformacionContabilidadPago.CuentaContableCodigo,
                                descripcion = detalle.Descripcion,
                                departamento = distribucion.DepartamentoCodigo,
                                centroCosto = distribucion.CentroCostoCodigo,
                                proposito = distribucion.PropositoCodigo,
                                codigoProyecto = "0",
                                codigoCompania = Solicitud.EmpresaCodigo
                            };

                            var DetalleLineaId = ContabilidadPagoRCL.LineaReembolso(
                                sesion,
                                DiarioDetalleLineaPago.numeroDiario,
                                DiarioDetalleLineaPago.valor,
                                DiarioDetalleLineaPago.cuentaContable, // revisar
                                DiarioDetalleLineaPago.descripcion,
                                DiarioDetalleLineaPago.departamento,
                                DiarioDetalleLineaPago.centroCosto,
                                DiarioDetalleLineaPago.proposito,
                                DiarioDetalleLineaPago.codigoProyecto,
                                DiarioDetalleLineaPago.codigoCompania
                            );

                            DetallesLineasPago.Add(DetalleLineaId);
                            DiarioDetalleLineaPago.detalleLineaId = DetalleLineaId;
                            DiarioCabeceraPago.DiarioDetallesLineaPago.Add(DiarioDetalleLineaPago);
                        }
                    }
                }
                else if (InformacionContabilidadPago.TipoDiarioCodigo == "10")
                {
                    foreach (var detalle in Factura.FacturaDetallesPago)
                    {
                        foreach (var distribucion in detalle.Distribuciones)
                        {
                            var parametros = Solicitud.EmpresaCodigo + "|" +
                                             InformacionContabilidadPago.CuentaContableTipo + "|" + 
                                             NumeroDiario + "|" +
                                             InformacionContabilidadPago.CuentaContableCodigo + "|" + 
                                             detalle.GrupoImpuestoCodigo + "|" +
                                             detalle.GrupoImpuestoArticuloCodigo + "|" +
                                             detalle.SustentoTributarioCodigo + "|" +
                                             detalle.ImpuestoRentaGrupoImpuestoArticuloCodigo + "|" +
                                             detalle.IvaGrupoImpuestoArticuloCodigo + "|" +
                                             distribucion.DepartamentoCodigo + "|" +
                                             distribucion.CentroCostoCodigo + "|" +
                                             distribucion.PropositoCodigo + "|" +
                                             "|" + 
                                             detalle.ImpuestoPago.Porcentaje + "|" + 
                                             "N|" + 
                                             "false|";

                            var descripcionLineaNueva = "F#" + Factura.NoFactura + " / " + detalle.Descripcion + " / " + Solicitud.NombreCorto;

                            decimal Valor = detalle.Subtotal * distribucion.Porcentaje / 100;

                            if (codigoCompaniaTem == "VIT")
                            {
                                Valor = detalle.Valor * distribucion.Porcentaje / 100;
                            }

                            var DiarioDetalleLineaPago = new DiarioDetalleLineaPago()
                            {
                                tipoDiario = InformacionContabilidadPago.TipoDiarioCodigo,
                                valor = Valor,
                                descripcion = descripcionLineaNueva,
                                credito = false,
                                parametros = parametros,
                                numeroDiario = NumeroDiario
                            };

                            var DetalleLineaId = ContabilidadPagoRCL.LineaFactura(
                                sesion,
                                DiarioDetalleLineaPago.parametros,
                                DiarioDetalleLineaPago.valor,
                                DiarioDetalleLineaPago.descripcion,
                                DiarioDetalleLineaPago.credito
                            );

                            DetallesLineasPago.Add(DetalleLineaId);
                            DiarioDetalleLineaPago.detalleLineaId = DetalleLineaId;
                            DiarioCabeceraPago.DiarioDetallesLineaPago.Add(DiarioDetalleLineaPago);
                        }
                    }
                }

                // ******** Crear Detalle Línea ******** //
                // ************************************* //


                // *********************************** //
                // ******** Cierre del diario ******** //

                var NumeroAutorizacionFisica = string.Empty;
                var NumeroAutorizacionElectronica = string.Empty;
                DateTime FechaVencimiento = DateTime.Now;

                if (InformacionContabilidadPago.EsReembolso == true)
                {
                    NumeroAutorizacionFisica = "0";
                    NumeroAutorizacionElectronica = "0";
                    FechaVencimiento = DateTime.Now;
                }
                else
                {
                    if (Factura.Tipo == "Física")
                    {
                        NumeroAutorizacionFisica = Factura.NoAutorizacion;
                        NumeroAutorizacionElectronica = "0";
                        FechaVencimiento = Factura.FechaVencimiento;
                    }
                    else if (Factura.Tipo == "Electrónica")
                    {
                        NumeroAutorizacionFisica = "0";
                        NumeroAutorizacionElectronica = Factura.NoAutorizacion;
                        FechaVencimiento = DateTime.Now;
                    }
                }

                var DiarioCierrePago = new DiarioCierrePago()
                {
                    Id = DiarioCabeceraPago.Id,
                    numeroDiario = NumeroDiario,
                    autorizacion = NumeroAutorizacionFisica,
                    fechaVigencia = FechaVencimiento,
                    autorizacionElectronica = NumeroAutorizacionElectronica,
                    codigoCompania = Solicitud.EmpresaCodigo
                };

                var CodigoCierreDiario = ContabilidadPagoRCL.RegistraDiario(
                    sesion,
                    DiarioCierrePago.numeroDiario,
                    DiarioCierrePago.autorizacion,
                    DiarioCierrePago.fechaVigencia,
                    DiarioCierrePago.autorizacionElectronica,
                    DiarioCierrePago.codigoCompania
                );

                DiarioCierrePago.cierreDiarioId = CodigoCierreDiario;
                DiarioCabeceraPago.DiarioCierrePago = DiarioCierrePago;

                // ******** Cierre del diario ******** //
                // *********************************** //

                db.DiarioCabecerasPago.Add(DiarioCabeceraPago);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                try
                {
                    foreach(var DetalleLineaPagoId in DetallesLineasPago)
                    {
                        ContabilidadPagoRCL.BorrarLineaDiario(
                            sesion,
                            DetalleLineaPagoId,
                            codigoCompaniaTem
                        );
                    }

                    if(numeroDiarioTem != string.Empty)
                    {
                        ContabilidadPagoRCL.BorrarDiario(
                            sesion,
                            numeroDiarioTem,
                            codigoCompaniaTem
                        );
                    }
                    
                }
                catch(Exception ex2)
                {
                    throw ex;
                }

                throw ex;
            }

            return numeroDiarioTem;
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
            return TareaPagoDAL.ObtenerTareas(sesion, db);
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
            return TareaPagoDAL.ObtenerCantidadTareasPendientes(sesion, db);
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
            return TareaPagoDAL.ObtenerTarea(Id, db);
        }
    }
}
