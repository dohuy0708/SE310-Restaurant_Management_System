using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public IActionResult MenuItem(int? id)
        {
            // Lấy danh sách SubCategories từ cơ sở dữ liệu
            var subCategories = db.SubCategories.AsNoTracking().ToList();

            // Thêm mục "Tất cả" vào đầu danh sách
            subCategories.Insert(0, new SubCategory
            {
                SubCategoryId = 0, // Đặt ID đặc biệt, ví dụ là 0, để đại diện cho "Tất cả"
                SubCategoryName = "Tất cả"
            });

            // Lấy tất cả menuItems
            var menuItems = db.MenuItems.AsNoTracking();
            ViewBag.Name = "Tất cả";

            // Nếu id không null và không phải là 0, lọc menuItems theo SubCategoryId
            if (id != null && id != 0)
            {
                menuItems = menuItems.Where(mi => mi.SubCategoryId == id);

                // Lấy tên SubCategory cho ViewBag.Name nếu có id hợp lệ
                var subCategory = subCategories.FirstOrDefault(sC => sC.SubCategoryId == id);
                if (subCategory != null)
                {
                    ViewBag.Name = subCategory.SubCategoryName;
                }
            }

            ViewBag.SubCategories = subCategories;
            return View(menuItems.ToList());
        }

        public IActionResult Tables(int? status)
        {
            // Lấy danh sách SubCategories từ cơ sở dữ liệu
            var subCategories = db.SubCategories.AsNoTracking().ToList();

            // Thêm mục "Tất cả" vào đầu danh sách
            subCategories.Insert(0, new SubCategory
            {
                SubCategoryId = 0, // Đặt ID đặc biệt, ví dụ là 0, để đại diện cho "Tất cả"
                SubCategoryName = "Tất cả"
            });
            ViewBag.SubCategories = subCategories;

            var tables = db.RestaurantTables.AsNoTracking();
            if(status!= null)
            {
                tables=tables.Where(t=>t.StatusId==status);
            }
            return View(tables.ToList());
        }
        public IActionResult GetMenuItems(int? id)
        {
            var menuItems = db.MenuItems.AsNoTracking();
            if (id != null && id != 0)
            {
                menuItems = menuItems.Where(mi => mi.SubCategoryId == id);
            }
                return PartialView("_MenuItemsPartial", menuItems.ToList());
        }

        public IActionResult BookingOrder()
        {
            var orders = db.BookingOrders.AsNoTracking().ToList();
            return View(orders);
        }
        public IActionResult Invoice()
        {
            return View();
        }
    }
}
