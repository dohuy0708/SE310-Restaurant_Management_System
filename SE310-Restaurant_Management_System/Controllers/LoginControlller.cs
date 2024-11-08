using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;

namespace SE310_Restaurant_Management_System.Controllers
{
    public class LoginControlller : Controller
    {
        QlnhaHangContext db = new QlnhaHangContext();
        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Logout()
        {
            return View();
        }

    }
}
