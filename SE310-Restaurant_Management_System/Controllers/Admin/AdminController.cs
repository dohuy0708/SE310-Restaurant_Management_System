using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;


namespace SE310_Restaurant_Management_System.Controllers.Admin

    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private QlnhaHangContext db = new QlnhaHangContext();

        [Route("MenuItem")]
        public IActionResult MenuItem(int? page, string? categoryName)
        {
            int pageSize = 100;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listMA = db.MenuItems
                            .AsNoTracking()
                            .Include(i => i.SubCategory.Category)
                            .OrderBy(x => x.MenuItemId);

            ViewBag.Name = "Tất cả";

            if (!string.IsNullOrEmpty(categoryName))
            {
                listMA = (IOrderedQueryable<MenuItem>)listMA.Where(x => x.SubCategory.Category.CategoryName == categoryName);
            }

            PagedList<MenuItem> list = new PagedList<MenuItem>(listMA, pageNumber, pageSize);

            var distinctCategories = db.MenuItems
                               .AsNoTracking()
                               .Select(i => i.SubCategory.Category.CategoryName)
                               .Distinct()
                               .ToList();

            var distinctSubCategories = db.MenuItems
                               .AsNoTracking()
                               .Include(i => i.SubCategory)
                               .Select(i => i.SubCategory.SubCategoryName)
                               .Distinct()
                               .ToList();

            ViewBag.Distinct = distinctCategories;
            ViewBag.DistinctSub = distinctSubCategories;

            return View(list);
        }

        [HttpGet]
        [Route("MenuItem/Add")]
        public IActionResult AddMenuItem()
        {
            return View();
        }

        [HttpPost]
        [Route("MenuItem/Add")]
        public IActionResult AddMenuItem([FromBody] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                db.MenuItems.Add(menuItem);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm món ăn thành công!" });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
        }

        [HttpGet]
        [Route("MenuItem/Edit")]
        public IActionResult EditMenuItem()
        {
            /*var item = db.MenuItems.Find(updatedItem.MenuItemId);
            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy món ăn!" });
            }

            if (ModelState.IsValid)
            {
                item.Name = updatedItem.Name;
                item.Price = updatedItem.Price;
                item.Description = updatedItem.Description;
                item.SubCategoryId = updatedItem.SubCategoryId;

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật món ăn thành công!" });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });*/
            return View();
        }

        [HttpPost]
        [Route("MenuItem/Edit")]
        public IActionResult EditMenuItem([FromBody] MenuItem updatedItem)
        {
            var item = db.MenuItems.Find(updatedItem.MenuItemId);
            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy món ăn!" });
            }

            if (ModelState.IsValid)
            {
                item.Name = updatedItem.Name;
                item.Price = updatedItem.Price;
                item.Description = updatedItem.Description;
                item.SubCategoryId = updatedItem.SubCategoryId;

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật món ăn thành công!" });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        [HttpPost]
        [Route("MenuItem/Delete")]
        public IActionResult DeleteMenuItem(int id)
        {
            var item = db.MenuItems.Find(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy món ăn!" });
            }

            db.MenuItems.Remove(item);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa món ăn thành công!" });
        }

        [Route("Combo")]
        public IActionResult Combo()
        {
            return View();
        }

        // Huy Hoàng

        [Route("Statistic")]
        public IActionResult Statistic()
        {
            return View();
        }

        //Hoàng Huy
        [Route("Staff")]
        public IActionResult Staff(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listNV = db.Users
                            .AsNoTracking()
                            .Where(x => x.RoleId != 1)
                            .Include(i => i.Role)
                            .OrderBy(x => x.UserId);
            PagedList<User> list = new PagedList<User>(listNV, pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Staff", list);
            }

            return View(list);
        }
    }
}