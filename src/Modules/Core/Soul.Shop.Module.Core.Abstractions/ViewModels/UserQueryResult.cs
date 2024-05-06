using System;
using System.Collections.Generic;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class UserQueryResult
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public string LastIpAddress { get; set; }

    public DateTime? LastLoginOn { get; set; }

    public DateTime? LastActivityOn { get; set; }

    public string AdminRemark { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IEnumerable<int> RoleIds { get; set; } = new List<int>();
}