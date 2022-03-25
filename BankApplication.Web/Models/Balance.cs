using System;

namespace BankApplication.Web.Models
{
    public class Balance
    {
        public long Id { get; set; }
        public long BankAccountId { get; set; }
        public double DepositeAmount { get; set; }
        public double WidthrownAmount { get; set; } 
        public double TotalAmount { get; set; }
        public DateTime? TransactionDate { get; set; }  
        public BankAccount BankAccount { get; set; }   
    }
}
