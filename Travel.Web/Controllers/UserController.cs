using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Common.ViewModels;
using Travel.Model.External;

namespace Travel.Web.Controllers
{
    public class UserController : ApiController
    {

        [HttpGet]
        public UsuarioViewModel getUser(string username)
        {
            var url = ConfigurationManager.AppSettings["Url"].ToString();
            var user = ConfigurationManager.AppSettings["Usuario"].ToString();
            var pass = ConfigurationManager.AppSettings["Clave"].ToString();
            var clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
            var applicationCode = ConfigurationManager.AppSettings["CodigoAplicacion"].ToString();

            return UserExternal.getUser(url, user, pass, clientId, applicationCode, username);
        }
    }
}
