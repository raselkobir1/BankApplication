using Bank.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class UserInvitation 
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; } 
        public UserInvitationType Type { get; set; }
        public DateTime? InvitedOn { get; set; }
        public long? InvitedById { get; set; }
        public DateTime? AcceptedOn { get; set; }
        public long? AcceptedById { get; set; }

        public ApplicationUser InvitedBy { get; set; }     
        public ApplicationUser AcceptedBy { get; set; }      
    }
}
