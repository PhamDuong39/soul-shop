﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Models;
using Shop.Module.Orders.Services;
using Shop.Module.Orders.ViewModels;
using Shop.Module.ShoppingCart.Entities;

namespace Shop.Module.Orders.Controllers;

/// <summary>
/// 结算 API 控制器，用于处理购物车、单个商品和订单的结算操作。
/// </summary>
[Authorize()]
[Route("api/checkout")]
public class CheckoutApiController : ControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IOrderService _orderService;

    public CheckoutApiController(
        IWorkContext workContext,
        IRepository<Order> orderRepository,
        IRepository<Cart> cartRepository,
        IOrderService orderService)
    {
        _workContext = workContext;
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _orderService = orderService;
    }

    /// <summary>
    /// 通过购物车结算。
    /// </summary>
    /// <param name="userAddressId">用户地址 ID（可选）。</param>
    /// <returns>结算结果。</returns>
    [HttpGet("cart")]
    public async Task<Result> CheckoutByCart(int? userAddressId)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var customerId = user.Id;
        var cart = await _cartRepository.Query()
            .Where(x => x.CustomerId == customerId && x.IsActive)
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        if (cart == null || cart.Items == null || cart.Items.Where(c => c.IsChecked).Count() <= 0)
            throw new Exception("请选择商品");
        if (cart.Items.Where(c => c.IsChecked).Any(c => c.Quantity <= 0)) throw new Exception("商品数量必须大于0");
        var param = new CheckoutParam()
        {
            CustomerId = customerId,
            UserAddressId = userAddressId
        };
        param.Items = cart.Items.Where(c => c.IsChecked).Select(c => new CheckoutItemParam()
        {
            ProductId = c.ProductId,
            Quantity = c.Quantity
        }).ToList();

        var data = await _orderService.OrderCheckout(param);
        return Result.Ok(data);
    }

    /// <summary>
    /// 通过单个商品直接结算。
    /// </summary>
    /// <param name="userAddressId">用户地址 ID（可选）。</param>
    /// <param name="productId">产品 ID。</param>
    /// <param name="quantity">购买数量。</param>
    /// <returns>结算结果。</returns>
    [HttpGet("product")]
    public async Task<Result> CheckoutByProduct(int? userAddressId, int productId, int quantity)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var param = new CheckoutParam()
        {
            CustomerId = user.Id,
            UserAddressId = userAddressId
        };
        param.Items.Add(new CheckoutItemParam() { ProductId = productId, Quantity = quantity });
        var data = await _orderService.OrderCheckout(param);
        return Result.Ok(data);
    }

    /// <summary>
    /// 通过现有订单结算。
    /// </summary>
    /// <param name="userAddressId">用户地址 ID（可选）。</param>
    /// <param name="orderId">订单 ID。</param>
    /// <returns>结算结果。</returns>
    [HttpGet("order")]
    public async Task<Result> CheckoutByOrder(int? userAddressId, int orderId)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var order = await _orderRepository.Query()
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == orderId && c.CustomerId == user.Id);
        if (order == null) throw new Exception("订单不存在");
        var param = new CheckoutParam()
        {
            CustomerId = user.Id,
            UserAddressId = userAddressId
        };
        param.Items = order.OrderItems.Select(c => new CheckoutItemParam()
        {
            ProductId = c.ProductId,
            Quantity = c.Quantity
        }).ToList();
        var data = await _orderService.OrderCheckout(param);
        return Result.Ok(data);
    }

    /// <summary>
    /// 提交购物车结算生成订单。
    /// </summary>
    /// <param name="model">购物车结算参数。</param>
    /// <returns>订单创建结果。</returns>
    [HttpPost("cart")]
    public async Task<Result> PostByCart([FromBody] OrderCreateByCartParam model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var cart = await _cartRepository.Query().FirstOrDefaultAsync(x => x.CustomerId == user.Id && x.IsActive);
        if (cart == null)
            throw new Exception("购物车信息不存在");

        var result = await _orderService.OrderCreateByCart(cart.Id, new OrderCreateBaseParam()
        {
            CustomerId = user.Id,
            DiscountAmount = 0,
            OrderNote = model.OrderNote,
            PaymentType = PaymentType.OnlinePayment,
            ShippingMethod = ShippingMethod.Free,
            ShippingFeeAmount = 0,
            ShippingUserAddressId = model.ShippingUserAddressId
        });
        return Result.Ok(result);
    }

    /// <summary>
    /// 提交单个商品结算生成订单。
    /// </summary>
    /// <param name="model">单个商品结算参数。</param>
    /// <returns>订单创建结果。</returns>
    [HttpPost("product")]
    public async Task<Result> PostByProduct([FromBody] OrderCreateByProductParam model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var param = new OrderCreateBaseParam()
        {
            CustomerId = user.Id,
            DiscountAmount = 0,
            OrderNote = model.OrderNote,
            ShippingUserAddressId = model.ShippingUserAddressId,
            PaymentType = PaymentType.OnlinePayment,
            ShippingMethod = ShippingMethod.Free,
            ShippingFeeAmount = 0
        };
        param.Items.Add(new OrderCreateBaseItemParam()
        {
            ProductId = model.ProductId,
            Quantity = model.Quantity
        });
        var order = await _orderService.OrderCreate(user.Id, param);
        var result = new OrderCreateResult()
        {
            OrderId = order.Id,
            OrderNo = order.No.ToString(),
            OrderTotal = order.OrderTotal
        };
        return Result.Ok(result);
    }

    /// <summary>
    /// 提交现有订单结算生成新订单。
    /// </summary>
    /// <param name="model">现有订单结算参数。</param>
    /// <returns>新订单创建结果。</returns>
    [HttpPost("order")]
    public async Task<Result> PostByOrder([FromBody] OrderCreateByOrderParam model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var oldOrder = await _orderRepository.Query()
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == model.OrderId && c.CustomerId == user.Id);
        if (oldOrder == null) throw new Exception("订单不存在");
        var param = new OrderCreateBaseParam()
        {
            CustomerId = user.Id,
            DiscountAmount = 0,
            OrderNote = model.OrderNote,
            ShippingUserAddressId = model.ShippingUserAddressId,
            PaymentType = PaymentType.OnlinePayment,
            ShippingMethod = ShippingMethod.Free,
            ShippingFeeAmount = 0
        };
        param.Items = oldOrder.OrderItems.Select(c => new OrderCreateBaseItemParam()
        {
            ProductId = c.ProductId,
            Quantity = c.Quantity
        }).ToList();
        var order = await _orderService.OrderCreate(user.Id, param);
        var result = new OrderCreateResult()
        {
            OrderId = order.Id,
            OrderNo = order.No.ToString(),
            OrderTotal = order.OrderTotal
        };
        return Result.Ok(result);
    }
}