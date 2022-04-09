using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels
{
    public class BankAccountDto
    {
        public long Id { get; set; }
        public long ApplicationUserId { get; set; }
        public double? OpeningBalance { get; set; }
        public string AccountType { get; set; }
        public string Brance { get; set; }
        public string Accholder { get; set; }
        public bool AccountStatus { get; set; }
        public string AccountNo { get; set; } 
    }
}
