// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Soul.Shop.Module.Support.Abstractions.Entities;

public class CustomerLevel
{
    public int LevelID
    {
        get;
        set;
    }

    public string LevelName
    {
        get;
        set;
    }

    public int MinPoints
    {
        get;
        set;
    }

    public decimal DiscountPercentage
    {
        get;
        set;
    }
}
