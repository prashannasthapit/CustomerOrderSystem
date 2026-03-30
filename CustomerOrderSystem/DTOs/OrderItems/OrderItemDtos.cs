using System.ComponentModel.DataAnnotations;

namespace CustomerOrderSystem.DTOs.OrderItems;

public record OrderItemResponseDto(
    int Id,
    int OrderId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public class CreateOrderItemRequestDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderItemRequestDto
{
    [StringLength(200, MinimumLength = 2)]
    public string? ProductName { get; set; }

    [Range(1, int.MaxValue)]
    public int? Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "999999999.99")]
    public decimal? UnitPrice { get; set; }
}

