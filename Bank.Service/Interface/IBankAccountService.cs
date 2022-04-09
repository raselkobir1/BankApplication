using Bank.Service.ContractModels;
using Bank.Service.ContractModels.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Interface
{
    public interface IBankAccountService
    {
        IEnumerable<BankAccResponse> GetAllBankAccounts(bool trackChanges);  
        BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto);
        void CreateTransaction(BalanceDto balanceDto, long loginUserId);
        IEnumerable<BalanceDto> GetAllBankBalance(bool trackChanges);  
        IEnumerable<TransactionHistoryResponse> GetCurrentCustomerTransactionHistoy(bool trackChanges, long loginUserId);    


    }
}
