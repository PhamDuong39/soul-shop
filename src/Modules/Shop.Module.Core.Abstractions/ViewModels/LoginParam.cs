using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Abstractions.ViewModels;

public class LoginParam
{
    [Required(ErrorMessage = "Please enter username, email, mobile phone number")]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
