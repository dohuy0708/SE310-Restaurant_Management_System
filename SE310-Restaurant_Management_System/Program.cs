using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



//// Thêm dịch vụ xác thực dựa trên cookie


builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.HttpOnly = true; // Cookie chỉ có thể được truy cập thông qua HTTP, ngăn JavaScript truy cập cookie (tăng cường bảo mật).
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie sẽ hết hạn sau 30 phút không hoạt động.
        options.SlidingExpiration = true; // Tự động gia hạn thời gian sống của cookie nếu người dùng vẫn hoạt động.
        options.LoginPath = "/Login"; // URL của trang login.
        options.LogoutPath = "/Login/Logout"; // URL của trang logout.
        options.AccessDeniedPath = "/AccessDenied"; // URL hiển thị khi người dùng bị từ chối truy cập.
    });

var connectionString = builder.Configuration.GetConnectionString("QlnhaHangContext");

builder.Services.AddDbContext<QlnhaHangContext>(x => x.UseSqlServer(connectionString));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();