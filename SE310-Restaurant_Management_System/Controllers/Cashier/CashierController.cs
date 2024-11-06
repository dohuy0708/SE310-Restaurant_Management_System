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

        public IActionResult Tables()
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

            // Lấy danh sách tất cả bàn (không lọc)
            var tables = db.RestaurantTables.AsNoTracking().ToList();
            return View(tables);
        }
        public IActionResult FilterTablesByStatus(int? status)
        {
            var tables = db.RestaurantTables.AsNoTracking();

            if (status != null)
            {
                tables = tables.Where(t => t.StatusId == status);
            }

            return PartialView("_TablesPartial", tables.ToList());
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
        [HttpGet]
        public IActionResult Booking()
        {
            // Lấy danh sách bàn trống từ cơ sở dữ liệu
            var tables = db.RestaurantTables.Where(p => p.StatusId == 2).ToList();

            // Truyền danh sách bàn vào ViewBag để sử dụng trong View
            ViewBag.Tables = tables;

            return View();
        }

        // POST: /Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Booking(BookingOrder bookingOrder)
        {
            if (ModelState.IsValid)
            {
                // Cập nhật trạng thái bàn thành đã được đặt (giả sử là StatusId = 1 cho bàn đã được đặt)
                var table = db.RestaurantTables.FirstOrDefault(t => t.TableId == bookingOrder.TableId);
                if (table != null)
                {
                    table.StatusId = 1; // Chuyển trạng thái bàn thành đã được đặt
                    db.SaveChanges();
                }

                // Thêm đơn đặt bàn vào cơ sở dữ liệu
                db.BookingOrders.Add(bookingOrder);
                db.SaveChanges();

                // Điều hướng về trang danh sách đặt bàn hoặc trang xác nhận
                return RedirectToAction("BookingOrder", "Cashier");
            }

            // Nếu có lỗi, truyền lại danh sách bàn trống và dữ liệu đã nhập vào form
            var tables = db.RestaurantTables.Where(p => p.StatusId == 2).ToList();
            ViewBag.Tables = tables; // Truyền lại danh sách bàn trống khi có lỗi
            return View(bookingOrder);
        }
        // hủy đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int bookingOrderId)
        {
            // Lấy đơn đặt bàn cần hủy
            var bookingOrder = db.BookingOrders.FirstOrDefault(b => b.BookingOrderId == bookingOrderId);
            if (bookingOrder != null)
            {
                // Cập nhật trạng thái bàn lại thành trống (StatusId = 2)
                var table = db.RestaurantTables.FirstOrDefault(t => t.TableId == bookingOrder.TableId);
                if (table != null)
                {
                    table.StatusId = 2; // Đặt lại trạng thái bàn thành "trống"
                    db.SaveChanges();
                }

                // Xóa đơn đặt bàn
                db.BookingOrders.Remove(bookingOrder);
                db.SaveChanges();
            }

            // Quay lại danh sách đơn đặt bàn
            return RedirectToAction("BookingOrder", "Cashier");
        }

        [HttpPost]
        public IActionResult ConfirmOrder([FromBody] OrderViewModel orderData)
        {
            try
            {
                // Kiểm tra xem bàn đã có hóa đơn chưa thanh toán chưa
                var existingInvoice = db.Invoices
                    .FirstOrDefault(i => i.TableId == orderData.TableId && !i.IsPaid);

                Invoice invoice;

                if (existingInvoice == null)
                {
                    // Tạo hóa đơn mới nếu chưa có
                    invoice = new Invoice
                    {
                        TableId = orderData.TableId,
                        InvoiceDate = DateTime.Now,
                        IsPaid = false,
                        TotalAmount = 0
                    };
                    db.Invoices.Add(invoice);

                    // Lưu lại Invoice để lấy InvoiceID
                    db.SaveChanges();
                }
                else
                {
                    invoice = existingInvoice;
                }

                // Thêm các món đã đặt vào chi tiết hóa đơn
                foreach (var item in orderData.Items)
                {
                    // Kiểm tra xem MenuItemID có tồn tại trong MenuItems không
                    var menuItem = db.MenuItems.Find(item.MenuItemId);
                    if (menuItem == null)
                    {
                        return Json(new { success = false, message = $"Món với ID {item.MenuItemId} không tồn tại." });
                    }

                    var invoiceItem = new InvoiceItem
                    {
                        InvoiceId = invoice.InvoiceId,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    db.InvoiceItems.Add(invoiceItem);
                }

                // Cập nhật tổng tiền
                invoice.TotalAmount = (invoice.TotalAmount ?? 0) +
                    orderData.Items.Sum(item => item.Price * item.Quantity);

                // Cập nhật trạng thái bàn thành "Đang phục vụ" (status = 3)
                var table = db.RestaurantTables.Find(orderData.TableId);
                if (table != null)
                {
                    table.StatusId = 3; // Đang phục vụ
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = $"An error occurred: {innerException}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public IActionResult Invoice()
        {
            var invoices = db.Invoices.AsNoTracking().ToList();
            return View(invoices);
        }
    }
}
