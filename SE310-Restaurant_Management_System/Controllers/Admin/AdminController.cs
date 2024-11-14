using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;


namespace SE310_Restaurant_Management_System.Controllers.Admin
{ 
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private QlnhaHangContext db = new QlnhaHangContext();

        public IActionResult Index()
        {
            var test = 2;
            return View();
        }

        [Route("quanlynhanvien")]
        public IActionResult QuanLyNhanVien(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listNV = db.Users.AsNoTracking().Where(x => x.RoleId != 1).Include(i => i.Role).OrderBy(x => x.UserId);
            PagedList<User> list = new PagedList<User>(listNV, pageNumber, pageSize);

            return View(list);
        }
    }
}