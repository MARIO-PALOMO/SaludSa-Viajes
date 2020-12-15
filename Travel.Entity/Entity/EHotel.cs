using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Entity.Entity
{
    [DataContract]
    [Serializable]
    public class EHotel
    {
        [DataMember] public int Identificador { get; set; }
        [DataMember] public int IdHotel { get; set; }
        [DataMember] public string NombreHotel { get; set; }
        [DataMember] public string DescripcionHotel { get; set; }
        [DataMember] public string CiudadHotel { get; set; }
        [DataMember] public string TarifaHotel { get; set; }
        [DataMember] public string EmailHotel { get; set; }
        [DataMember] public string CargoAutorizadoHotel { get; set; }
        [DataMember] public int EstadoHotel { get; set; }
    }
}
