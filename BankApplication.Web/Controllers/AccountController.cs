using BankApplication.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [Route("bank-create")]
        public async Task<IActionResult> CreateAccount(string email, string fullname, string password) 
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
        [Route("customer-create")]
        public async Task <IActionResult> BankCustomerAccountCreate([FromBody] BankAccount account)  
        {
            try
            {
                ApplicationUser user = await _UserManager.GetUserAsync(HttpContext.User);

                _DatabaseContext.Add(account);
                _DatabaseContext.SaveChanges();
            }
            catch (System.Exception e)
            {
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(string email, string password) 
        {
            var result = await _SignInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
            ApplicationUser user = await _UserManager.FindByEmailAsync(email);
            return Ok( new { Id= user.Id, FullName = user.UserName});
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
