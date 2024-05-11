﻿using Soul.Shop.Module.ShoppingCart.Abstractions.Entities;
using Soul.Shop.Module.ShoppingCart.Abstractions.ViewModels;

namespace Soul.Shop.Module.ShoppingCart.Abstractions.Services;

public interface ICartService
{
    Task AddToCart(int customerId, int productId, int quantity);

    Task AddToCart(int customerId, int createdById, int productId, int quantity);

    Task UpdateItemQuantity(int customerId, int createdById, int productId, int quantity);

    Task CheckedItem(int customerId, CheckedItemParam model);

    IQueryable<Cart> Query();

    IQueryable<Cart> GetActiveCart(int customerId);

    IQueryable<Cart> GetActiveCart(int customerId, int createdById);

    Task<CartResult> GetActiveCartDetails(int customerId);

    Task<CartResult> GetActiveCartDetails(int customerId, int createdById);

    // Task<CouponValidationResult> ApplyCoupon(int cartId, string couponCode);

    Task MigrateCart(int fromUserId, int toUserId);
}