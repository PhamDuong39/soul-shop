﻿using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels;

public class OrderEditParam
{
    public int? ShippingAddressId { get; set; }

    public int? BillingAddressId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// 配送方式
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; }

    /// <summary>
    /// 配送/运费金额
    /// </summary>
    public decimal ShippingFeeAmount { get; set; }

    /// <summary>
    /// 订单明细 产品总额 Sum(ProductPrice * Quantity)
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// 订单明细 折扣总额 Sum(DiscountAmount)
    /// </summary>
    public decimal SubTotalWithDiscount { get; set; }

    /// <summary>
    /// 订单总金额 SubTotal + ShippingFeeAmount - SubTotalWithDiscount - DiscountAmount 
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// 订单折扣总额（运费券、满减券等）
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 下单备注
    /// </summary>
    [StringLength(450)]
    public string OrderNote { get; set; }

    /// <summary>
    /// 管理员备注（仅内部使用）
    /// </summary>
    [StringLength(450)]
    public string AdminNote { get; set; }

    public IList<OrderCreateItemParam> Items { get; set; } = new List<OrderCreateItemParam>();

    public OrderCreateAddressParam BillingAddress { get; set; }

    public OrderCreateAddressParam ShippingAddress { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// 支付金额
    /// </summary>
    public decimal PaymentFeeAmount { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaymentOn { get; set; }

    /// <summary>
    /// 运输状态
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    public DateTime? ShippedOn { get; set; }

    /// <summary>
    /// 收货时间
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    public RefundStatus? RefundStatus { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    public string RefundReason { get; set; }

    /// <summary>
    /// 退款时间
    /// </summary>
    public DateTime? RefundOn { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 交易关闭/交易取消原因
    /// 可以选择的理由有：
    /// 1、未及时付款
    /// 2、买家不想买了
    /// 3、买家信息填写错误，重新拍
    /// 4、恶意买家/同行捣乱
    /// 5、缺货
    /// 6、买家拍错了
    /// 7、同城见面交易
    /// ...
    /// </summary>
    [StringLength(450)]
    public string CancelReason { get; set; }

    /// <summary>
    /// 交易关闭/交易取消时间
    /// </summary>
    public DateTime? CancelOn { get; set; }
}