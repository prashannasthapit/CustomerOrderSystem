using System.ComponentModel.DataAnnotations;
using CustomerOrderSystem.Domain.Entities;

namespace CustomerOrderSystem.DTOs.Orders;

public record OrderResponseDto(
    int Id,
    int CustomerId,
    DateTime OrderDateUtc,
    OrderStatus Status,
    decimal TotalAmount);

public class CreateOrderRequestDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    public DateTime? OrderDateUtc { get; set; }

    public OrderStatus? Status { get; set; }
}

public class UpdateOrderRequestDto
{
    public DateTime? OrderDateUtc { get; set; }

    public OrderStatus? Status { get; set; }
}

