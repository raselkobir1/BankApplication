using BankApplication.Web.ContractModels;
using BankApplication.Web.Models;
using EmailService;
using EmailService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DatabaseContext _DatabaseContext;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private UserManager<ApplicationUser> _UserManager { get; set; }
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private RoleManager<IdentityRole<long>> _RoleManager { get; set; }
        private readonly IEmailSender _EmailSender; 

        public AccountController(DatabaseContext databaseContext, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager, IEmailSender emailSender)
        {
            _DatabaseContext = databaseContext;
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor; 
            _UserManager = userManager;
            _RoleManager = roleManager;
            _EmailSender = emailSender;
        }
        //admin acccount create
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAccount(string email, string password)  
        {
            try
            {
                ApplicationUser applicationUser = await _UserManager.FindByEmailAsync(email);
                if (applicationUser != null)
                    return Ok("Already have an account for this email");

                  applicationUser = new ApplicationUser() { Email = email, UserName = email, BankId = 1, };
               
                var result = await _UserManager.CreateAsync(applicationUser, password);
                if (result.Succeeded)
                {
                    var token = await _UserManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var confirmationLink = $"{GetSiteBaseUrl()}/confirm-email?email={applicationUser.Email}&token={token}";
                    var message = new Message(new string[] { applicationUser.Email }, "Registration Confirmation link", confirmationLink); 
                    _EmailSender.SendEmail(message);

                    await RoleCreateIfNotExists();
                    _UserManager.AddToRoleAsync(applicationUser, Roles.Customer.ToString()).Wait();
                    _DatabaseContext.SaveChanges();
                    applicationUser = await _UserManager.FindByEmailAsync(email);

                    return Ok(new { appuser = applicationUser }); 
                }
               
                return Ok("Something want wrong...!!");
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var result = await _SignInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
            ApplicationUser user = await _UserManager.FindByEmailAsync(email);
            var role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();

            if (!await _UserManager.IsEmailConfirmedAsync(user))
                return Ok("Please confirm your email");

            return Ok(new { AppUserId = user.Id, role = role });
        }

        // customer applay for a bank account  
        [HttpPost]
        [Route("bankaccount-create")]
        public async Task <IActionResult> CustomerBankAccountCreate([FromBody] BankAccountDto bankAccountDto)   
        {
            try
            {
                ApplicationUser user = null;
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    user = await _UserManager.GetUserAsync(HttpContext.User);
                    Random rnd = new Random();
                    int accNumber = rnd.Next(100, 5000);  

                    var bankAccount = new BankAccount()
                    {
                         Id = bankAccountDto.Id,
                         ApplicationUserId = user.Id,
                         AccountNo = "Acc-"+accNumber.ToString(),
                         AccountStatus = false,
                         AccountType = bankAccountDto.AccountType,
                         OpeningBalance = bankAccountDto.OpeningBalance,
                    };

                    _DatabaseContext.Add(bankAccount);
                    _DatabaseContext.SaveChanges();
                    return Ok(new { bankaccount = bankAccount });
                }
                return Ok("User is not Verified");
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        [HttpPost]
        [Route("transaction")]
        public async Task<IActionResult> TransactionBalance([FromBody] BalanceDto balanceDto) 
        {
            try
            {
                ApplicationUser user = null;
                user = await _UserManager.GetUserAsync(HttpContext.User);
                var accounts = _DatabaseContext.BankAccounts.Where(a => a.ApplicationUserId == user.Id).ToList();

                var currentUserAccount = accounts.FirstOrDefault(a => a.AccountNo == balanceDto.AccountNo);
               

                if (balanceDto.TransactionType == "Deposite")
                {
                    var currentBalance = currentUserAccount.OpeningBalance += balanceDto.DepositeAmount;
                    var balance = new Balance()
                    {
                         BankAccountId = currentUserAccount.Id, 
                         DepositeAmount = balanceDto.DepositeAmount,
                         Id = 0,
                         TotalAmount = currentBalance,
                         TransactionDate =  DateTime.UtcNow,
                         AccountNo = balanceDto.AccountNo,
                         TransactionType = "Deposite"
                    };
                    _DatabaseContext.Add(balance);
                    _DatabaseContext.SaveChanges();
                }
                else
                {
                    // if opening balance <= widthdrown amount = return please deposite your account first, you have not enough balance for widthdrown.
                    //var currentBalance = currentUserAccount.OpeningBalance -= balanceDto.WidthrownAmount;
                    var accBalance = _DatabaseContext.Balances.FirstOrDefault(b => b.BankAccountId == currentUserAccount.Id);   
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
                    _DatabaseContext.Add(balance);
                    _DatabaseContext.SaveChanges();
                }
                return Ok();
            }
            catch (System.Exception)
            {

                throw;
            }
            
        }

        [HttpGet("get-accounts")]
        [Authorize(Roles = "Administrator")]
        public IActionResult CustomerBankAccounts()
        {
            try
            {
                var accounts = _DatabaseContext.BankAccounts.ToList();
                return Ok(new {accounts = accounts });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPut("active-account")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ActivationCustomerAccount(long accountid) 
        {
            var accounts = _DatabaseContext.BankAccounts.ToList();
            var account = accounts.Where(a => a.Id == accountid).FirstOrDefault();
            account.AccountStatus = true;

            _DatabaseContext.Update(account);
            _DatabaseContext.SaveChanges();

            return Ok("Customer account approved and Activated");
        }

        [HttpPut("inactive-account")]
        [Authorize(Roles = "Administrator")]
        public IActionResult InactivationCustomerAccount(long accountid) 
        {
            var accounts = _DatabaseContext.BankAccounts.ToList();
            var account = accounts.Where(a => a.Id == accountid).FirstOrDefault();
            account.AccountStatus = false;

            _DatabaseContext.Update(account);
            _DatabaseContext.SaveChanges();

            return Ok("Customer account Inactivated successfull");
        }
        [HttpGet]
        [Route("active-acc-list")]
        public async Task<IActionResult> GetLoginCustomerActiveAccount() 
        {
            try
            {
                ApplicationUser user = null;
                user = await _UserManager.GetUserAsync(HttpContext.User);
                var accounts = _DatabaseContext.BankAccounts.Where(a => a.ApplicationUserId == user.Id && a.AccountStatus == true).ToList();
                return Ok(new { acclist = accounts });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
    
        }

        [HttpGet]
        [Route("transaction-history")]
        public async Task<IActionResult> GetLoginCustomerTransactionHistoy() 
        {
            try
            {
                ApplicationUser user = null;
                user = await _UserManager.GetUserAsync(HttpContext.User);
                var accounts = _DatabaseContext.BankAccounts.Where(a => a.ApplicationUserId == user.Id && a.AccountStatus == true).ToList();
                var balanceStatement = _DatabaseContext.Balances.ToList();
                
                var finalList = (from a in accounts
                                 join bs in balanceStatement on a.Id equals bs.BankAccountId
                                 select new { AccType = a.AccountType, AccNo = bs.AccountNo, Deposite = bs.DepositeAmount, Widthdrown = bs.WidthrownAmount, Balance =bs.TotalAmount , Date=bs.TransactionDate })
                                .ToList(); 

                return Ok(new { transactions = finalList });  
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpPost]
        [Route("signout")]
        public async Task<IActionResult> SignOutAccount() 
        {
            try
            {
                await _SignInManager.SignOutAsync();
                return Ok("Sign out user");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("app-context")]
        public async Task<IActionResult> GetApplicationContext()
        {
            ApplicationUser user = null;
            string role = "";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _UserManager.GetUserAsync(HttpContext.User);
                role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();
            }
            return Ok(new { user = user, role = role });
        }


        private async Task RoleCreateIfNotExists()  
        {
            var adminRole = "Administrator";
            if (!await _RoleManager.RoleExistsAsync(adminRole))
            {
                var res = await _RoleManager.CreateAsync(new IdentityRole<long>(adminRole));
            }
            var roleName = "Customer";
            if (!await _RoleManager.RoleExistsAsync(roleName))
            {
                var res = await _RoleManager.CreateAsync(new IdentityRole<long>(roleName));
            }
        }

        private string GetSiteBaseUrl()
        {
            HttpRequest request = _HttpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }
    }
}
