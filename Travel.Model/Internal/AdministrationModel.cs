using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Model.Management;

namespace Travel.Model.Internal
{
    public class AdministrationModel
    {
        public static string generateToken()
        {
            string parameter = ConfigurationManager.AppSettings["KeyInternal"].ToString();
            return Encryption.encryption(parameter);
        }
    }
}
