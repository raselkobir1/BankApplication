using Microsoft.AspNetCore.Identity;

namespace BankApplication.Web.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public long? BankId { get; set; } 
    }
}
