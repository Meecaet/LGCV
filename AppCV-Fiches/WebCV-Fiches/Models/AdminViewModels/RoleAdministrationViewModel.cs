using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Models.Admin;

namespace WebCV_Fiches.Models.AdminViewModels
{
    public class RoleAdministrationViewModel
    {
        public ApplicationRole Role { get; set; }
        public List<ApplicationUser> Users{ get; set; }
    }
}
