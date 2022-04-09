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
    }
}
