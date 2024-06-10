// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Soul.Shop.Module.Catalog.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Entities;

namespace Soul.Shop.Module.Support.Abstractions.Entities;

public class Return
{
    public int ReturnID
    {
        get;
        set;
    }

    public int OrderID
    {
        get;
        set;
    }

    public int CustomerID
    {
        get;
        set;
    }

    public int ProductID
    {
        get;
        set;
    }

    public DateTime ReturnDate
    {
        get;
        set;
    }

    public int Quantity
    {
        get;
        set;
    }

    public string Reason
    {
        get;
        set;
    }

    public string Status
    {
        get;
        set;
    } // Navigation properties

    public Order Order { get; set; }
    public User Customer { get; set; }
    public Product Product { get; set; }
}
