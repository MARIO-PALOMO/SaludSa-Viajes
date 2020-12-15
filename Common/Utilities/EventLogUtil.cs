using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace Common.Utilities
{
    public class EventLogUtil
    {
        #region Instancias

        public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly ILog LogWebIngenesis = LogManager.GetLogger("WebIngenesis");

        public static readonly ILog LogEscalamiento = LogManager.GetLogger("EscalamientoIngenesis");

        #endregion Instancias

        public static void EscribirLogErrorWeb(Exception exception, int EventId, string Origen)
        {
            var LogName = Origen == "WebIngenesisCompras" ? "WebIngenesis" : "EscalamientoIngenesis";

            var mensaje = exception.Message;

            if (Origen.Equals("WebIngenesisCompras", StringComparison.InvariantCultureIgnoreCase))
            {
                LogWebIngenesis.Error(exception);
            }
            else
            {
                LogEscalamiento.Error(exception);
            }

            try
            {
                if (!EventLog.SourceExists(Origen))
                {
                    EventLog.CreateEventSource(Origen, LogName);
                }

                EventLog.WriteEntry(Origen, mensaje, EventLogEntryType.Error, EventId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void EscribirLogErrorWeb(string mensaje, int EventId, string Origen)
        {
            var LogName = Origen == "WebIngenesisCompras" ? "WebIngenesis" : "EscalamientoIngenesis";

            if (Origen.Equals("WebIngenesisCompras", StringComparison.InvariantCultureIgnoreCase))
            {
                LogWebIngenesis.Error(mensaje);
            }
            else
            {
                LogEscalamiento.Error(mensaje);
            }

            try
            {
                if (!EventLog.SourceExists(Origen))
                {
                    EventLog.CreateEventSource(Origen, LogName);
                }

                EventLog.WriteEntry(Origen, mensaje, EventLogEntryType.Error, EventId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void EscribirLogInfoWeb(string mensaje, int EventId, string Origen)
        {
            var LogName = Origen == "WebIngenesisCompras" ? "WebIngenesis" : "EscalamientoIngenesis";

            if (Origen.Equals("WebIngenesisCompras", StringComparison.InvariantCultureIgnoreCase))
            {
                LogWebIngenesis.Info(mensaje);
            }
            else
            {
                LogEscalamiento.Info(mensaje);
            }

            try
            {
                if (!EventLog.SourceExists(Origen))
                {
                    EventLog.CreateEventSource(Origen, LogName);
                }

                EventLog.WriteEntry(Origen, mensaje, EventLogEntryType.Information, EventId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}