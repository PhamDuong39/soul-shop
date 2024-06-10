// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Soul.Shop.Module.Support.Abstractions.Entities;

public class Discount
{
    public int DiscountID
    {
        get;
        set;
    }

    public string DiscountCode
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public decimal DiscountAmount
    {
        get;
        set;
    }

    public DateTime StartDate
    {
        get;
        set;
    }

    public DateTime EndDate
    {
        get;
        set;
    }

    public decimal MinOrderAmount
    {
        get;
        set;
    }

    public int MaxUses
    {
        get;
        set;
    }
}
