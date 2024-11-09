using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;

namespace SE310_Restaurant_Management_System.Controllers.Cashier
{
    [Route("api/[action]")]
    [ApiController]
    public class APIController : Controller
    {
        private readonly QlnhaHangContext db;

        public APIController(QlnhaHangContext context)
        {
            db = context;
        }
        [HttpGet]
        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            var sanPham = db.MenuItems.ToList();
            return sanPham;
        }
        [HttpGet("{id}")]
        public IEnumerable<MenuItem> GetMenuItemsByCategory(int id)
        {
            var sanPham = db.MenuItems.Where(p=>p.SubCategoryId==id).ToList();
            return sanPham;
        }
    }
}
