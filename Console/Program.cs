using Common.Utilities;
using Common.ViewModels;
using Data.Context;
using Logic;
using Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime FechaInicio = DateTime.Now;

            ApplicationDbContext db = new ApplicationDbContext();

            using (var dbcxtransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var HoraActual = int.Parse(DateTime.Now.ToString("HH"));

                    CultureInfo ci = new CultureInfo("Es-Es");
                    var DiaSemana = ci.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);

                    if (HoraActual >= 9 && HoraActual < 17 && DiaSemana != "sábado" && DiaSemana != "domingo")
                    {
                        var UrlServicios = ConfigurationManager.AppSettings["Url"].ToString();
                        var Usuario = ConfigurationManager.AppSettings["Usuario"].ToString();
                        var Clave = ConfigurationManager.AppSettings["Clave"].ToString();
                        var ClientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                        var CodigoAplicacion = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();

                        var EmailDestinatario = ConfigurationManager.AppSettings["EmailDestinatario"].ToString();
                        var EmailNombreOrigen = ConfigurationManager.AppSettings["EmailNombreOrigen"].ToString();
                        var EmailEmailOrigen = ConfigurationManager.AppSettings["EmailEmailOrigen"].ToString();
                        var EmailTiempoEspera = ConfigurationManager.AppSettings["EmailTiempoEspera"].ToString();

                        var UrlVisorRidePdf = ConfigurationManager.AppSettings["UrlVisorRidePdf"].ToString();
                        var IdClaseMFiles = ConfigurationManager.AppSettings["IdClaseMFiles"].ToString();

                        var UrlWeb = ConfigurationManager.AppSettings["UrlWeb"].ToString();

                        var token = AccountBLL.ObtenerToken(UrlServicios, Usuario, Clave, ClientId, "EscalamientoIngenesisCompras");

                        if (token != null && token.error == null)
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
                                OrigenLog = "EscalamientoIngenesisCompras"
                            };

                            TareaBLL.ActualizarIteraciones10Minutos(db);

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
                catch(Exception)
                {
                    dbcxtransaction.Rollback();
                }
            }

            DateTime FechaFin = DateTime.Now;

            int CalculoTiempo = (int)FechaFin.Subtract(FechaInicio).TotalMilliseconds;

            if (CalculoTiempo < 600000)
            {
                var TiempoEntreEjecuciones = 600000 - CalculoTiempo;
                Thread.Sleep(TiempoEntreEjecuciones);
            }

            //Llamar exe
            string direccionAbsoluta = ConfigurationManager.AppSettings["direccionAbsoluta"];
            System.Diagnostics.Process newProc;
            newProc = System.Diagnostics.Process.Start(direccionAbsoluta + "Exe\\Exe.exe", direccionAbsoluta);
        }

        static void ActualizarEstadoAAmarillo(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            System.Console.WriteLine("Inicia actualización a Amarillo");
            TareaBLL.ActualizarEstadoAAmarillo(sesion, db);
            System.Console.WriteLine("Finaliza actualización a Amarillo");
        }

        static void ActualizarEstadoARojo(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            System.Console.WriteLine("Inicia actualización a Rojo");
            TareaBLL.ActualizarEstadoARojo(sesion, db);
            System.Console.WriteLine("Finaliza actualización a Rojo");
        }

        static void NotificarAntesDeEscalar(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            System.Console.WriteLine("Inicia notificaciones antes de escalar");
            TareaBLL.NotificarAntesDeEscalar(sesion, db);
            System.Console.WriteLine("Finaliza notificaciones antes de escalar");
        }

        static void EscalarTareas(ContenedorVariablesSesion sesion, ApplicationDbContext db)
        {
            System.Console.WriteLine("Inicia escalar tareas");
            TareaBLL.EscalarTareas(sesion, db);
            System.Console.WriteLine("Finaliza escalar tareas");
        }
    }
}
