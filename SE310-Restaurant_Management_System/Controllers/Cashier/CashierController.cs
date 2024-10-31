using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;

namespace SE310_Restaurant_Management_System.Controllers.Cashier
{
    public class CashierController : Controller
    {
        private readonly QlnhaHangContext db;
        public CashierController(QlnhaHangContext context)
        {
            db = context;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
