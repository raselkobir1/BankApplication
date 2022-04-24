using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.ResponseModel;
using Bank.Utilities.Pagination;
using System.Collections.Generic;

namespace Bank.Service.Interface
{
    public interface IBankAccountService
    {
        PaginatedData<BankAccResponse> GetAllBankAccounts(bool trackChanges, int pageNo, int pageSize, string searchValue, string selectedItem);   
        BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto);
        void CreateTransaction(BalanceDto balanceDto, long loginUserId);
        IEnumerable<BalanceDto> GetAllBankBalance(bool trackChanges);  
        IEnumerable<TransactionHistoryResponse> GetCurrentCustomerTransactionHistoy(bool trackChanges, long loginUserId);    
        IEnumerable<BankAccount> GetCurrentCustomerActiveAccount(bool trackChanges, long loginUserId);
        void ActivationCustomerAccount(long accountid);
        void InActivationCustomerAccount(long accountid);
        void InviteForUser(string email, string userType, long loginUserId); 
    }
}
