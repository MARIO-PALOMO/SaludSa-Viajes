using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
using Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Escalamiento
{
    partial class Escalamiento : ServiceBase
    {
        TokenViewModel token;

        bool blBandera = false;

        string UrlServicios;
        string Usuario;
        string Clave;
        string ClientId;
        string CodigoAplicacion;
        string EmailDestinatario;
        string EmailNombreOrigen;
        string EmailEmailOrigen;
        string EmailTiempoEspera;
        string UrlVisorRidePdf;
        string IdClaseMFiles;
        string UrlWeb;
        string OrigenLog;

        public Escalamiento()
        {
            InitializeComponent();

            try
            {
                UrlServicios = ConfigurationManager.AppSettings["Url"].ToString();
                Usuario = ConfigurationManager.AppSettings["Usuario"].ToString();
                Clave = ConfigurationManager.AppSettings["Clave"].ToString();
                ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                CodigoAplicacion = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();
                EmailDestinatario = ConfigurationManager.AppSettings["EmailDestinatario"].ToString();
                EmailNombreOrigen = ConfigurationManager.AppSettings["EmailNombreOrigen"].ToString();
                EmailEmailOrigen = ConfigurationManager.AppSettings["EmailEmailOrigen"].ToString();
                EmailTiempoEspera = ConfigurationManager.AppSettings["EmailTiempoEspera"].ToString();
                UrlVisorRidePdf = ConfigurationManager.AppSettings["UrlVisorRidePdf"].ToString();
                IdClaseMFiles = ConfigurationManager.AppSettings["IdClaseMFiles"].ToString();
                UrlWeb = ConfigurationManager.AppSettings["UrlWeb"].ToString();
                OrigenLog = "EscalamientoIngenesisCompras";

                token = AccountBLL.ObtenerToken(UrlServicios, Usuario, Clave, ClientId, OrigenLog);
            }
            catch(Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");

                throw ex;
            }
        }

        protected override void OnStart(string[] args)
        {
            stLapso.Start();
            EventLogUtil.EscribirLogInfoWeb("Servicio de escalamiento ejecutado.", 0, "EscalamientoIngenesisCompras");
        }

        protected override void OnStop()
        {
            stLapso.Stop();
            EventLogUtil.EscribirLogInfoWeb("Servicio de escalamiento detenido.", 0, "EscalamientoIngenesisCompras");
        }

        private void stLapso_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (blBandera) return;

                blBandera = true;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var HoraActual = int.Parse(DateTime.Now.ToString("HH"));

                            CultureInfo ci = new CultureInfo("Es-Es");
                            var DiaSemana = ci.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);

                            if (HoraActual >= 9 && HoraActual < 17 && DiaSemana != "sábado" && DiaSemana != "domingo")
                            {
                                if(token == null || token.error != null)
                                {
                                    token = AccountBLL.ObtenerToken(UrlServicios, Usuario, Clave, ClientId, OrigenLog);
                                }
                                else if(token.expires <= DateTime.Now)
                                {
                                    token = AutenticationRCL.RefrescarToken(UrlServicios, ClientId, token.refresh_token, OrigenLog);

                                    if (token == null || token.error != null)
                                    {
                                        token = AccountBLL.ObtenerToken(UrlServicios, Usuario, Clave, ClientId, OrigenLog);
                                    }
                                }
                                
                                if(token != null && token.error == null)
                                {
                                    ContenedorVariablesSesion sesion = new ContenedorVariablesSesion()
                                    {
                                        UrlServicios = UrlServicios,
                                        token = token,
                                        CodigoAplicacion = CodigoAplicacion,
                                        SistemaOperativo = "------",
                                        DispositivoNavegador = "------",
                                        DireccionIP = "10.10.10.10",
                                        EmailNombreOrigen = EmailNombreOrigen,
                                        EmailEmailOrigen = EmailEmailOrigen,
                                        EmailTiempoEspera = EmailTiempoEspera,
                                        UrlWeb = UrlWeb,
                                        EmailDestinatario = EmailDestinatario,
                                        UrlVisorRidePdf = UrlVisorRidePdf,
                                        IdClaseMFiles = IdClaseMFiles,
                                        OrigenLog = OrigenLog
                                    };

                                    EventLogUtil.EscribirLogInfoWeb("Inicia incremento de contador de tiempo.", 0, "EscalamientoIngenesisCompras");
                                    TareaBLL.ActualizarIteraciones10Minutos(db);
                                    EventLogUtil.EscribirLogInfoWeb("Finaliza incremento de contador de tiempo.", 0, "EscalamientoIngenesisCompras");

                                    List<Thread> ListaHilos = new List<Thread>();

                                    Thread ActualizarEstadoAAmarilloHilo = new Thread(() => ActualizarEstadoAAmarillo(sesion, db));
                                    Thread ActualizarEstadoARojoHilo = new Thread(() => ActualizarEstadoARojo(sesion, db));
                                    Thread NotificarAntesDeEscalarHilo = new Thread(() => NotificarAntesDeEscalar(sesion, db));
                                    Thread EscalarTareasHilo = new Thread(() => EscalarTareas(sesion, db));

                                    ListaHilos.Add(ActualizarEstadoAAmarilloHilo);
                                    ListaHilos.Add(ActualizarEstadoARojoHilo);
                                    ListaHilos.Add(NotificarAntesDeEscalarHilo);
                                    ListaHilos.Add(EscalarTareasHilo);

                                    ActualizarEstadoAAmarilloHilo.Start();
                                    ActualizarEstadoARojoHilo.Start();
                                    NotificarAntesDeEscalarHilo.Start();
                                    EscalarTareasHilo.Start();

                                    foreach (var hilo in ListaHilos)
                                    {
                                        hilo.Join();
                                    }

                                    db.SaveChanges();
                                    dbcxtransaction.Commit();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            dbcxtransaction.Rollback();
                            EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
                        }
                    }
                }

                blBandera = false;
            }
            catch (Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
            }
        }

        void ActualizarEstadoAAmarillo(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            EventLogUtil.EscribirLogInfoWeb("Inicia actualización a Amarillo.", 0, "EscalamientoIngenesisCompras");

            try
            {
                TareaBLL.ActualizarEstadoAAmarillo(sesion, db);
            }
            catch (Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
            }
            
            EventLogUtil.EscribirLogInfoWeb("Finaliza actualización a Amarillo.", 0, "EscalamientoIngenesisCompras");
        }

        void ActualizarEstadoARojo(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            EventLogUtil.EscribirLogInfoWeb("Inicia actualización a Rojo.", 0, "EscalamientoIngenesisCompras");

            try
            {
                TareaBLL.ActualizarEstadoARojo(sesion, db);
            }
            catch(Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
            }

            EventLogUtil.EscribirLogInfoWeb("Finaliza actualización a Rojo.", 0, "EscalamientoIngenesisCompras");
        }

        void NotificarAntesDeEscalar(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            EventLogUtil.EscribirLogInfoWeb("Inicia notificaciones previo escalamiento.", 0, "EscalamientoIngenesisCompras");

            try
            {
                TareaBLL.NotificarAntesDeEscalar(sesion, db);
            }
            catch (Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
            }
            
            EventLogUtil.EscribirLogInfoWeb("Finaliza notificaciones previo escalamiento.", 0, "EscalamientoIngenesisCompras");
        }

        void EscalarTareas(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            EventLogUtil.EscribirLogInfoWeb("Inicia escalar tareas.", 0, "EscalamientoIngenesisCompras");

            try
            {
                TareaBLL.EscalarTareas(sesion, db);
            }
            catch (Exception ex)
            {
                EventLogUtil.EscribirLogErrorWeb(ex.ToString(), 0, "EscalamientoIngenesisCompras");
            }
            
            EventLogUtil.EscribirLogInfoWeb("Finaliza escalar tareas.", 0, "EscalamientoIngenesisCompras");
        }
    }
}
