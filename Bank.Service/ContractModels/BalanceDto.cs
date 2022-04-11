using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels
{
    public class BalanceDto
    {
        public double DepositeAmount { get; set; } = 0;
        public double WidthrownAmount { get; set; } = 0;
        public string TransactionType { get; set; }
        public string AccountNo { get; set; }
    }
}
