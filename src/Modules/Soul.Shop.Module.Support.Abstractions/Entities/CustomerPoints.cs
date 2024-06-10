// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Support.Abstractions.Entities;

public class CustomerPoints
{
    public int CustomerID
    {
        get;
        set;
    }

    public int TotalPoints
    {
        get;
        set;
    }

    public User Customer { get; set; }
}
