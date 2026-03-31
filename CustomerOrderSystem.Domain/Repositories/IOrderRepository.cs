using CustomerOrderSystem.Domain.Entities;

namespace CustomerOrderSystem.Domain.Repositories;

public interface IOrderRepository : IRepositoryBase<Order>
{
    Task<IReadOnlyCollection<Order>> GetAllWithItemsAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithItemsAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Order>> GetByCustomerIdWithItemsAsync(int customerId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}

