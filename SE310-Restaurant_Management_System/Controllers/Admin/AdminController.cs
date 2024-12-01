using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using SE310_Restaurant_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SE310_Restaurant_Management_System.Controllers.Admin
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private QlnhaHangContext db = new QlnhaHangContext();
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(QlnhaHangContext db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this._webHostEnvironment = webHostEnvironment;
        }

        [Route("menuitem")]
        [HttpGet]
        public IActionResult MenuItem(int? page, int? typeId, string searchTerm)
        {
            int pageSize = 100;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = db.MenuItems.AsNoTracking().Include(i => i.SubCategory.Category).AsQueryable();

            ViewBag.Name = "Tất cả";

            if (typeId.HasValue)
            {
                listItem = listItem.Where(i => i.SubCategory.CategoryId == typeId.Value);

                var selectedType = listItem.FirstOrDefault()?.SubCategory.Category.CategoryName;

                if (selectedType != null)
                {
                    ViewBag.Name = selectedType;
                }
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                listItem = listItem.Where(i => i.Name.Contains(searchTerm));
            }

            var item = listItem.ToList();
            PagedList<MenuItem> list = new PagedList<MenuItem>(item, pageNumber, pageSize);

            ViewBag.SearchTerm = searchTerm;

            return View(list);
        }

        [Route("addnewitem")]
        [HttpGet]
        public IActionResult AddNewItem()
        {
            ViewBag.SubCategoryId = new SelectList(db.SubCategories.ToList(), "SubCategoryId", "SubCategoryName");

            return View();
        }

        [Route("addnewitem")]
        [HttpPost]
        public IActionResult AddNewItem(MenuItemModel itemModel)
        {
            if (itemModel.ImagePath == null)
            {
                ModelState.AddModelError("ImagePath", "File hình ảnh là bắt buộc");
            }

            if (!ModelState.IsValid)
            {
                return View(itemModel);
            }

            string newFileName = itemModel.ImagePath.FileName;

            string imageFullPath = _webHostEnvironment.WebRootPath + "/assets/images/foods/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                itemModel.ImagePath.CopyTo(stream);
            }

            MenuItem menuItem = new MenuItem()
            {
                Name = itemModel.Name,
                Description = itemModel.Description,
                Image = newFileName,
                Price = itemModel.Price,
                SubCategoryId = itemModel.SubCategoryId,
            };

            db.MenuItems.Add(menuItem);
            db.SaveChanges();

            return RedirectToAction("MenuItem", "Admin");
        }

        [Route("edititem")]
        [HttpGet]
        public IActionResult EditItem(int id)
        {
            ViewBag.SubCategoryId = new SelectList(db.SubCategories.ToList(), "SubCategoryId", "SubCategoryName");

            var item = db.MenuItems.Find(id);

            if (item == null)
            {
                return RedirectToAction("MenuItem", "Admin");
            }

            var model = new MenuItemModel()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                SubCategoryId = item.SubCategoryId,
            };

            ViewData["ItemId"] = item.MenuItemId;
            ViewData["ItemImage"] = item.Image;

            return View(model);
        }

        [Route("edititem")]
        [HttpPost]
        public IActionResult EditItem([FromForm] int id, MenuItemModel model)
        {
            var item = db.MenuItems.Find(id);

            if (item == null)
            {
                return RedirectToAction("MenuItem", "Admin");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ItemId"] = item.MenuItemId;
                ViewData["ItemImage"] = item.Image;

                return View(model);
            }

            string newFileName = item.Image;
            if (model.ImagePath != null)
            {
                newFileName = model.ImagePath.FileName;

                string folder = "/assets/images/foods/";

                string imageFullPath = _webHostEnvironment.WebRootPath + folder + newFileName;

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    model.ImagePath.CopyTo(stream);
                }

                string oldIamgeFullPath = _webHostEnvironment.WebRootPath + "/assets/images/foods/" + item.Image;
                System.IO.File.Delete(oldIamgeFullPath);
            }

            item.Name = model.Name;
            item.Description = model.Description;
            item.Image = newFileName;
            item.Price = model.Price;
            item.SubCategoryId = model.SubCategoryId;

            db.SaveChanges();
            return RedirectToAction("MenuItem", "Admin");
        }

        [HttpGet]
        [Route("deleteitem")]
        public IActionResult DeleteItem(int id)
        {
            var item = db.MenuItems.Find(id);
            if (item == null)
            {
                return RedirectToAction("MenuItem", "Admin");
            }
            string imageFullPath = _webHostEnvironment.WebRootPath + "/assets/images/foods/" + item.Image;
            System.IO.File.Delete(imageFullPath);

            db.MenuItems.Remove(item);
            db.SaveChanges(true);

            return RedirectToAction("MenuItem", "Admin");
        }

        [Route("combo")]
        public IActionResult Combo()
        {
            var combo = db.Combos.ToList();
            return View(combo);
        }

        [Route("getmenuitems")]
        [HttpGet]
        public IActionResult GetMenuItems()
        {
            var menuItems = db.MenuItems
                .Select(m => new MenuItemViewModel
                {
                    MenuItemID = m.MenuItemId,
                    Name = m.Name,
                    Image = m.Image,
                    Price = m.Price,
                    IsSelected = false
                }).ToList();

            return PartialView("_MenuItemsPopup", menuItems);
        }

        [Route("savecomboitems")]
        [HttpPost]
        public IActionResult SaveComboItems(int comboId, List<int> MenuItemIds)
        {
            foreach (var menuItemId in MenuItemIds)
            {
                db.ComboItems.Add(new ComboItem
                {
                    ComboId = comboId,
                    MenuItemId = menuItemId,
                });
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        [Route("getselectedmenuitems")]
        [HttpPost]
        public IActionResult GetSelectedMenuItems(List<int> selectedItemIds)
        {
            // Lọc món ăn theo ID đã chọn
            var selectedMenuItems = db.MenuItems
                .Where(m => selectedItemIds.Contains(m.MenuItemId)) // Lọc món ăn theo các ID đã chọn
                .Select(m => new MenuItemViewModel
                {
                    MenuItemID = m.MenuItemId,
                    Name = m.Name,
                    Price = m.Price
                }).ToList();

            // Trả về partial view chứa danh sách món ăn đã chọn
            return PartialView("_SelectedMenuItems", selectedMenuItems);
        }

        [Route("addnewcombo")]
        [HttpGet]
        public IActionResult AddNewCombo()
        {
            return View();
        }

        [Route("addnewcombo")]
        [HttpPost]
        public IActionResult AddNewCombo(ComboViewModel combo)
        {
            if (ModelState.IsValid)
            {
                // Lưu combo vào cơ sở dữ liệu
                var newCombo = new Combo
                {
                    ComboName = combo.TenCombo,
                    ComboPrice = combo.GiaCombo,
                };

                db.Combos.Add(newCombo);
                db.SaveChanges();

                // Lưu các món ăn trong combo
                foreach (var item in combo.SelectedMenuItems)
                {
                    var comboItem = new ComboItem
                    {
                        ComboId = newCombo.ComboId,
                        MenuItemId = item.MenuItemID,
                    };
                    db.ComboItems.Add(comboItem);
                }

                db.SaveChanges();

                // Sau khi lưu, chuyển về màn hình chính
                return RedirectToAction("combo");
            }

            // Nếu model không hợp lệ, trở lại màn hình tạo combo
            return View("AddCombo", combo);
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