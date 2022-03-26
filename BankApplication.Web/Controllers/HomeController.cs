using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["BaseUrl"] = $"{this.Request.Scheme}://{this.Request.Host}/{this.Request.PathBase}";
            return View();
        }
    }
}
