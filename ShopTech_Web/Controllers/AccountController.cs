using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopTech_Web.Models;
using ShopTech_Web.ViewModels;

namespace ShopTech_Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    // "Tiêm" 2 người quản gia của Identity vào Controller
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // --- 1. GET: Hiển thị form Đăng ký ---
    [HttpGet]
    public IActionResult Register()
    {
        // Nếu đã đăng nhập rồi thì "mời" về thẳng Trang chủ
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }

    // --- 2. POST: Xử lý dữ liệu Đăng ký khi user bấm Submit ---
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Gói dữ liệu vào AppUser
            var user = new AppUser 
            { 
                UserName = model.Email, 
                Email = model.Email,
                FullName = model.FullName 
            };

            // Gọi UserManager để tạo tài khoản (Nó tự băm mật khẩu và lưu vào DB)
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Nếu tạo thành công, tự động đăng nhập luôn và cấp Cookie
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home"); // Đẩy về trang chủ
            }

            // Nếu lỗi (vd: Trùng email), báo lỗi ra giao diện
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        // Nếu đã đăng nhập rồi thì "mời" về thẳng Trang chủ
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return View();
    }

    // --- 4. POST: Xử lý Đăng nhập khi bấm Submit ---
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Gọi SignInManager để kiểm tra password trong MySQL
            // Tham số thứ 3: RememberMe (Ghi nhớ đăng nhập)
            // Tham số thứ 4: LockoutOnFailure (Khóa tài khoản nếu nhập sai nhiều lần - Tạm tắt để học)
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home"); // Chuyển về trang chủ nếu đúng pass
            }

            // Nếu sai pass hoặc email không tồn tại
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        }
        return View(model);
    }

    // --- 5. Đăng xuất (Thu hồi Cookie) ---
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}