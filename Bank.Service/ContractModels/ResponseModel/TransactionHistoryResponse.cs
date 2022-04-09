using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.ContractModels.ResponseModel
{
    public class TransactionHistoryResponse
    {
        public long Id { get; set; }
        public string AccType { get; set; } 
        public string AccNo { get; set; } 
        public double? Deposite { get; set; } 
        public double? Widthdrown { get; set; } 
        public double? Balance { get; set; } 
        public DateTime? Date { get; set; }   
    }
}
