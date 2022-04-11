using Bank.Application;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.ResponseModel;
using Bank.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Implementation
{
    internal sealed class BankAccountService : IBankAccountService
    {
        private readonly IRepositoryManager _repositoryManager;
        //private readonly ILoggerManager _loggerManager;
        //private readonly IMapper _mapper;
        public BankAccountService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public BankAccountDto CreateBankAccount(BankAccountDto bankAccountDto)
        {
            try
            {
                    Random rnd = new Random();
                    int accNumber = rnd.Next(100, 5000);

                    var bankAccount = new BankAccount()
                    {
                        Id = bankAccountDto.Id,
                        ApplicationUserId = bankAccountDto.ApplicationUserId,
                        AccountNo = "Acc-" + accNumber.ToString(),
                        AccountStatus = false,
                        AccountType = bankAccountDto.AccountType,
                        OpeningBalance = bankAccountDto.OpeningBalance,
                    };
                    _repositoryManager.BankAccount.CreateBankAccount(bankAccount);
                    _repositoryManager.SaveChange();

                bankAccountDto = new BankAccountDto()
                {
                    Id = bankAccount.Id,
                };
                    return bankAccountDto;
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
           
        }

        public void CreateTransaction(BalanceDto balanceDto, long loginUserId)  
        {
            try
            {
                var bankAccounts = _repositoryManager.BankAccount.GetAllBankAccounts(trackChanges: false).Where(a => a.ApplicationUserId == loginUserId).ToList(); ;
                var currentUserAccount = bankAccounts.FirstOrDefault(a => a.AccountNo == balanceDto.AccountNo);


                if (balanceDto.TransactionType == "Deposite")
                {
                    var currentBalance = currentUserAccount.OpeningBalance += balanceDto.DepositeAmount;
                    var balance = new Balance()
                    {
                        BankAccountId = currentUserAccount.Id,
                        DepositeAmount = balanceDto.DepositeAmount,
                        Id = 0,
                        TotalAmount = currentBalance,
                        TransactionDate = DateTime.UtcNow,
                        AccountNo = balanceDto.AccountNo,
                        TransactionType = "Deposite"
                    };
                    _repositoryManager.BankTransaction.CreateBankTransaction(balance);
                    _repositoryManager.SaveChange();
                }
                else
                {
                    var accBalance = _repositoryManager.BankTransaction.GetAllBalance(trackChanges:false).FirstOrDefault(b => b.BankAccountId == currentUserAccount.Id); ;
                    var totalBalance = accBalance.TotalAmount - balanceDto.WidthrownAmount;

                    var balance = new Balance()
                    {
                        BankAccountId = currentUserAccount.Id,
                        WidthrownAmount = balanceDto.WidthrownAmount,
                        Id = 0,
                        TotalAmount = totalBalance,
                        TransactionDate = DateTime.UtcNow,
                        AccountNo = balanceDto.AccountNo,
                        TransactionType = "Widthdrown"
                    };
                    _repositoryManager.BankTransaction.CreateBankTransaction(balance);
                    _repositoryManager.SaveChange();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<BankAccResponse> GetAllBankAccounts(bool trackChanges)
        {
            var bankAccounts = _repositoryManager.BankAccount.GetAllBankAccounts(trackChanges);
            var accountList = new List<BankAccResponse>();
            foreach (var bankAcc in bankAccounts)
            {
                var bankAccount = new BankAccResponse()
                {
                    Id = bankAcc.Id,
                    ApplicationUserId = bankAcc.ApplicationUserId,
                    AccountType = bankAcc.AccountType,
                    OpeningBalance = bankAcc.OpeningBalance,
                    AccountStatus = bankAcc.AccountStatus,
                    AccountNo = bankAcc.AccountNo
                };
                accountList.Add(bankAccount);
            }
            var users = _repositoryManager.AuthinticationRepository.GetApplicationUsers(false).ToList();
            var accounts = (from a in accountList
                            join u in users on a.ApplicationUserId equals u.Id
                            select new BankAccResponse { UserName = u.UserName, AccountType = a.AccountType, AccountNo = a.AccountNo, AccountStatus = a.AccountStatus, OpeningBalance = a.OpeningBalance, Id = a.Id })
                            .ToList();
            return accountList;
        }

        public IEnumerable<BalanceDto> GetAllBankBalance(bool trackChanges)
        {
            var balances = _repositoryManager.BankTransaction.GetAllBalance(trackChanges);
            var balanceDto = new List<BalanceDto>(); 
            foreach (var balanc in balances) 
            {
                var balance = new BalanceDto()
                {
                    AccountNo = balanc.AccountNo,
                    TransactionType = balanc.TransactionType,
                };
                balanceDto.Add(balance);
            }
            return balanceDto; //need balance responseDto
        }

        public IEnumerable<BankAccount> GetCurrentCustomerActiveAccount(bool trackChanges, long loginUserId)
        {
            var accounts = _repositoryManager.BankAccount.GetAllBankAccounts(false).Where(a => a.ApplicationUserId == loginUserId && a.AccountStatus == true).ToList();
            return accounts;

        }

        public IEnumerable<TransactionHistoryResponse> GetCurrentCustomerTransactionHistoy(bool trackChanges, long loginUserId)  
        {
            try
            {
                var accounts = _repositoryManager.BankAccount.GetAllBankAccounts(false).Where(a => a.ApplicationUserId == loginUserId && a.AccountStatus == true).ToList();
                var balanceStatement = _repositoryManager.BankTransaction.GetAllBalance(false).ToList();

                var finalList = (from a in accounts
                                 join bs in balanceStatement on a.Id equals bs.BankAccountId
                                 select new TransactionHistoryResponse { AccType = a.AccountType, AccNo = bs.AccountNo, Deposite = bs.DepositeAmount, Widthdrown = bs.WidthrownAmount, Balance = bs.TotalAmount, Date = bs.TransactionDate })
                                .ToList();
                return finalList;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void ActivationCustomerAccount(long accountid)
        {
            var accounts = _repositoryManager.BankAccount.GetAllBankAccounts(true).ToList();
            var account = accounts.Where(a => a.Id == accountid).FirstOrDefault();
            account.AccountStatus = true;

            _repositoryManager.BankAccount.UpdateBankAccount(account);
            _repositoryManager.SaveChange();

        }

        public void InActivationCustomerAccount(long accountid)
        {
            var accounts = _repositoryManager.BankAccount.GetAllBankAccounts(true).ToList();
            var account = accounts.Where(a => a.Id == accountid).FirstOrDefault();
            account.AccountStatus = false;

            _repositoryManager.BankAccount.UpdateBankAccount(account);
            _repositoryManager.SaveChange();
        }
    }
}
