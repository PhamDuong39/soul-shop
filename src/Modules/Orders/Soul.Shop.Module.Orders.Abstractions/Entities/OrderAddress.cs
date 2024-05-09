﻿using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.Entities;

public class OrderAddress : EntityBase
{
    public OrderAddress()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int OrderId { get; set; }

    [JsonIgnore] public Order Order { get; set; }

    public string ContactName { get; set; }

    public string Phone { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string City { get; set; }

    public string ZipCode { get; set; }

    public string Email { get; set; }

    public string Company { get; set; }

    public AddressType AddressType { get; set; }

    public int StateOrProvinceId { get; set; }

    public StateOrProvince StateOrProvince { get; set; }

    public int CountryId { get; set; }

    public Country Country { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}