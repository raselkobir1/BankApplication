using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class ApplicationUser : IdentityUser<long>
    {
        public long? BankId { get; set; }
    }
}
