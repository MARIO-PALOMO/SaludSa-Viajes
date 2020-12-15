using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Model.External;

namespace Travel.Controller.External
{
    public class UserController
    {

        public static TokenViewModel getToken()
        {
            return UserModel.getToken();
        }       

        public static UsuarioViewModel getUser(TokenViewModel token, string username)
        {
            return UserModel.getUser(token, username);
        }

        public static List<UsuarioViewModel> getUserGroup(TokenViewModel token)
        {
            return UserModel.getUserGroup(token);
        }
    }
}
