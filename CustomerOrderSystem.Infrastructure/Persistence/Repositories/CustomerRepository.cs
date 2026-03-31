using CustomerOrderSystem.Data;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Infrastructure.Persistence.Repositories;

public class CustomerRepository(AppDbContext dbContext) : RepositoryBase<Customer>(dbContext), ICustomerRepository
{
    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await FindAll()
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = FindByCondition(c => c.Id == id, !asNoTracking);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdWithOrdersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Customers.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        return Context.Customers.AnyAsync(
            c => c.Email == email && (!excludingId.HasValue || c.Id != excludingId.Value),
            cancellationToken);
    }
}

