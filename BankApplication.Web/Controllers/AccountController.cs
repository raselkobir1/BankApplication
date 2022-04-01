using BankApplication.Web.ContractModels;
using BankApplication.Web.Models;
using EmailService;
using EmailService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        private readonly IConfiguration _Configuration;

        public AccountController(DatabaseContext databaseContext, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _DatabaseContext = databaseContext;
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor; 
            _UserManager = userManager;
            _RoleManager = roleManager;
            _EmailSender = emailSender;
            _Configuration = configuration;
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
                    _UserManager.AddToRoleAsync(applicationUser, Roles.Administrator.ToString()).Wait();
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

            return Ok(new 
            { 
                AppUserId = user.Id,
                role = role ,
                token = await CreateTokenAsync(user)
            });
        }

        // customer applay for a bank account  
        [HttpPost]
        [Route("bankaccount-create")]
        public async Task <IActionResult> CustomerBankAccountCreate([FromBody] BankAccountDto bankAccountDto)   
        {
            try
            {
                //if (HttpContext.User.Identity.IsAuthenticated)
                var user = await GetLoggedInUserAsync();
                if (user != null)
                {
                    //user = await _UserManager.GetUserAsync(HttpContext.User);
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
                //ApplicationUser user = null;
                //user = await _UserManager.GetUserAsync(HttpContext.User);
                var user = await GetLoggedInUserAsync();
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Administrator")]
        public IActionResult CustomerBankAccounts()
        {
            try
            {
                var accountList = _DatabaseContext.BankAccounts.ToList(); 
                var users = _DatabaseContext.Users.ToList(); 

                //var query = objEntities.Employee.Join(objEntities.Department, r => r.EmpId, p => p.EmpId, (r, p) => new { r.FirstName, r.LastName, p.DepartmentName });
                var accounts = (from a in accountList
                                 join u in users on a.ApplicationUserId equals u.Id
                                 select new { UserName = u.UserName , AccountType = a.AccountType, AccountNo = a.AccountNo, AccountStatus = a.AccountStatus, OpeningBalance = a.OpeningBalance, a.Id })
                                .ToList(); 


                return Ok(new {accounts = accounts }); 
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPut("active-account")] 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
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
                //ApplicationUser user = null;
                //user = await _UserManager.GetUserAsync(HttpContext.User);
                var user = await GetLoggedInUserAsync();
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
                var user = await GetLoggedInUserAsync();
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
            var user = await GetLoggedInUserAsync();
            string role = "";
            //if (HttpContext.User.Identity.IsAuthenticated)
            if (user != null)
            {
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

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var userRoles = await _UserManager.GetRolesAsync(user);
            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JwtSettings:Secret"]));

            var securityToken = new JwtSecurityToken(
                issuer: _Configuration["JwtSettings:ValidIssuer"],
                audience: _Configuration["JwtSettings:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        private async Task<ApplicationUser> GetLoggedInUserAsync()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.PrimarySid);
            var loggedInUser = await _UserManager.FindByIdAsync(loggedInUserId);
            return loggedInUser;
        }
        private string GetSiteBaseUrl()
        {
            HttpRequest request = _HttpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }
    }
}
