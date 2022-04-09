using Bank.Service.ContractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Interface
{
    public interface IBankAccountService
    {
        IEnumerable<BankAccountDto> GetAllBankAccounts(bool trackChanges); 
        BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto);
        void CreateTransaction(BalanceDto balanceDto, long loginUserId);
        IEnumerable<BalanceDto> GetAllBankBalance(bool trackChanges);  

    }
}
