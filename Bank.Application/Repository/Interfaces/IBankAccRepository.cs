using Bank.Entity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Repository.Interfaces
{
    public interface IBankAccRepository 
    {
        IEnumerable<BankAccount> GetAllBankAccounts(bool trackChanges);  
        void CreateBankAccount(BankAccount bankAccount); 
        void UpdateBankAccount(BankAccount bankAccount);
    }
}
