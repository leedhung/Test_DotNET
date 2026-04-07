using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopTech_Web.Data;
using ShopTech_Web.Models;

namespace ShopTech_Web.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var query = _context.Products.Where(p => p.IsDeleted == false);

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(p => p.Name.Contains(searchString) || p.Category.Contains(searchString));
        }

        ViewData["CurrentFilter"] = searchString;

        return View(await query.ToListAsync());
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            product.CreatedBy = User.Identity?.Name ?? "Admin";
            
            _context.Add(product);
            await _context.SaveChangesAsync(); 
            
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}