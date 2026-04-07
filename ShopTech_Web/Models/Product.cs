using System.ComponentModel.DataAnnotations;

namespace ShopTech_Web.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;
    
    public string ImageUrl { get; set; } = string.Empty; 
        
    public int Stock { get; set; }

    public string CreatedBy { get; set; } = string.Empty; 
    public bool IsSelling { get; set; } = true; 
    public bool IsDeleted { get; set; } = false; 
}