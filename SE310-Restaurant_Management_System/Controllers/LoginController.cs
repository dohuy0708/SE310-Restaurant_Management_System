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
            return View();
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

                var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName) // Role của người dùng
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
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Login");
        }
    }
}