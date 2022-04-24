using Bank.Application;
using Bank.Entity.Core;
using Bank.Service.ContractModels;
using Bank.Service.ContractModels.RequestModels;
using Bank.Utilities.EmailConfig;
using Bank.Utilities.EmailConfig.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
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

        public IdentityServices(DatabaseContext databaseContext, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager, IEmailSender emailSender)
        {
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor;
            _UserManager = userManager;
            _RoleManager = roleManager;
            _EmailSender = emailSender;
            _DatabaseContext = databaseContext;
        }

        public async Task<ApplicationUser> RegisterAccount(string email, string password, bool isAdmin) 
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
                    await SendMailForVerification(applicationUser);
                    await RoleCreateIfNotExists();
                    await AddRoleToUser(isAdmin, applicationUser);
                   
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

        public async Task<Task> SendMailForVerification(ApplicationUser applicationUser)
        {
            var token = await _UserManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{GetSiteBaseUrl()}/confirm-email?email={applicationUser.Email}&token={token}";
            var message = new Message(new string[] { applicationUser.Email }, "Registration Confirmation link", confirmationLink);
            _EmailSender.SendEmail(message);
            return Task.CompletedTask;
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
        private Task AddRoleToUser(bool isAdmin, ApplicationUser applicationUser) 
        {
            if (isAdmin)
            {
                _UserManager.AddToRoleAsync(applicationUser, Roles.Administrator.ToString()).Wait();
            }
            else
            {
                _UserManager.AddToRoleAsync(applicationUser, Roles.Customer.ToString()).Wait();
            }
            _DatabaseContext.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task SignOutAccount()
        {
            try
            {
                await _SignInManager.SignOutAsync();
               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task ForgotPassword(string email)
        {
            try
            {
                var user = await _UserManager.FindByEmailAsync(email);
                if (user == null)
                    throw new Exception("Can't find user for this email");

                var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetPasswordUrl = $"{GetSiteBaseUrl()}/resetPassword?email={user.Email}&token={token}";
                var message = new Message(new string[] { user.Email }, "Password reset link", resetPasswordUrl);
                _EmailSender.SendEmail(message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task ResetPassword(ResetPassword model)
        {
            var user = await _UserManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new Exception("Can't find user for this email");

            model.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var resetPasswordResult = await _UserManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!resetPasswordResult.Succeeded)
            {
                throw new Exception("Can't reset your password");
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePassword model, long userId)
        {
            try
            {
                var user = await _UserManager.FindByIdAsync(userId.ToString());
                var result = await _UserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                    throw new Exception("User password can't change");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        private string GetSiteBaseUrl()
        {
            HttpRequest request = _HttpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

    }
}
