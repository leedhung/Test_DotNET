using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopTech_Web.Models;
using ShopTech_Web.ViewModels;

namespace ShopTech_Web.Controllers;

[Authorize(Roles = "Admin")] // Bắt buộc phải là Admin
public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            userViewModels.Add(new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Roles = roles
            });
        }

        return View(userViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (user.Email == "admin@shoptech.com") 
            {
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }
}