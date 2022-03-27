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
                var applicationUser = new ApplicationUser() { Email = email, UserName = email, BankId = 1, };
               
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

                    return Ok("User create successfully with role");
                }
               
                return Ok("Something want wrong...!!");
            }
            catch (System.Exception e)
            {
                throw;
            }
            
        }

        // customer and account of custoerm create for bank 
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
                    //role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();
                    Random rnd = new Random();
                    int accNumber = rnd.Next(100, 201);  

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
                }
               
            }
            catch (System.Exception e)
            {
                throw;
            }
            return Ok("Bank account create successfully");
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(string email, string password) 
        {
            var result = await _SignInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
            ApplicationUser user = await _UserManager.FindByEmailAsync(email);

            if (!await _UserManager.IsEmailConfirmedAsync(user))
                return Ok("Please confirm your email");

            return Ok( new { AppUserId = user.Id});  
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
                var accounts = _DatabaseContext.BankAccounts;
                return Ok(new {accounts = accounts });
            }
            catch (Exception e)
            {
                throw;
            }
           
        }

        [HttpPut("active-account/{accountid})")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ActiveCustomerAccount(long accountid)
        {
            var accounts = _DatabaseContext.BankAccounts;
            var account = accounts.Where(a => a.Id == accountid).FirstOrDefault();
            account.AccountStatus = true;

            _DatabaseContext.Update(account);
            _DatabaseContext.SaveChanges();

            return Ok("Customer account approved and Activated");
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
