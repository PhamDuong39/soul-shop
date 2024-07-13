using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Payments.Models;

/// <summary>
/// Order payment request parameters
/// </summary>
public class PaymentOrderRequest
{
    [Required] public int OrderId { get; set; }

    /// <summary>
    /// Order number, within 32 characters
    /// </summary>
    [StringLength(32)]
    [Required]
    public string OrderNo { get; set; }

    /// <summary>
    /// Simple description of the product
    /// Tianwang-Mall
    /// Tencent Recharge Center-QQ Member Recharge The title of the website homepage opened by the browser-Product Overview
    /// Tencent-Game Merchant Name-Selling Product Category
    /// Xiao Zhang Nanshan Store-Supermarket Store Name-Selling Product Category
    /// Tiantian Ai Xiaochu-Game Recharge The name of the APP on the application market-Product Overview
    /// </summary>
    [StringLength(128)]
    [Required]
    public string Subject { get; set; }

    /// <summary>
    /// Total order amount, in RMB (up to 2 decimal places)
    /// </summary>
    [Required]
    [Range(0.01, 50000)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public string OpenId { get; set; }
}
