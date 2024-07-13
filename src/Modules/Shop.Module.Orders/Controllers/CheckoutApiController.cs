using Microsoft.AspNetCore.Authorization;
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
/// Checkout API controller, used to handle checkout operations for shopping carts, individual items, and orders.
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
    /// Checkout through the shopping cart.
    /// </summary>
    /// <param name="userAddressId">User address ID (optional). </param>
    /// <returns>Checkout result. </returns>
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
            throw new Exception("Please select product");
        if (cart.Items.Where(c => c.IsChecked).Any(c => c.Quantity <= 0)) throw new Exception("The quantity of the product must be greater than 0");
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
    /// Direct settlement through a single product.
    /// </summary>
    /// <param name="userAddressId">User address ID (optional). </param>
    /// <param name="productId">Product ID. </param>
    /// <param name="quantity">Purchase quantity. </param>
    /// <returns>Settlement result. </returns>
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
    /// Settle via existing order.
    /// </summary>
    /// <param name="userAddressId">User address ID (optional). </param>
    /// <param name="orderId">Order ID. </param>
    /// <returns>Settlement result. </returns>
    [HttpGet("order")]
    public async Task<Result> CheckoutByOrder(int? userAddressId, int orderId)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var order = await _orderRepository.Query()
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == orderId && c.CustomerId == user.Id);
        if (order == null) throw new Exception("The order does not exist");
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
    /// Submit the shopping cart to settle the order.
    /// </summary>
    /// <param name="model">Shopping cart settlement parameters. </param>
    /// <returns>Order creation results. </returns>
    [HttpPost("cart")]
    public async Task<Result> PostByCart([FromBody] OrderCreateByCartParam model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var cart = await _cartRepository.Query().FirstOrDefaultAsync(x => x.CustomerId == user.Id && x.IsActive);
        if (cart == null)
            throw new Exception("Shopping cart information does not exist");

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
    /// Submit a single product settlement to generate an order.
    /// </summary>
    /// <param name="model">Single product settlement parameters. </param>
    /// <returns>Order creation results. </returns>
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
    /// Submit the existing order settlement to generate a new order.
    /// </summary>
    /// <param name="model">Existing order settlement parameters. </param>
    /// <returns>New order creation results. </returns>
    [HttpPost("order")]
    public async Task<Result> PostByOrder([FromBody] OrderCreateByOrderParam model)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var oldOrder = await _orderRepository.Query()
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == model.OrderId && c.CustomerId == user.Id);
        if (oldOrder == null) throw new Exception("The order does not exist");
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
