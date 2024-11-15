using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;
using SE310_Restaurant_Management_System.Models.ItemModels;

namespace SE310_Restaurant_Management_System.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private QlnhaHangContext db = new QlnhaHangContext();

        [HttpGet]
        public IEnumerable<Item> GetAllItems()
        {
            var item = (from p in db.MenuItems
                        select new Item
                        {
                            MenuItemId = p.MenuItemId,
                            Name = p.Name,
                            Image = p.Image,
                            Description = p.Description,
                            Price = p.Price,
                        }).ToList();
            return item;
        }

        [HttpGet("{maloai}")]
        public IEnumerable<Item> GetItemByCategory(int maloai)
        {
            var item = (from p in db.MenuItems
                        where p.SubCategoryId == maloai
                        select new Item
                        {
                            MenuItemId = p.MenuItemId,
                            Name = p.Name,
                            Image = p.Image,
                            Description = p.Description,
                            Price = p.Price,
                        }).ToList();
            return item;
        }
    }
}