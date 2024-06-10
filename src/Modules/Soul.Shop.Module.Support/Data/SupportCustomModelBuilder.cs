// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Support.Abstractions.Entities;

namespace Soul.Shop.Module.Support.Data;

public class SupportCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerPoints>().HasKey(cp
            => cp.CustomerID);
        modelBuilder.Entity<CustomerLevelAssignment>()
            .HasKey(cla => new { cla.CustomerID, cla.LevelID });
        modelBuilder.Entity<CustomerDiscountUsage>()
            .HasKey(cdu => cdu.UsageID); // Define foreign key
        modelBuilder.Entity<Return>()
            .HasOne(r => r.Order).WithMany().HasForeignKey(r
                => r.OrderID);
        modelBuilder.Entity<Return>()
            .HasOne(r => r.Customer).WithMany()
            .HasForeignKey(r => r.CustomerID);
        modelBuilder.Entity<Return>().HasOne(r =>
            r.Product).WithMany().HasForeignKey(r =>
            r.ProductID);
        modelBuilder.Entity<LoyaltyPoint>()
            .HasOne(lp => lp.Customer).WithMany()
            .HasForeignKey(lp => lp.CustomerID);
        modelBuilder.Entity<LoyaltyPoint>().HasOne(lp =>
            lp.Order).WithMany().HasForeignKey(lp =>
            lp.OrderID);
        modelBuilder.Entity<CustomerLevelAssignment>()
            .HasOne(cla => cla.Customer).WithMany()
            .HasForeignKey(cla => cla.CustomerID);
        modelBuilder.Entity<CustomerLevelAssignment>()
            .HasOne(cla => cla.CustomerLevel).WithMany()
            .HasForeignKey(cla => cla.LevelID);
        modelBuilder.Entity<CustomerDiscountUsage>()
            .HasOne(cdu => cdu.Customer).WithMany()
            .HasForeignKey(cdu => cdu.CustomerID);
        modelBuilder.Entity<CustomerDiscountUsage>()
            .HasOne(cdu => cdu.Discount).WithMany()
            .HasForeignKey(cdu => cdu.DiscountID);
        modelBuilder.Entity<CustomerDiscountUsage>()
            .HasOne(cdu => cdu.Order).WithMany()
            .HasForeignKey(cdu => cdu.OrderID);
    }
}
