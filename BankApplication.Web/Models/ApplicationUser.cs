using Microsoft.AspNetCore.Identity;

namespace BankApplication.Web.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; }
        public long BankId { get; set; } 
    }
}
