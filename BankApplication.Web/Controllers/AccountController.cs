using BankApplication.Web.ContractModels;
using BankApplication.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
        public AccountController(DatabaseContext databaseContext, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _DatabaseContext = databaseContext;
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor; 
            _UserManager = userManager;
        }
        //admin acccount create
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAccount(string email, string password)  
        {
            try
            {
                var applicationUser = new ApplicationUser()
                {
                    Email = email,
                    UserName = email,
                    BankId = 1,
                };
                var role = "Admin";
                var result = await _UserManager.CreateAsync(applicationUser, password);
                _DatabaseContext.SaveChanges();
                return Ok("User create successfully");
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
                string role = "";
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    user = await _UserManager.GetUserAsync(HttpContext.User);
                    //role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();
                    var bankAccount = new BankAccount()
                    {
                         Id = bankAccountDto.Id,
                         ApplicationUserId = user.Id,
                         AccountNo = bankAccountDto.AccountNo,
                         AccountStatus = bankAccountDto.AccountStatus,
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
    }
}
