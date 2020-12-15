using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;

namespace Travel.Service.Token
{
    public class Token : ServiceAuthorizationManager
    {
        private static string cadenaSeguridad = "AB467FFCB4BDF02295D63000602EA619081BFF9D5C7166A067EB61DD443484FD";
        private static string entrada = "FEABC65C6873D40E98643C54BE20251D9C138AA20B22F435C06EF4FABB13E6EE";

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            bool result = false;

            Message message = operationContext.RequestContext.RequestMessage;
            object value;
            if (message.Properties.TryGetValue("HttpOperationName", out value))
            {
                if ((string)value == "LoginUser")
                {
                    result = true;
                }
            }

            if (!result)
            {
                HttpRequestMessageProperty httpRequestMessage;
                object httpRequestMessageObject;
                if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
                {
                    httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                    if (!string.IsNullOrEmpty(httpRequestMessage.Headers["Authorization"]))
                    {
                        string authorization = httpRequestMessage.Headers["Authorization"];
                        result = new Token().ValidacionAutorizacion(authorization);
                    }
                }
            }

            if (result)
            {
                operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] = Thread.CurrentPrincipal;
            }

            return result;
        }

        public bool ValidacionAutorizacion(string value)
        {
            bool result = false;
            if (value != null || value != "")
            {
                try
                {
                    DeriveBytes condificacionKey = new Rfc2898DeriveBytes(cadenaSeguridad, Encoding.Unicode.GetBytes(entrada));
                    SymmetricAlgorithm algoritmoEncriptacion = new TripleDESCryptoServiceProvider();
                    byte[] claveSimetrica = condificacionKey.GetBytes(algoritmoEncriptacion.KeySize >> 3);
                    byte[] vectorInicializacion = condificacionKey.GetBytes(algoritmoEncriptacion.BlockSize >> 3);

                    ICryptoTransform transform = new TripleDESCryptoServiceProvider().CreateDecryptor(claveSimetrica, vectorInicializacion);
                    byte[] buff = Convert.FromBase64String(value);

                    buff = transform.TransformFinalBlock(buff, 0, buff.Length);
                    string ticket = Encoding.Default.GetString(buff);

                    string[] values = ticket.Split('-');
                    if (values != null && values.Length == 2)
                    {
                        int userId;
                        long ticks;
                        if (int.TryParse(values[0], out userId) && long.TryParse(values[1], out ticks))
                        {
                            result = true;
                        }
                    }
                }
                catch (Exception)
                {
                    result = false;
                }

            }

            return result;
        }
    }
}