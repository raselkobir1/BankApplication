using Bank.Application;
using Bank.Entity.Core;
using Bank.Service.ContractModels;
using EmailService;
using EmailService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Service
{
    public class IdentityServices
    {
        private readonly DatabaseContext _DatabaseContext;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private UserManager<ApplicationUser> _UserManager { get; set; }
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private RoleManager<IdentityRole<long>> _RoleManager { get; set; }
        private readonly IEmailSender _EmailSender;
        private readonly IConfiguration _Configuration;
        //private readonly IWebHostEnvironment _Env;

        public IdentityServices(DatabaseContext databaseContext, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _EmailSender = emailSender;
            _Configuration = configuration;
            _DatabaseContext = databaseContext;
        }

        public async Task<ApplicationUser> RegisterAccount(string email, string password)
        {
            try
            {
                ApplicationUser applicationUser = await _UserManager.FindByEmailAsync(email);
                if (applicationUser != null)
                    throw new Exception("Already have an account for this email");

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
                    return applicationUser;
                }
                return new ApplicationUser();
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
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
