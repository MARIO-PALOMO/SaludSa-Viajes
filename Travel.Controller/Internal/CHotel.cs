using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Entity.Entity;
using Travel.Model.Internal;

namespace Travel.Controller.Internal
{
    public class CHotel
    {
        public static List<EHotel> getHotels(string destination) {
            return MHotel.getHotels(destination);
        }
    }
}
