using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Entity.Entity;
using Travel.Model.Management;

namespace Travel.Model.Internal
{
    public class MParameter : ConecctionBD
    {
        public static List<EParameter> getParameters()
        {
            List<EParameter> list = new List<EParameter>();
            EParameter parameter;
            try
            {
                using (SqlConnection connection = ObtenerConecctionString())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Parametro WHERE Parametro.EstadoParametro = 1", connection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {

                        parameter = new EParameter();

                        parameter.IdParametro = rdr["IdParametro"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["IdParametro"]);
                        parameter.NombreParametro = rdr["NombreParametro"].ToString();
                        parameter.ValorParametro = rdr["ValorParametro"].ToString();
                        parameter.EstadoParametro = rdr["EstadoParametro"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["EstadoParametro"]);

                        list.Add(parameter);
                    }
                    rdr.Close();
                    return list;
                }
            }
            catch (SqlException)
            {
                Cerrar();
                throw;
            }
            finally
            {
                Cerrar();
            }
        }
    }
}
