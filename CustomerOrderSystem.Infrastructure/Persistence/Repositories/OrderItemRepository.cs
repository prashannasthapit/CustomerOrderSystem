using CustomerOrderSystem.Data;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Infrastructure.Persistence.Repositories;

public class OrderItemRepository(AppDbContext dbContext) : RepositoryBase<OrderItem>(dbContext), IOrderItemRepository
{
    public async Task<IReadOnlyCollection<OrderItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await FindAll()
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderItem?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = FindByCondition(i => i.Id == id, !asNoTracking);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await Context.OrderItems
            .AsNoTracking()
            .Where(i => i.OrderId == orderId)
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);
    }
}

