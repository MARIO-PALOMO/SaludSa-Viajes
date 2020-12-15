using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Travel.Service.NoAccessService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "INoAccessService" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface INoAccessService
    {
        [OperationContract]
        void DoWork();
    }
}
