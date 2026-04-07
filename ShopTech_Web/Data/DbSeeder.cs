using Microsoft.AspNetCore.Identity;
using ShopTech_Web.Models;

namespace ShopTech_Web.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        var userManager = service.GetRequiredService<UserManager<AppUser>>();
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Tạo Role
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"=> ĐÃ TẠO ROLE: {roleName}");
            }
        }

        // 2. Tạo Admin
        var adminEmail = "admin@shoptech.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };

            // Tạo tài khoản với mật khẩu
            var result = await userManager.CreateAsync(newAdmin, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                Console.WriteLine("=> TẠO TÀI KHOẢN ADMIN THÀNH CÔNG!");
            }
            else
            {
                // NẾU CÓ LỖI, IN RA MÀN HÌNH ĐỂ TÌM THỦ PHẠM
                Console.WriteLine("=> LỖI TẠO ADMIN:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }
        
    }
    public static async Task SeedProductsAsync(IServiceProvider service)
    {
        // Lấy DbContext để nói chuyện với Database
        var context = service.GetRequiredService<AppDbContext>();

        // Kiểm tra xem trong DB đã có sản phẩm nào chưa
        if (!context.Products.Any())
        {
            var dummyProducts = new List<Product>
            {
                new Product { 
                    Name = "Bàn phím cơ K-Series Pro", 
                    Category = "Keyboard", 
                    Price = 149.99m, 
                    Stock = 50, 
                    ImageUrl = "https://images.unsplash.com/photo-1595225476474-87563907a212?w=500&q=80", 
                    CreatedBy = "System Admin" 
                },
                new Product { 
                    Name = "Tai nghe Acoustic Studio Max", 
                    Category = "Audio", 
                    Price = 549.00m, 
                    Stock = 15, 
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500&q=80", 
                    CreatedBy = "System Admin" 
                },
                new Product { 
                    Name = "Đồng hồ Pulse Chrono S", 
                    Category = "Wearables", 
                    Price = 399.00m, 
                    Stock = 120, 
                    ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=500&q=80", 
                    CreatedBy = "System Admin" 
                },
                new Product { 
                    Name = "Chuột gaming Logitech G Pro X", 
                    Category = "Mouse", 
                    Price = 129.50m, 
                    Stock = 8, 
                    ImageUrl = "https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?w=500&q=80", 
                    CreatedBy = "System Admin" 
                },
                new Product { 
                    Name = "Màn hình UltraSharp 27 inch", 
                    Category = "Monitor", 
                    Price = 450.00m, 
                    Stock = 0, // Cố tình để 0 để test nút SOLD OUT
                    ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=500&q=80", 
                    CreatedBy = "System Admin" 
                },
                new Product { 
                    Name = "Loa Vocalis One Wireless", 
                    Category = "Audio", 
                    Price = 299.00m, 
                    Stock = 45, 
                    ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500&q=80", 
                    CreatedBy = "System Admin" 
                }
            };

            // Thêm danh sách này vào Database
            context.Products.AddRange(dummyProducts);
            await context.SaveChangesAsync();
            
            Console.WriteLine("=> ĐÃ TẠO THÀNH CÔNG 6 SẢN PHẨM MẪU!");
        }
    }
}