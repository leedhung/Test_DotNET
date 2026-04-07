using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShopTech_Web.Data;
using ShopTech_Web.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Cấu hình Identity và nới lỏng luật Mật khẩu
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
        options.Password.RequireDigit = false; 
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews(); 

var app = builder.Build(); // BẮT ĐẦU GIAI ĐOẠN APP

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
    await DbSeeder.SeedProductsAsync(scope.ServiceProvider);
}


// 2. CẤU HÌNH CÁC LỚP BẢO VỆ VÀ ĐƯỜNG DẪN (MIDDLEWARE)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// THÊM DÒNG NÀY: Phải có Xác thực (Authentication) rồi mới tới Phân quyền (Authorization)
app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();