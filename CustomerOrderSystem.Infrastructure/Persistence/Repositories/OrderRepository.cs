using CustomerOrderSystem.Data;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext dbContext) : RepositoryBase<Order>(dbContext), IOrderRepository
{
    public async Task<IReadOnlyCollection<Order>> GetAllWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(int id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = FindByCondition(o => o.Id == id, !asNoTracking);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Id == id);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByCustomerIdWithItemsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await Context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDateUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Orders.AnyAsync(o => o.Id == id, cancellationToken);
    }
}

