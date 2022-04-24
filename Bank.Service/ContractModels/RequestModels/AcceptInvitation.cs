using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels.RequestModels
{
    public class AcceptInvitation
    {
        public string Code { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string ConfirmPassword { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; } 
    }
}
