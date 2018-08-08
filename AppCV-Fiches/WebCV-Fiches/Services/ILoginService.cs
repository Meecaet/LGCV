using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Models;
using WebCV_Fiches.Models.AccountApiModels;
using WebCV_Fiches.Models.AccountViewModels;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Services
{
    public interface ILoginService
    {
        ApiCredential Find(ApplicationUser user, LoginModel loginModel, string NomeComplet);
    }
}
