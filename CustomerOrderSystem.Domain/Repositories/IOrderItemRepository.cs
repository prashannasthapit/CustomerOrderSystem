using CustomerOrderSystem.Domain.Entities;

namespace CustomerOrderSystem.Domain.Repositories;

public interface IOrderItemRepository : IRepositoryBase<OrderItem>
{
    Task<OrderItem?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
}

