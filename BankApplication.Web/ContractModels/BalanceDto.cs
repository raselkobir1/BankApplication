using System;

namespace BankApplication.Web.ContractModels
{
    public class BalanceDto
    {
        public double DepositeAmount { get; set; } = 0;
        public double WidthrownAmount { get; set; } = 0;
        public string TransactionType { get; set; }
        public string AccountNo { get; set; }
    }
}
