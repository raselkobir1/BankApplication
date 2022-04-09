using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.ResponseModel;
using System.Collections.Generic;

namespace Bank.Service.Interface
{
    public interface IBankAccountService
    {
        IEnumerable<BankAccResponse> GetAllBankAccounts(bool trackChanges);  
        BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto);
        void CreateTransaction(BalanceDto balanceDto, long loginUserId);
        IEnumerable<BalanceDto> GetAllBankBalance(bool trackChanges);  
        IEnumerable<TransactionHistoryResponse> GetCurrentCustomerTransactionHistoy(bool trackChanges, long loginUserId);    
        IEnumerable<BankAccount> GetCurrentCustomerActiveAccount(bool trackChanges, long loginUserId);
        void ActivationCustomerAccount(long accountid);
        void InActivationCustomerAccount(long accountid); 



    }
}
