using System.ComponentModel.DataAnnotations;

namespace ShopTech_Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } // Tính năng "Nhớ mật khẩu"
}