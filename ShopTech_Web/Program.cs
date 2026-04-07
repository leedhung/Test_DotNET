using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShopTech_Web.Data;
using ShopTech_Web.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- 1. MÁY QUÉT SỰ CỐ (Dùng để bắt bệnh trên Docker) ---
Console.WriteLine("========================================");
Console.WriteLine($"[DEBUG] ĐỊA CHỈ DB ĐANG DÙNG LÀ: {connectionString}");
Console.WriteLine("========================================");

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
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // --- 2. CƠ CHẾ AUTO-MIGRATION (Cực kỳ quan trọng cho Docker) ---
        // Lệnh này sẽ tự động chạy "dotnet ef database update" ngay khi Web vừa bật lên
        Console.WriteLine("[DEBUG] Đang tự động chạy Migration tạo bảng...");
        await context.Database.MigrateAsync();
        Console.WriteLine("[DEBUG] Migration tạo bảng thành công!");

        // Sau khi có bảng rồi mới bắt đầu bơm dữ liệu (Seed Data)
        await DbSeeder.SeedRolesAndAdminAsync(services);
        await DbSeeder.SeedProductsAsync(services);
        Console.WriteLine("[DEBUG] Bơm dữ liệu ban đầu thành công!");
    }
    catch (Exception ex)
    {
        // Bắt lỗi đỏ ra màn hình thay vì sập ngầm
        Console.WriteLine($"[ERROR FATAL] LỖI KẾT NỐI HOẶC TẠO BẢNG: {ex.Message}");
    }
}

// CẤU HÌNH CÁC LỚP BẢO VỆ VÀ ĐƯỜNG DẪN (MIDDLEWARE)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();