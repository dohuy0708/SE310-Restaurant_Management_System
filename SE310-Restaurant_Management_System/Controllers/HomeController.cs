using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;

namespace SE310_Restaurant_Management_System.Controllers
{
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
