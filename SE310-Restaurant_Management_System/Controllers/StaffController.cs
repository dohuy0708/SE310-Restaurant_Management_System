using Microsoft.AspNetCore.Mvc;

namespace SE310_Restaurant_Management_System.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
