using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;

namespace SE310_Restaurant_Management_System.Controllers.Cashier
{
    public class CashierController : Controller
    {
        QlnhaHangContext db = new QlnhaHangContext();


        public IActionResult MenuItem()
        {
            var menuItems = db.MenuItems.AsNoTracking().ToList();
            return View(menuItems);
        }
        public IActionResult EmptyTables()
        {
            
            var tables = db.RestaurantTables.AsNoTracking().Where(p => p.StatusId == 2).ToList();
            return View(tables);
        }
        public IActionResult OrderingTables()
        {

            var tables = db.RestaurantTables.AsNoTracking().Where(p => p.StatusId == 1).ToList();
            return View(tables);
        }
        public IActionResult BookingTables()
        {

            var tables = db.RestaurantTables.AsNoTracking().Where(p => p.StatusId == 3).ToList();
            return View(tables);
        }
        public IActionResult BookingOrder()
        {
            var orders = db.BookingOrders.AsNoTracking().ToList();
            return View(orders);
        }
    }
}
