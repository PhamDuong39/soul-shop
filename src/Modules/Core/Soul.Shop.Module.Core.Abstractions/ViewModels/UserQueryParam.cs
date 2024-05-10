using System;
using System.Collections.Generic;
using System.Text;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class UserQueryParam
{
    public string Name { get; set; }

    public string Email { get; set; }


    public bool? IsActive { get; set; }

    public string PhoneNumber { get; set; }


    public string Contact { get; set; }

    public IList<int> RoleIds { get; set; } = new List<int>();
}