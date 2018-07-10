using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.AccountApiModels
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RemeberMe { get; set; }
    }
}
