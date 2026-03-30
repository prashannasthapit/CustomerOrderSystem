using CustomerOrderSystem.Data;
using CustomerOrderSystem.DTOs.Orders;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Models;
using CustomerOrderSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Services.Implementations;

public class OrderService(AppDbContext dbContext) : IOrderService
{
    public async Task<IReadOnlyCollection<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDateUtc)
            .Select(o => ToDto(o))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order with id '{id}' was not found.");
        }

        return ToDto(order);
    }

    public async Task<IReadOnlyCollection<OrderResponseDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customerExists = await dbContext.Customers.AnyAsync(c => c.Id == customerId, cancellationToken);
        if (!customerExists)
        {
            throw new NotFoundException($"Customer with id '{customerId}' was not found.");
        }

        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDateUtc)
            .Select(o => ToDto(o))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var customerExists = await dbContext.Customers.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
        if (!customerExists)
        {
            throw new NotFoundException($"Customer with id '{request.CustomerId}' was not found.");
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDateUtc = request.OrderDateUtc ?? DateTime.UtcNow,
            Status = request.Status ?? OrderStatus.Pending
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(order);
    }

    public async Task UpdateAsync(int id, UpdateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException($"Order with id '{id}' was not found.");
        }

        if (request.OrderDateUtc.HasValue)
        {
            order.OrderDateUtc = request.OrderDateUtc.Value;
        }

        if (request.Status.HasValue)
        {
            order.Status = request.Status.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException($"Order with id '{id}' was not found.");
        }

        if (order.OrderItems.Count > 0)
        {
            throw new ConflictException("Cannot delete order with existing order items. Delete order items first.");
        }

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static OrderResponseDto ToDto(Order order)
    {
        var totalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

        return new OrderResponseDto(
            order.Id,
            order.CustomerId,
            order.OrderDateUtc,
            order.Status,
            totalAmount);
    }
}

