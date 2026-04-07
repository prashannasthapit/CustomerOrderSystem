using CustomerOrderSystem.Domain.Entities;

namespace CustomerOrderSystem.Domain.Repositories;

public interface ICustomerRepository : IRepositoryBase<Customer>
{
    Task<Customer?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithOrdersAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, int? excludingId = null, CancellationToken cancellationToken = default);
}

