using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;


namespace Travel.Model.Management
{
    public class Encryption
    {
        private static string cadenaSeguridad = "AB467FFCB4BDF02295D63000602EA619081BFF9D5C7166A067EB61DD443484FD";
        private static string entrada = "FEABC65C6873D40E98643C54BE20251D9C138AA20B22F435C06EF4FABB13E6EE";

        public static string encryption(string valor)
        {
            DeriveBytes condificacionKey = new Rfc2898DeriveBytes(cadenaSeguridad, Encoding.Unicode.GetBytes(entrada));
            SymmetricAlgorithm algoritmoEncriptacion = new TripleDESCryptoServiceProvider();
            byte[] claveSimetrica = condificacionKey.GetBytes(algoritmoEncriptacion.KeySize >> 3);
            byte[] vectorInicializacion = condificacionKey.GetBytes(algoritmoEncriptacion.BlockSize >> 3);
            ICryptoTransform transformacion = algoritmoEncriptacion.CreateEncryptor(claveSimetrica, vectorInicializacion);
            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transformacion, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(valor);
                    }
                }
                return Convert.ToBase64String(buffer.ToArray());
            }
        }
    }
}
