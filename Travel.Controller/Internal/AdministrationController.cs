using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Model.Internal;

namespace Travel.Controller.Internal
{
    public class AdministrationController
    {
        public static string generateToken()
        {
            return AdministrationModel.generateToken();
        }
    }
}
