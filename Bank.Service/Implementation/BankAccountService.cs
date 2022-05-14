using Bank.Application;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Entity.Enum;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.RequestModels;
using Bank.Service.ContractModels.ResponseModel;
using Bank.Service.Interface;
using Bank.Utilities.EmailConfig;
using Bank.Utilities.EmailConfig.Models;
using Bank.Utilities.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service.Implementation
{
    internal sealed class BankAccountService : IBankAccountService
    {
        public BankAccountService(IRepositoryManager repositoryManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)  
        {
            _repositoryManager = repositoryManager;
            _EmailSender = emailSender;
            _HttpContextAccessor = httpContextAccessor; 
            _UserManager = userManager;
            _SignInManager = signInManager;
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

        public void SendInvitation(string email, string userType, long loginUserId) 
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
            var acceptInvitationURL = $"{GetSiteBaseUrl()}/accept-invitation?email={email}&code={code}";
            var message = new Message(new string[] { email }, "Accept invitation link", acceptInvitationURL);
            _EmailSender.SendEmail(message);  
        }

        public async Task AcceptInvitation(AcceptInvitation acceptInvitation)
        {
            var invitedUserList = _repositoryManager.UserInvitationRepsitory.GetInvitedUsers(trackChanges:false);
            var invitedUser = invitedUserList.Where(u=> u.Email == acceptInvitation.Email && u.Code == acceptInvitation.Code).FirstOrDefault(); 
            if (invitedUser != null)
            {
               var applicationUser = new ApplicationUser() { Email = acceptInvitation.Email, UserName = acceptInvitation.Email, BankId = 1, };
                var result = await _UserManager.CreateAsync(applicationUser, acceptInvitation.Password);
                //insert user table
                if (result.Succeeded)
                {
                    invitedUser.AcceptedById = applicationUser.Id;
                    invitedUser.AcceptedOn = DateTime.UtcNow;
                    _repositoryManager.UserInvitationRepsitory.UpdateInvitation(invitedUser);
                    await AddRoleToUser(invitedUser.Type.ToString(), applicationUser);
                    _repositoryManager.SaveChange();
                    await SendMailForVerification(applicationUser);
                }
            }
        }
        public async Task<Task> SendMailForVerification(ApplicationUser applicationUser)
        {
            var token = await _UserManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{GetSiteBaseUrl()}/confirm-email?email={applicationUser.Email}&token={token}";
            var message = new Message(new string[] { applicationUser.Email }, "Registration Confirmation link", confirmationLink);
            _EmailSender.SendEmail(message);
            return Task.CompletedTask;
        }
        private Task AddRoleToUser(string type, ApplicationUser applicationUser)
        {
            if (type == "1")
            {
                _UserManager.AddToRoleAsync(applicationUser, Roles.Administrator.ToString()).Wait();
            }
            else
            {
                _UserManager.AddToRoleAsync(applicationUser, Roles.Customer.ToString()).Wait();
            }
            _repositoryManager.SaveChange();
            return Task.CompletedTask;
        }

        private string GetSiteBaseUrl()
        {
            HttpRequest request = _HttpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

        public List<InvalidVoucherExcelModel> ReadAndSavePromocode(MemoryStream mStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
            var ValidPromoCode = new List<PromoCode>();
            var invalidPromoCode = new List<InvalidVoucherExcelModel>();

            try
            {
                using (var excelPackage = new ExcelPackage(mStream))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            AddToValidPromoCodeList(ValidPromoCode, worksheet, row);
                        }
                        catch (Exception ex)
                        {

                            AddToInvalidPromoCodeList(invalidPromoCode, worksheet, row);
                        }
                    }
                }
                _repositoryManager.PromoCodeRepository.CreatePromocode(ValidPromoCode);
                _repositoryManager.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return invalidPromoCode;
        }

        private void AddToInvalidPromoCodeList(List<InvalidVoucherExcelModel> invalidPromoCode, ExcelWorksheet worksheet, int row)
        {
            var invalidpromoCode = new InvalidVoucherExcelModel
            {
                Code = worksheet.Cells[row, 1].Value.ToString(),
                Months = worksheet.Cells[row, 3].Value.ToString(),
                IsMultiple = worksheet.Cells[row, 4].Value.ToString(),
                ExpiryOn = worksheet.Cells[row, 5].Value.ToString(),
                IsUsed = string.Empty,
                UsedBy = string.Empty
            };

            invalidPromoCode.Add(invalidpromoCode);
        }

        private void AddToValidPromoCodeList(List<PromoCode> validPromoCode, ExcelWorksheet worksheet, int row)
        {
            var promoCode = new PromoCode
            {
                Code = worksheet.Cells[row, 1].Value.ToString().Trim(),
                Months = int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),
                IsMultiple = worksheet.Cells[row, 3].Value.ToString().Trim() == "multiple_time",
                ExpiryOn = ConvertExcelDate(worksheet.Cells[row, 4]).Value,
                IsUsed = false,
                UsedBy = null
            };
            //check is existing voucher on db
            var existingPromocode = _repositoryManager.PromoCodeRepository.GetPromocodes(false).Where(x => x.Code == promoCode.Code);
            if (existingPromocode is null)
            {
                validPromoCode.Add(promoCode);
            }
            else
            {
                throw new Exception("Duplicate promocode detected");
            }
        }
        private DateTime? ConvertExcelDate(object date)
        {
            if (date != null && date.GetType() == typeof(DateTime))
            {
                return (DateTime)date;
            }
            return null;
        }

        private readonly IRepositoryManager _repositoryManager;
        private readonly IEmailSender _EmailSender;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        //private readonly ILoggerManager _loggerManager;
        //private readonly IMapper _mapper;
    }
}
