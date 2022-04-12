using Bank.Entity.Core;
using Bank.Service;
using Bank.Service.ContractModels.RequestModels;
using Bank.Utilities.LoggerConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private UserManager<ApplicationUser> _UserManager { get; set; }
        private readonly IConfiguration _Configuration;
        private readonly IWebHostEnvironment _Env;
        private readonly IdentityServices _IdentityServices; 
        private readonly ILoggerManager _LoggerServices;  

        public AccountController(ILoggerManager loggerServices,IdentityServices identityServices, IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _SignInManager = signInManager;
            _UserManager = userManager;
            _Configuration = configuration;
            _Env = env;
            _IdentityServices = identityServices;
            _LoggerServices = loggerServices;
        }
       
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAccount(string email, string password, bool role)  
        {
            try
            {
               var applicationUser =  await _IdentityServices.RegisterAccount(email, password, role);
                return Ok(new { appuser = applicationUser });
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(string email, bool isRemembeMe, string password)
        {
            var result = await _SignInManager.PasswordSignInAsync(email, password, isRemembeMe, lockoutOnFailure: false);
            ApplicationUser user = await _UserManager.FindByEmailAsync(email);
            var role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();

            if (!await _UserManager.IsEmailConfirmedAsync(user))
                return Ok("Please confirm your email");
            if (result.Succeeded)
            {
                return Ok(new
                {
                    AppUserId = user.Id,
                    userEmail = email,
                    role = role,
                    token = await CreateTokenAsync(user)
                });
            }
            return Ok("Your email or password wrong!");
        }

        [HttpPost]
        [Route("signout")]
        public async Task<IActionResult> SignOutAccount()
        {
            try
            {
                await _IdentityServices.SignOutAccount();
                return Ok("Sign out user");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _IdentityServices.ForgotPassword(email);
                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [Route("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
                return Ok(model);
            await _IdentityServices.ResetPassword(model);
            return Ok("Password reset successfull");
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            var user = await GetLoggedInUserAsync();
            if (user == null)
                return Ok("Invalid user");
            var result = await _IdentityServices.ChangePasswordAsync(model, user.Id);
            return Ok("your password successfully changed");
        }

        //--------------------This action method use for customer profile, currently not use (study purpose)------
        [HttpPost]
        [Route("register-form")]
        public IActionResult RegisterAccountForm(IFormFile Image)
        {
            var httpRequest = Request.Form;
            var postedFile = httpRequest.Files[0];
            string filename = postedFile.FileName;
            var physicalPath = _Env.ContentRootPath + "/wwwroot/Photos/" + filename;

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            //var formDataReg = regModel.Image;
            return Ok();
        }
        //private string UploadedFile(RegistrationDto model)
        //{
        //    string uniqueFileName = null;

        //    if (model.Image != null)
        //    {
        //        string uploadsFolder = Path.Combine(_Env.WebRootPath, "Photos");
        //        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            model.Image.CopyTo(fileStream);
        //        }
        //    }
        //    return uniqueFileName;
        //}

        //admin or customer acccount create

        [HttpGet("app-context")]
        public async Task<IActionResult> GetApplicationContext()
        {
            var user = await GetLoggedInUserAsync();
            string role = "";
            if (user != null)
            {
                role = (await _UserManager.GetRolesAsync(user)).FirstOrDefault();
            }
            return Ok(new { user = user, role = role });
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
    }
}



//if (HttpContext.User.Identity.IsAuthenticated)
//ApplicationUser user = null;
//user = await _UserManager.GetUserAsync(HttpContext.User);