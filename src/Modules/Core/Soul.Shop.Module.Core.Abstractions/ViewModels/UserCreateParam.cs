using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class UserCreateParam
{
    [Required]
    [RegularExpression(@"(\w[-\w.?]*@?[-\w.?]*){4,64}", ErrorMessage = "Not a valid username format")]
    public string UserName { get; set; }

    [Required] public string FullName { get; set; }

    [EmailAddress] public string Email { get; set; }

    [RegularExpression(@"[0-9-()（）]{4,32}", ErrorMessage = "Not a valid phone number format")]
    public string PhoneNumber { get; set; }


    public string Password { get; set; }


    public bool IsActive { get; set; }

    public IList<int> RoleIds { get; set; } = new List<int>();

    public string AdminRemark { get; set; }
}