namespace ECommerce.Api.DTOs;

public class CheckoutRequest
{
    public int UserId { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public List<CheckoutItemRequest> Items { get; set; } = new();
}