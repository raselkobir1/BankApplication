using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels.ResponseModel
{
    public class BankAccResponse
    {
        public long Id { get; set; } 
        public long ApplicationUserId { get; set; }  
        public string UserName { get; set; } 
        public string AccountType { get; set; } 
        public string AccountNo { get; set; } 
        public bool AccountStatus { get; set; } 
        public double? OpeningBalance { get; set; } 
    }
}

