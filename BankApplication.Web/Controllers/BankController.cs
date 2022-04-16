using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankApplication.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private UserManager<ApplicationUser> _UserManager { get; set; }
        private readonly IServiceManager _ServiceManager;
        public BankController(IServiceManager serviceManager, UserManager<ApplicationUser> userManager)
        {
            _UserManager = userManager;
            _ServiceManager = serviceManager;
        }


        // customer applay for a bank account  
        [HttpPost]
        [Route("bankaccount-create")]
        public async Task<IActionResult> CustomerBankAccountCreate([FromBody] BankAccountDto bankAccountDto)
        {
            try
            {
                var user = await GetLoggedInUserAsync();
                bankAccountDto.ApplicationUserId = user.Id;
                var id = _ServiceManager.BankAccountService.CreateBankAccount(bankAccountDto);
                return Ok(new { bankId = id });
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
                var user = await GetLoggedInUserAsync();
                _ServiceManager.BankAccountService.CreateTransaction(balanceDto, user.Id);
                return Ok("Transaction successfully");
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpGet("get-accounts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult CustomerBankAccounts(int pageNo, int pageSize) 
        {
            try
            {
                var accounts = _ServiceManager.BankAccountService.GetAllBankAccounts(false, pageNo, pageSize);
                return Ok(new { accounts = accounts });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("active-account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult ActivationCustomerAccount(long accountid)
        {
            _ServiceManager.BankAccountService.ActivationCustomerAccount(accountid);
            return Ok("Customer account approved and Activated");
        }

        [HttpPut("inactive-account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult InactivationCustomerAccount(long accountid)
        {
            _ServiceManager.BankAccountService.InActivationCustomerAccount(accountid);
            return Ok("Customer account Inactivated successfull");
        }
        [HttpGet]
        [Route("active-acc-list")]
        public async Task<IActionResult> GetLoginCustomerActiveAccount()
        {
            try
            {
                var user = await GetLoggedInUserAsync();
                var accounts = _ServiceManager.BankAccountService.GetCurrentCustomerActiveAccount(false, user.Id);
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
                var finalList = _ServiceManager.BankAccountService.GetCurrentCustomerTransactionHistoy(false, user.Id);
                return Ok(new { transactions = finalList });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private async Task<ApplicationUser> GetLoggedInUserAsync()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.PrimarySid);
            var loggedInUser = await _UserManager.FindByIdAsync(loggedInUserId);
            return loggedInUser;
        }
    }
}
