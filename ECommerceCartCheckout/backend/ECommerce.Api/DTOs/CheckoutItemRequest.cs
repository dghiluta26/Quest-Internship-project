namespace ECommerce.Api.DTOs;

public class CheckoutItemRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}