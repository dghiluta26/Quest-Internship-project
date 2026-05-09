namespace ECommerce.Api.DTOs;

public class CheckoutResponse
{
    public int OrderId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Message { get; set; } = string.Empty;
}