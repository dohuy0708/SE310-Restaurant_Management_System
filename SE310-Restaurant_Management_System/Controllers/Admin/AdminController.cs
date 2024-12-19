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
using System.Security.Claims;
using System.Globalization;

using SE310_Restaurant_Management_System.ViewModels;

using Newtonsoft.Json;

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
            // Kiểm tra lỗi nếu không có file hình ảnh
            if (itemModel.ImagePath == null)
            {
                ModelState.AddModelError("ImagePath", "File hình ảnh là bắt buộc");
            }

            // Kiểm tra lỗi nếu không chọn SubCategoryId
            var subCategory = db.SubCategories.FirstOrDefault(sc => sc.SubCategoryId == itemModel.SubCategoryId);
            if (subCategory == null)
            {
                ModelState.AddModelError("SubCategoryId", "Loại món ăn không hợp lệ.");
            }

            // Nếu có lỗi trong ModelState
            if (!ModelState.IsValid)
            {
                // Truyền lại danh sách SubCategories vào ViewBag
                ViewBag.SubCategoryId = new SelectList(db.SubCategories.ToList(), "SubCategoryId", "SubCategoryName");
                return View(itemModel);
            }

            // Xử lý upload hình ảnh
            string newFileName = itemModel.ImagePath.FileName;
            string imageFullPath = _webHostEnvironment.WebRootPath + "/assets/images/foods/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                itemModel.ImagePath.CopyTo(stream);
            }

            // Tạo MenuItem mới
            MenuItem menuItem = new MenuItem()
            {
                Name = itemModel.Name,
                Description = itemModel.Description,
                Image = newFileName,
                Price = itemModel.Price,
                SubCategoryId = itemModel.SubCategoryId,
                SubCategory = subCategory
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

            // Nếu ModelState không hợp lệ
            if (!ModelState.IsValid)
            {
                // Truyền lại danh sách SubCategories vào ViewBag
                ViewBag.SubCategoryId = new SelectList(db.SubCategories.ToList(), "SubCategoryId", "SubCategoryName");

                // Truyền lại dữ liệu hình ảnh và ID
                ViewData["ItemId"] = item.MenuItemId;
                ViewData["ItemImage"] = item.Image;

                return View(model);
            }

            // Xử lý upload hình ảnh mới
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

                // Xóa hình ảnh cũ
                string oldImageFullPath = _webHostEnvironment.WebRootPath + "/assets/images/foods/" + item.Image;
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Cập nhật dữ liệu món ăn
            item.Name = model.Name;
            item.Description = model.Description;
            item.Image = newFileName;
            item.Price = model.Price;
            item.SubCategoryId = model.SubCategoryId;

            db.SaveChanges();
            return RedirectToAction("MenuItem", "Admin");
        }


        [Route("deleteitem")]

        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm bảo vệ CSRF
        public IActionResult DeleteItem(int id)
        {
            // Tìm món ăn cần xóa
            var item = db.MenuItems.Find(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy món ăn." });
            }

            // Xóa món ăn khỏi cơ sở dữ liệu
            db.MenuItems.Remove(item);
            db.SaveChanges();

            // Redirect về danh sách món ăn
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
                })
                .Distinct()
                .ToList();

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
        public IActionResult AddNewCombo(ComboViewModel combo, string selectedItemsInput)
        {
            // Kiểm tra lỗi nếu không có file hình ảnh
            if (combo.ImagePath == null)
            {
                ModelState.AddModelError("ImagePath", "File hình ảnh là bắt buộc");
            }

            // Kiểm tra nếu ComboName hoặc ComboPrice chưa nhập
            if (string.IsNullOrEmpty(combo.ComboName))
            {
                ModelState.AddModelError("ComboName", "Tên Combo không được bỏ trống");
            }

            if (combo.ComboPrice <= 0)
            {
                ModelState.AddModelError("ComboPrice", "Giá Combo phải lớn hơn 0");
            }

            // Nếu có lỗi trong ModelState
            if (!ModelState.IsValid)
            {
                return View(combo);
            }

            // Xử lý upload hình ảnh
            string newFileName = combo.ImagePath.FileName;
            string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/combos");

            // Kiểm tra và tạo thư mục nếu chưa có
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            string imageFullPath = Path.Combine(imageDirectory, newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                combo.ImagePath.CopyTo(stream);
            }

            // Chuyển đổi giá trị JSON thành danh sách các MenuItemId đã chọn
            combo.SelectedMenuItems = JsonConvert.DeserializeObject<List<int>>(selectedItemsInput);

            // Tạo Combo mới
            var newCombo = new Combo
            {
                ComboName = combo.ComboName,
                ComboPrice = combo.ComboPrice,
                Image = newFileName
            };

            // Lưu Combo vào cơ sở dữ liệu
            db.Combos.Add(newCombo);
            db.SaveChanges();

            // Lưu các món ăn trong combo
            foreach (var menuItemId in combo.SelectedMenuItems)
            {
                db.ComboItems.Add(new ComboItem
                {
                    ComboId = newCombo.ComboId,
                    MenuItemId = menuItemId
                });
            }

            db.SaveChanges();

            // Sau khi lưu, chuyển về màn hình chính
            return RedirectToAction("Combo", "Admin");
        }

        [Route("deletecombo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCombo(int comboId)
        {
            try
            {
                // Tìm combo cần xóa và lấy thông tin ComboItems liên quan
                var combo = db.Combos.Include(c => c.ComboItems).FirstOrDefault(c => c.ComboId == comboId);
                if (combo == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy combo." });
                }

                // Xóa tất cả ComboItems liên quan đến combo này
                if (combo.ComboItems.Any())
                {
                    db.ComboItems.RemoveRange(combo.ComboItems);  // Xóa các ComboItems liên quan
                }

                // Sau khi xóa các ComboItems, xóa Combo chính
                db.Combos.Remove(combo);
                db.SaveChanges();

                return RedirectToAction("Combo", "Admin");

            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi, bao gồm cả InnerException nếu có
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi khi xóa combo: " + errorMessage });
            }
        }




        [HttpGet]
        [Route("editcombo")]
        public IActionResult EditCombo(int id)
        {
            var combo = db.Combos.Include(c => c.ComboItems)
                                 .ThenInclude(ci => ci.MenuItem) // Lấy thông tin món ăn từ ComboItems
                                 .FirstOrDefault(c => c.ComboId == id);

            if (combo == null)
            {
                // Nếu không tìm thấy combo, bạn có thể trả về một thông báo lỗi hoặc redirect
                return NotFound();
            }
            
            // Tính giá gốc (OriginalPrice) bằng cách cộng giá các món ăn trong combo
            var originalPrice = combo.ComboItems.Sum(ci => ci.MenuItem.Price);

            var comboViewModel = new ComboViewModel
            {
                ComboId = combo.ComboId,
                ComboName = combo.ComboName,
                ComboPrice = combo.ComboPrice,
                SelectedMenuItems = combo.ComboItems.Select(ci => ci.MenuItemId).ToList(),
            };
            // Lấy danh sách tất cả các menu item để hiển thị
            ViewBag.AllMenuItems = db.MenuItems.ToList();
            ViewData["ComboImage"] = combo.Image;

            return View(comboViewModel);
        }


        [Route("editcombo")]
        [HttpPost]
        public IActionResult EditCombo(ComboViewModel combo, string selectedItemsInput)
        {
            try {
        var selectedMenuItemIds = JsonConvert.DeserializeObject<List<int>>(selectedItemsInput);

        var existingCombo = db.Combos.Include(c => c.ComboItems)
                                    .FirstOrDefault(c => c.ComboId == combo.ComboId);

        existingCombo.ComboName = combo.ComboName;
        existingCombo.ComboPrice = combo.ComboPrice;

        // Xóa comboitems cũ
        db.ComboItems.RemoveRange(existingCombo.ComboItems);

        // Thêm comboitems mới
        foreach (var menuItemId in selectedMenuItemIds)
        {
            db.ComboItems.Add(new ComboItem
            {
                ComboId = existingCombo.ComboId,
                MenuItemId = menuItemId,
            });
        }

        db.SaveChanges();
        return RedirectToAction("Combo");
    }
    catch (Exception ex) {
        return View(combo); 
    }
          
        }

        // Huy Hoàng

        [Route("Statistic")]
        public IActionResult Statistic(DateTime? date, int? month, int? year)
        {
            var model = new StatisticViewModel();

            if (date.HasValue)
            {
                model.RevenueByDay = new List<RevenueData> { GetRevenueByDate(date.Value) };
                model.ChartType = "day";
            }
            else if (month.HasValue && year.HasValue)
            {
                // Lấy dữ liệu cho từng ngày trong tháng
                model.RevenueByMonth = GetRevenueByDaysInMonth(year.Value, month.Value);
                model.ChartType = "month";
            }
            else if (year.HasValue)
            {
                model.RevenueByYear = GetRevenueByMonthsInYear(year.Value);
                model.ChartType = "year";
            }
            else
            {
                model.RevenueByDay = new List<RevenueData> { GetRevenueByDate(DateTime.Today) };

                model.RevenueByMonth = new List<RevenueData> { GetRevenueByMonth(DateTime.Today.Year, DateTime.Today.Month) };
                model.RevenueByYear = GetRevenueByMonthsInYear(DateTime.Today.Year);
                model.ChartType = "day";
            }

            // Thêm tổng doanh thu cho current period
            model.CurrentDayRevenue = GetRevenueByDate(DateTime.Today).TotalRevenue;
            model.CurrentMonthRevenue = GetRevenueByMonth(DateTime.Today.Year, DateTime.Today.Month).TotalRevenue;
            model.CurrentYearRevenue = GetRevenueByMonthsInYear(DateTime.Today.Year).Sum(x => x.TotalRevenue);

            ViewBag.ChartName = "Doanh thu hôm nay";
            ViewBag.TotalRevenue = "Tổng cộng: " + GetRevenueByDate(DateTime.Today).TotalRevenue.ToString("N0") + " VNĐ";
            return View(model);
        }

        [HttpGet]
        public IActionResult StatisticFilter(DateTime? date, int? month, int? year)
        {
            var model = new StatisticViewModel();
            ViewBag.ChartName = "";
            // Kiểm tra và xử lý theo ngày
            if (date.HasValue)
            {
                model.RevenueByDay = new List<RevenueData> { GetRevenueByDate(date.Value) };
                model.ChartType = "day";
                ViewBag.ChartName = "Doanh thu ngày " + date.Value.ToString("dd/MM/yyyy");
                ViewBag.TotalRevenue = "Tổng cộng: " + GetRevenueByDate(date.Value).TotalRevenue.ToString("N0") + " VNĐ";
            }
            // Kiểm tra và xử lý theo tháng
            else if (month.HasValue)
            {
                model.RevenueByMonth = GetRevenueByDaysInMonth(year.Value, month.Value);
                model.ChartType = "month";
                ViewBag.ChartName = "Doanh thu tháng " + month.Value.ToString() + '/' + year.Value.ToString();
                ViewBag.TotalRevenue = "Tổng cộng: " + GetRevenueByMonth(year.Value, month.Value).TotalRevenue.ToString("N0") + " VNĐ";
            }
            // Kiểm tra và xử lý theo năm
            else if (year.HasValue)
            {
                model.RevenueByYear = GetRevenueByMonthsInYear(year.Value);
                model.ChartType = "year";
                ViewBag.ChartName = "Doanh thu năm " + year.Value.ToString();
                var revenueData = GetRevenueByYear(year.Value);

                // Tính tổng doanh thu trong năm
                decimal totalRevenue = revenueData.Sum(r => r.TotalRevenue);

                // Gán vào ViewBag để hiển thị
                ViewBag.TotalRevenue = "Tổng cộng: " + totalRevenue.ToString("N0") + " VNĐ";
            }
            else
            {
                // Mặc định: thống kê theo ngày hôm nay
                model.RevenueByDay = new List<RevenueData> { GetRevenueByDate(DateTime.Today) };
                model.RevenueByMonth = new List<RevenueData> { GetRevenueByMonth(DateTime.Today.Year, DateTime.Today.Month) };
                model.RevenueByYear = GetRevenueByMonthsInYear(DateTime.Today.Year);
                model.ChartType = "day";
            }

            // Thêm tổng doanh thu cho period hiện tại
            model.CurrentDayRevenue = GetRevenueByDate(DateTime.Today).TotalRevenue;
            model.CurrentMonthRevenue = GetRevenueByMonth(DateTime.Today.Year, DateTime.Today.Month).TotalRevenue;
            model.CurrentYearRevenue = GetRevenueByMonthsInYear(DateTime.Today.Year).Sum(x => x.TotalRevenue);

            // Trả về một phần của trang (partial view) cho Ajax
            return PartialView("_chartPartial", model);
        }

        private List<RevenueData> GetRevenueByDaysInMonth(int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var dailyRevenue = db.Invoices
                .Where(i => i.InvoiceDate >= startDate &&
                           i.InvoiceDate <= endDate &&
                           i.IsPaid)
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new RevenueData
                {
                    Period = g.Key.Day.ToString("D2"), // Chỉ lấy ngày (dd) dưới dạng số
                    TotalRevenue = g.Sum(i => i.TotalAmount.Value)
                })
                .ToDictionary(x => x.Period);

            var allDays = new List<RevenueData>();
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(year, month, day);
                var period = currentDate.Day.ToString("D2"); // Lấy ngày (dd) dưới dạng số
                if (dailyRevenue.TryGetValue(period, out var revenue))
                {
                    allDays.Add(revenue);
                }
                else
                {
                    allDays.Add(new RevenueData
                    {
                        Period = period, // Chỉ hiển thị ngày
                        TotalRevenue = 0
                    });
                }
            }

            return allDays;
        }

        private List<RevenueData> GetRevenueByMonthsInYear(int year)
        {
            var monthlyRevenue = db.Invoices
                .Where(i => i.InvoiceDate.Year == year && i.IsPaid)
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new RevenueData
                {
                    Period = $"Tháng {g.Key.Month}",
                    TotalRevenue = g.Sum(i => i.TotalAmount.Value)
                })
                .ToDictionary(x => int.Parse(x.Period.Replace("Tháng ", "")));

            var allMonths = new List<RevenueData>();
            for (int month = 1; month <= 12; month++)
            {
                if (monthlyRevenue.TryGetValue(month, out var revenue))
                {
                    allMonths.Add(revenue);
                }
                else
                {
                    allMonths.Add(new RevenueData
                    {
                        Period = $"Tháng {month}",
                        TotalRevenue = 0
                    });
                }
            }

            return allMonths;
        }

        public RevenueData GetRevenueByDate(DateTime date)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu
            var revenue = db.Invoices
                .Where(i => i.InvoiceDate.Date == date && i.IsPaid)
                .Sum(i => (decimal?)i.TotalAmount) ?? 0;

            return new RevenueData
            {
                Period = date.ToString("dd/MM/yyyy"),
                TotalRevenue = revenue
            };
        }

        // Lấy doanh thu theo tháng và năm
        public RevenueData GetRevenueByMonth(int year, int month)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu
            var revenue = db.Invoices
                .Where(i => i.InvoiceDate.Year == year && i.InvoiceDate.Month == month && i.IsPaid)
                .Sum(i => (decimal?)i.TotalAmount) ?? 0;

            return new RevenueData
            {
                Period = $"{month} - {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}",
                TotalRevenue = revenue
            };
        }

        // Lấy doanh thu theo năm
        public List<RevenueData> GetRevenueByYear(int year)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu (không nhóm theo tháng ngay tại SQL)
            var allRevenue = db.Invoices
                .Where(i => i.InvoiceDate.Year == year && i.IsPaid)
                .ToList(); // Chuyển dữ liệu về client-side để xử lý

            var revenueByMonth = allRevenue
                .GroupBy(i => i.InvoiceDate.Month)
                .Select(g => new RevenueData
                {
                    Period = $"{g.Key} - {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key)}", // Lấy tên tháng
                    TotalRevenue = g.Sum(i => (decimal?)i.TotalAmount) ?? 0
                })
                .OrderBy(r => r.Period) // Sắp xếp theo tên tháng
                .ToList();

            return revenueByMonth;
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

            return View(list);
        }

        [HttpGet]
        [Route("CreateStaff")]
        public IActionResult CreateStaff()
        {
            var roles = new List<object>
    {
        new { RoleId = 2, RoleName = "Cashier" },
        new { RoleId = 3, RoleName = "Staff" }
    };

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [Route("CreateStaff")]
        public async Task<IActionResult> CreateStaff([FromBody] User user)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await db.Users
                                       .Where(u => u.Email == user.Email)
                                       .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return Json(new { success = false, message = "Email đã tồn tại." });
            }

            // Tạo người dùng mới
            var newUser = new User
            {
                Username = user.Username,
                Password = user.Password,
                RoleId = user.RoleId,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber
            };

            // Thêm người dùng vào cơ sở dữ liệu
            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            // Trả về JSON thông báo thành công
            return Json(new { success = true, message = "Thêm nhân viên thành công." });
        }

        [HttpGet]
        [Route("EditStaff/{id}")]
        public async Task<IActionResult> EditStaff(int id)
        {
            var user = await db.Users
                                .Include(u => u.Role) // Include Role data if needed
                                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new List<object>
    {
        new { RoleId = 2, RoleName = "Cashier" },
        new { RoleId = 3, RoleName = "Staff" }
    };

            return View(user); // Passing the user to the view for editing
        }

        [HttpPost]
        [Route("EditStaff/{id}")]
        public async Task<IActionResult> EditStaff(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest();
            }

            if (id != updatedUser.UserId)
            {
                return BadRequest();
            }

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.RoleId = updatedUser.RoleId;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.PhoneNumber = updatedUser.PhoneNumber;

            // Lưu thay đổi
            await db.SaveChangesAsync();

            // Log response to check
            var response = new { success = true, message = "Cập nhật nhân viên thành công." };

            return Json(response);
        }

        [HttpPost]
        [Route("DeleteStaff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Remove the user from the database
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa nhân viên thành công." });
        }

        [HttpGet]
        [Route("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Dữ liệu không hợp lệ.");
                return View(model);
            }

            try
            {
                // Lấy email của người dùng từ Claims
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("Không thể xác định email người dùng.");
                    ModelState.AddModelError(string.Empty, "*Không thể xác định email người dùng.");
                    return View(model);
                }

                // Tìm người dùng dựa trên email
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    Console.WriteLine("Người dùng không tồn tại.");
                    ModelState.AddModelError(string.Empty, "*Người dùng không tồn tại.");
                    return View(model);
                }

                // Kiểm tra mật khẩu hiện tại
                if (user.Password != model.CurrentPassword)
                {
                    Console.WriteLine("Mật khẩu hiện tại không đúng.");
                    ModelState.AddModelError(string.Empty, "*Mật khẩu hiện tại không đúng.");
                    return View(model);
                }

                // Kiểm tra xác nhận mật khẩu mới
                if (model.NewPassword != model.ConfirmPassword)
                {
                    Console.WriteLine("Xác nhận mật khẩu không khớp.");
                    ModelState.AddModelError(string.Empty, "*Xác nhận mật khẩu mới không khớp.");
                    return View(model);
                }

                // Cập nhật mật khẩu mới
                user.Password = model.NewPassword; // Không mã hóa mật khẩu (không an toàn, chỉ dùng thử nghiệm)
                Console.WriteLine($"Mật khẩu mới được cập nhật: {user.Password}");

                // Lưu thay đổi
                await db.SaveChangesAsync();

                // Hiển thị thông báo thành công
                ViewBag.Success = "Đổi mật khẩu thành công!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình đổi mật khẩu: {ex.Message}");
                // Hiển thị thông báo lỗi
                ModelState.AddModelError(string.Empty, "*Đã xảy ra lỗi trong quá trình đổi mật khẩu.");
            }

            return View(model);
        }
    }
}