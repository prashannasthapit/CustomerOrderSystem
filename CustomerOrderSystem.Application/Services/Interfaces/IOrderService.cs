using CustomerOrderSystem.DTOs.Orders;

namespace CustomerOrderSystem.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyCollection<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderResponseDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<OrderResponseDto> CreateAsync(CreateOrderRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateOrderRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

