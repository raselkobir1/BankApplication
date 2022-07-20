using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.RequestModels;
using Bank.Service.ContractModels.ResponseModel;
using Bank.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
                return Ok();
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpGet("get-accounts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public IActionResult CustomerBankAccounts(int pageNo = 1, int pageSize = 10, string searchValue = "", string selectedItem = "")
        {
            try
            {
                var accounts = _ServiceManager.BankAccountService.GetAllBankAccounts(false, pageNo, pageSize, searchValue, selectedItem);
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
        [HttpPost]
        [Route("invite-user")]
        public async Task<IActionResult> SendInvitation(string email, string userType)
        {
            var loginUser = await GetLoggedInUserAsync();
            _ServiceManager.BankAccountService.SendInvitation(email, userType, loginUser.Id);
            return Ok();
        }
        [HttpPut]
        [Route("accept-invitation")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitation acceptInvitation)
        {
            await _ServiceManager.BankAccountService.AcceptInvitation(acceptInvitation);
            return Ok();
        }
        [HttpPost]
        [Route("import-promo")]
        public IActionResult ImportPromocode([FromForm] IFormFile file)
        {
            var invalidPromoCode = new List<InvalidVoucherExcelModel>();

            using (var mStream = new MemoryStream())
            {
                try
                {
                    file.CopyTo(mStream);
                    invalidPromoCode = _ServiceManager.BankAccountService.ReadAndSavePromocode(mStream);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            if (invalidPromoCode.Count > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                byte[] invalidPromoCodeBytes;
                using (var excelPackage = new ExcelPackage())
                {
                    var workSheet = excelPackage.Workbook.Worksheets.Add("Invalid Vouchers");
                    var rowRange = workSheet.Cells["A1"].LoadFromCollection(invalidPromoCode, true);
                    rowRange.AutoFitColumns();
                    invalidPromoCodeBytes = excelPackage.GetAsByteArray();
                }
                return File(invalidPromoCodeBytes, "application/octet-stream", "invalid-vouvhers.xlsx");
            }
            return StatusCode(201, "Vouchars created");
        }

        private async Task<ApplicationUser> GetLoggedInUserAsync()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.PrimarySid);
            var loggedInUser = await _UserManager.FindByIdAsync(loggedInUserId);
            return loggedInUser;
        }

        [HttpPost("excel-download")]
        public IActionResult DownloadExcelFile()
        {
            var list = new List<UserInfo>()
                     {
                         new UserInfo { UserName = "catcher", Age = 18 },
                         new UserInfo { UserName = "james", Age = 20 },
                     };
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //way 1:

            //string excelName = $"UserList- {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
            //byte[] userListBytes;  
            //using (var excelPackage = new ExcelPackage())
            //{
            //    var workSheet = excelPackage.Workbook.Worksheets.Add("Users Information");
            //    var rowRange = workSheet.Cells["A1"].LoadFromCollection(list, true);
            //    rowRange.AutoFitColumns();
            //    userListBytes = excelPackage.GetAsByteArray();

            //}
            //return File(userListBytes, "application/octet-stream", excelName);

            ////way  2:
            //using (var package = new ExcelPackage(stream))
            //{
            //    var worksheet = package.Workbook.Worksheets.Add("sheet1");
            //    worksheet.Cells.LoadFromCollection(list, true);
            //    package.Save();
            //}
            //stream.Position = 0;
            //string excelName = $"UserList- {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
            //return File(stream, "application/octet-stream", excelName);
            ////return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            //way 3: trying
            byte[] fileData = null;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells.LoadFromCollection(list, true);
                //package.Save();

                using (var ms = new MemoryStream())
                {
                    package.SaveAs(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    fileData = ms.ToArray();
                }
            }
            string excelName = $"UserList- {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);


        }


    }
}

public class UserInfo
{
    public string UserName { get; set; }
    public int Age { get; set; }
}
