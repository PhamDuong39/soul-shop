// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Entities;

namespace Soul.Shop.Module.Support.Abstractions.Entities;

public class CustomerDiscountUsage
{
    public int UsageID { get; set; }
    public int CustomerID { get; set; }
    public int DiscountID { get; set; }
    public int OrderID { get; set; }

    public DateTime UsageDate
    {
        get;
        set;
    } // Navigation properties

    public User Customer { get; set; }
    public Discount Discount { get; set; }
    public Order Order { get; set; }
}
