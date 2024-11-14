using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
namespace SE310_Restaurant_Management_System.Controllers
{
    [Route("Home")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
       QlnhaHangContext db = new QlnhaHangContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            var menuItem = db.MenuItems.ToList();
            return View(menuItem);
        }


        

    
       

    }
}
