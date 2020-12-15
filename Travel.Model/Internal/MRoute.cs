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
    public class MRoute : ConecctionBD
    {
        public static List<ERoute> ConsultarRutas()
        {
            List<ERoute> lsRoute = new List<ERoute>();
            ERoute route;
            try
            {
                using (SqlConnection connection = ObtenerConecctionString())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM ConsultaListadoRutas", connection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {

                        route = new ERoute();

                        route.IdRuta = Convert.ToInt32(rdr["IdRuta"]);
                        route.NombreRuta = rdr["NombreRuta"].ToString();
                        route.DescripcionRuta = rdr["DescripcionRuta"].ToString();
                        route.OrigenRuta = rdr["OrigenRuta"].ToString();
                        route.DestinoRuta = rdr["DestinoRuta"].ToString();

                        lsRoute.Add(route);
                    }
                    rdr.Close();
                    return lsRoute;
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
