using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Entity.Core
{
    public class Balance
    {
        public long Id { get; set; }
        public long BankAccountId { get; set; }
        public string AccountNo { get; set; }
        public string TransactionType { get; set; }
        public double? DepositeAmount { get; set; }
        public double? WidthrownAmount { get; set; }
        public double? TotalAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
