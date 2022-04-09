using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class BankAccount
    {
        public long Id { get; set; }
        public long ApplicationUserId { get; set; }
        public string AccountNo { get; set; }
        public double? OpeningBalance { get; set; }
        public bool AccountStatus { get; set; }
        public string AccountType { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
