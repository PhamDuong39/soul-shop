﻿using Soul.Shop.Module.Core.Abstractions.Models;
using System;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class UserAddress : EntityBase
{
    public int UserId { get; set; }

    public User User { get; set; }

    public int AddressId { get; set; }

    public Address Address { get; set; }

    public AddressType AddressType { get; set; }

    public DateTime? LastUsedOn { get; set; }

    public bool IsDeleted { get; set; }
}