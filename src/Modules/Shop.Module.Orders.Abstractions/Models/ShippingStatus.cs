namespace Shop.Module.Orders.Models;

/// <summary>
/// Shipping status
/// </summary>
public enum ShippingStatus
{
    /// <summary>
    /// No Shipping
    /// No shipping
    /// </summary>
    NoShipping = 0,

    /// <summary>
    /// Not yet shipped
    /// Not yet shipped
    /// WAIT_SELLER_SEND_GOODS (waiting for the seller to ship, that is: the buyer has paid)
    /// </summary>
    NotYetShipped = 20,

    /// <summary>
    /// Partially shipped
    /// Partially shipped
    /// SELLER_PART_SEND_GOODS: Partial shipment; SELLER_CONSIGNED_PART (partial shipment by the seller)
    /// </summary>
    PartiallyShipped = 25,

    /// <summary>
    /// Shipped
    /// Shipped/All shipped
    /// WAIT_BUYER_ACCEPT_GOODS: Waiting for the buyer to receive the goods; WAIT_BUYER_CONFIRM_GOODS (waiting for the buyer to confirm receipt, that is: the seller has shipped); the seller has shipped SELLER_SEND_GOODS
    /// </summary>
    Shipped = 30,

    /// <summary>
    /// Delivered
    /// The buyer has confirmed receipt BUYER_ACCEPT_GOODS,NO_LOGISTICS
    /// </summary>
    Delivered = 40
}
