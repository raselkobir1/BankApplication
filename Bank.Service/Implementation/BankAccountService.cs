using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Service.ContractModels;
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

        public IEnumerable<BankAccountDto> GetAllBankAccounts(bool trackChanges)
        {
            var bankAccounts = _repositoryManager.BankAccount.GetAllBankAccounts(trackChanges);

            var bankAccountsDto = new List<BankAccountDto>();
           
            foreach (var bankAcc in bankAccounts) 
            {
                var bankAccount = new BankAccountDto()
                {
                     Id = bankAcc.Id,
                     ApplicationUserId = bankAcc.ApplicationUserId,
                     AccountType = bankAcc.AccountType,
                     OpeningBalance = bankAcc.OpeningBalance,
                     AccountStatus = bankAcc.AccountStatus,
                     AccountNo = bankAcc.AccountNo
                };
                bankAccountsDto.Add(bankAccount);
            }
            return bankAccountsDto;
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
    }
}
