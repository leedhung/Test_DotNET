using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopTech_Web.Data;
using ShopTech_Web.Models;

namespace ShopTech_Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly AppDbContext _context;
    
    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.IsDeleted == false && p.IsSelling == true)
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();

        return View(products);
    }
}