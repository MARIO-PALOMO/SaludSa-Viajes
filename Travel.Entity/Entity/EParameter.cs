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
    public class EParameter
    {
        [DataMember] public int Identificador { get; set; }
        [DataMember] public int IdParametro { get; set; }
        [DataMember] public string NombreParametro { get; set; }
        [DataMember] public string ValorParametro { get; set; }
        [DataMember] public int EstadoParametro { get; set; }
    }
}
