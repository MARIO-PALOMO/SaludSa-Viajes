using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Model.Management
{


    public class ConecctionBD
    {
        public static SqlConnection cnn;

        public static SqlConnection ObtenerConecctionString()
        {

            if (ConfigurationManager.AppSettings["BDActivo"].ToString() == "0")
            {
                cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBDevelopment"].ToString());
            }
            else if (ConfigurationManager.AppSettings["BDActivo"].ToString() == "1")
            {
                cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBTest"].ToString());
            }
            else if (ConfigurationManager.AppSettings["BDActivo"].ToString() == "2")
            {
                cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBProducction"].ToString());
            }

            return cnn;
        }

        public static void Cerrar()
        {
            if (cnn != null)
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }

    }
}
