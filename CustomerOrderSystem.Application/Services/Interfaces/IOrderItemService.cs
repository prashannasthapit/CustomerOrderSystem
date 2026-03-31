using CustomerOrderSystem.DTOs.OrderItems;

namespace CustomerOrderSystem.Services.Interfaces;

public interface IOrderItemService
{
    Task<IReadOnlyCollection<OrderItemResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderItemResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderItemResponseDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<OrderItemResponseDto> CreateAsync(CreateOrderItemRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateOrderItemRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

