using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using System.Globalization;


namespace SE310_Restaurant_Management_System.Controllers.Admin { 

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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Staff", list);
            }

            return View(list);
        }
    }
}