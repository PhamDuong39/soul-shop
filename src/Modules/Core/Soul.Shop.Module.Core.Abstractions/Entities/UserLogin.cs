﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class UserLogin : IdentityUserLogin<int>, IEntityWithTypedId<int>
{
    public UserLogin()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int Id { get; set; }

    [StringLength(450)] public string UnionId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}