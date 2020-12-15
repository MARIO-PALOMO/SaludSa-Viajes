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
    public class MHotel : ConecctionBD
    {
        public static List<EHotel> getHotels(string destination) {

            List<EHotel> lsHotel = new List<EHotel>();
            EHotel hotel;
            try
            {
                using (SqlConnection connection = ObtenerConecctionString())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM ConsultaListadoHoteles WHERE CiudadHotel = @destination", connection);
                    cmd.Parameters.AddWithValue("@destination", destination);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        hotel = new EHotel();

                        hotel.IdHotel = Convert.ToInt32(rdr["IdHotel"]);
                        hotel.NombreHotel = rdr["NombreHotel"].ToString();
                        hotel.DescripcionHotel = rdr["DescripcionHotel"].ToString();
                        hotel.CiudadHotel = rdr["CiudadHotel"].ToString();
                        hotel.TarifaHotel = rdr["TarifaHotel"].ToString();
                        hotel.EmailHotel = rdr["EmailHotel"].ToString();
                        hotel.CargoAutorizadoHotel = rdr["CargoAutorizadoHotel"].ToString();

                        lsHotel.Add(hotel);
                    }
                    rdr.Close();
                    return lsHotel;
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
