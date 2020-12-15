using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Travel.Entity.Entity
{
    [DataContract]
    [Serializable]
    public class ERoute
    {
        [DataMember] public int Identificador { get; set; }
        [DataMember] public int IdRuta { get; set; }
        [DataMember] public string NombreRuta { get; set; }
        [DataMember] public string DescripcionRuta { get; set; }
        [DataMember] public string OrigenRuta { get; set; }
        [DataMember] public string DestinoRuta { get; set; }
        [DataMember] public int EstadoRuta { get; set; }
    }
}
