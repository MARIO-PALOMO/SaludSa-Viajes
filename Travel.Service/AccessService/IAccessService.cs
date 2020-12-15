using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Travel.Entity.Entity;

namespace Travel.Service.AccessService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IAccessService" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IAccessService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
         UriTemplate = "externo/usuario/obtener/token")]
        TokenViewModel getToken();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "externo/usuario/listar/{username}")]
        UsuarioViewModel getUser(TokenViewModel token, string username);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "externo/usuario/listar/grupos")]
        List<UsuarioViewModel> getUserGroup(TokenViewModel token);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "externo/empresa/listar")]
        List<EmpresaViewModel> getCompany(TokenViewModel token);

        //LISTAR RUTAS DESDE BDD 
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "interno/viaje/listar/rutas")]
        List<ERoute> ConsultarRutas();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "interno/parametros/listar")]
        List<EParameter> getParameters();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "interno/hoteles/listar/{destination}")]
        List<EHotel> getHotels(string destination);
    }
}
