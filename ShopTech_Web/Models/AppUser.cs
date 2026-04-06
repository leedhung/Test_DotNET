using Microsoft.AspNetCore.Identity;

namespace ShopTech_Web.Models;

public class AppUser : IdentityUser 
{
    public string FullName { get; set; } = string.Empty;
}