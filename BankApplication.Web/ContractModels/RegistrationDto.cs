using Microsoft.AspNetCore.Http;

namespace BankApplication.Web.ContractModels
{
    public class RegistrationDto
    { 
        public string Password { get; set; }
        public string Email { get; set; }

    }
}
