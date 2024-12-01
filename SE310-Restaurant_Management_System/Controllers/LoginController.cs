using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SE310_Restaurant_Management_System.Models;
using SE310_Restaurant_Management_System.ViewModels;
using System.Security.Claims;

namespace SE310_Restaurant_Management_System.Controllers
{
    public class LoginController : Controller
    {
        private QlnhaHangContext db = new QlnhaHangContext();

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Lấy vai trò từ claim của người dùng đã đăng nhập
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == "Admin")
                {
                    return RedirectToAction("MenuItem", "Admin");
                }
                else if (role == "Cashier")
                {
                    return RedirectToAction("MenuItem", "Cashier");
                }
                else if (role == "Staff")
                {
                    return RedirectToAction("Home", "WareHouse");
                }
            }

            return View(); // Nếu chưa đăng nhập, hiển thị trang Login
        }
    

        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel account)
        { // Kiểm tra thông tin đăng nhập từ CSDL
            var user = db.Users
                .Include(u => u.Role)  // Eager load liên kết Role
                .FirstOrDefault(u => u.Email == account.Email && u.Password == account.Password);

            if (user != null)
            {
                Console.WriteLine(user.Email + user.Role.RoleName);


                // Ví dụ về cách thêm claim khi đăng nhập người dùng

                var claims = new List<Claim>
{
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.Role.RoleName),
};

            

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal); // Tạo cookie xác thực

                // Điều hướng theo vai trò của người dùng
                if (user.Role.RoleId == 1)
                {
                    return RedirectToAction("Statistic", "Admin");
                }
                else if (user.Role.RoleId == 2)
                {
                    return RedirectToAction("MenuItem", "Cashier");
                }
                else if (user.Role.RoleId == 3)
                {
                    return RedirectToAction("Home", "WareHouse");
                }
            }

            ViewBag.Error = "*Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa cookie xác thực
            await HttpContext.SignOutAsync("CookieAuth");

            // Xóa cookie theo tên
            HttpContext.Response.Cookies.Delete(".AspNetCore.CookieAuth");

            // Điều hướng lại trang login
            return RedirectToAction("Login", "Login");
        }

    }
}