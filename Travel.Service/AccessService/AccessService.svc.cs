using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Travel.Controller.External;
using Travel.Controller.Internal;
using Travel.Entity.Entity;

namespace Travel.Service.AccessService
{
    [Serializable]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AccessService : IAccessService
    {
        public List<EmpresaViewModel> getCompany(TokenViewModel token)
        {
            return CompanyController.getCompany(token);
        }

        public TokenViewModel getToken()
        {
            return UserController.getToken();
        }

        public UsuarioViewModel getUser(TokenViewModel token, string username)
        {
            return UserController.getUser(token, username);
        }

        public List<UsuarioViewModel> getUserGroup(TokenViewModel token)
        {
            return UserController.getUserGroup(token);
        }

        public List<ERoute> ConsultarRutas()
        {
            return CRoute.ConsultarRutas();
        }

        public List<EParameter> getParameters()
        {
            return CParameter.getParameters();
        }

        public List<EHotel> getHotels(string destination) 
        {
            return CHotel.getHotels(destination);
        }
    }
}
