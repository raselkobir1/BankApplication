using Bank.Application;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.ResponseModel;
using Bank.Service.Interface;
using Bank.Utilities.EmailConfig;
using Bank.Utilities.EmailConfig.Models;
using Bank.Utilities.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly IEmailSender _EmailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        //private readonly ILoggerManager _loggerManager;
        //private readonly IMapper _mapper;
        public BankAccountService(IRepositoryManager repositoryManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor) 
        {
            _repositoryManager = repositoryManager;
            _EmailSender = emailSender;
            _HttpContextAccessor = httpContextAccessor; 
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


        public PaginatedData<BankAccResponse> GetAllBankAccounts(bool trackChanges, int pageNo, int pageSize,string searchValue, string selectedItem) 
        {

            IEnumerable<BankAccount> bankAccountList = new List<BankAccount>();
            bankAccountList = _repositoryManager.BankAccount.GetAllBankAccounts(trackChanges);

            var accountList = new List<BankAccResponse>();
            foreach (var bankAcc in bankAccountList)
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
                            select new BankAccResponse { UserName = u.UserName, AccountType = a.AccountType, AccountNo = a.AccountNo, AccountStatus = a.AccountStatus, OpeningBalance = a.OpeningBalance, Id = a.Id });
            

            if (selectedItem != null || searchValue != null)
            {
                if(selectedItem == "ActiveAccount")
                {
                    accounts = accounts.Where(x => x.AccountStatus == true);
                }
                else if (selectedItem == "InActiveAccount")
                {
                    accounts = accounts.Where(x => x.AccountStatus == false);
                }
                else if (selectedItem == "AccountNo" && searchValue != null)
                {
                    accounts = accounts.Where(x => x.AccountNo.Trim().ToLower().Contains(searchValue.ToLower().Trim()));
                }
                else if(selectedItem == "UserName" && searchValue != null)
                {
                    accounts = accounts.Where(x => x.UserName.ToLower().Trim().Contains(searchValue.ToLower().Trim()));
                }
            }

            var count = accounts.Count();

            var finalResult = accounts.OrderBy(x => x.UserName)
                             .Skip((pageNo - 1) * pageSize)
                             .Take(pageSize)
                             .Select(r => r)
                             .ToList();
           
            var responseList = new PaginatedData<BankAccResponse>(finalResult, pageNo, pageSize, count);
            return responseList; 
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

        public void InviteForUser(string email, string userType, long loginUserId)
        {
            if (userType == "Admin")
            {
                var invitation = new UserInvitation()
                {
                    Id = 0,
                    InvitedById = loginUserId,
                    Email = email,
                    InvitedOn = DateTime.UtcNow,
                    Code = Guid.NewGuid().ToString(),
                    Type = Entity.Enum.UserInvitationType.Admin,
                };
                _repositoryManager.UserInvitationRepsitory.CreateInvitation(invitation);
                _repositoryManager.SaveChange();
                SendInvitationEmail(email, invitation.Code);

            }
            else
            {
                var invitation = new UserInvitation()
                {
                    Id = 0,
                    InvitedById = loginUserId,
                    Email = email,
                    InvitedOn = DateTime.UtcNow,
                    Code = Guid.NewGuid().ToString(),
                    Type = Entity.Enum.UserInvitationType.Customer,
                };
                _repositoryManager.UserInvitationRepsitory.CreateInvitation(invitation);
                _repositoryManager.SaveChange();
                SendInvitationEmail(email, invitation.Code);
            }

        }

        private void SendInvitationEmail(string email, string code) 
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var acceptInvitationURL = $"{GetSiteBaseUrl()}/accept-invitation?email={email}&code={code}";
            var message = new Message(new string[] { email }, "Accept invitation link", acceptInvitationURL);
            _EmailSender.SendEmail(message);  
        }
        private string GetSiteBaseUrl()
        {
            HttpRequest request = _HttpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }
    }
}
