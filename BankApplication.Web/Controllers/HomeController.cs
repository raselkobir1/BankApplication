using BankApplication.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly UserManager<ApplicationUser> _UserManager; 

        public HomeController(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) 
        {
            _SignInManager = signInManager;
            _HttpContextAccessor = httpContextAccessor;
            _UserManager = userManager;
        }
        public IActionResult Index()
        {
            ViewData["BaseUrl"] = $"{this.Request.Scheme}://{this.Request.Host}/{this.Request.PathBase}";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                    var data = await EmailConfirmed(email, token);
                    if (data.Item2 == true)
                    {
                        //await _SignInManager.SignInAsync(data.Item1, true);
                        HttpRequest request = _HttpContextAccessor.HttpContext.Request;
                        var redirect_link = $"{request.Scheme}://{request.Host}{request.PathBase}";
                        redirect_link = $"{redirect_link}/login";

                        ViewBag.REDIRECT_LINK = redirect_link;
                        return View(true);
                    }
                }
                return View(false);
            }
            catch (Exception e)
            {
                return View(false);
            }
        }


        private async Task<(ApplicationUser, bool)> EmailConfirmed(string email, string token)
        {
            ApplicationUser applicationUser = await _UserManager.FindByEmailAsync(email);
            var emailConfirmationStatus = await _UserManager.ConfirmEmailAsync(applicationUser, token);
            if (applicationUser == null && !emailConfirmationStatus.Succeeded)
                throw new Exception("Email confirmation wrong.");
            return (applicationUser, emailConfirmationStatus.Succeeded);
        }
    }
}
