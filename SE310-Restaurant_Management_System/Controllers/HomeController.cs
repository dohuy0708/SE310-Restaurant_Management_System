using Microsoft.AspNetCore.Mvc;

namespace SE310_Restaurant_Management_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
