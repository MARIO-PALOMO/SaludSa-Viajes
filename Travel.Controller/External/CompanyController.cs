using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Model.External;

namespace Travel.Controller.External
{
    public class CompanyController
    {
        public static List<EmpresaViewModel> getCompany(TokenViewModel token)
        {
            return CompanyModel.getCompany(token);
        }
    }
}
