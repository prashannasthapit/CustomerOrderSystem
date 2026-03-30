using CustomerOrderSystem.Data;
using CustomerOrderSystem.DTOs.OrderItems;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Models;
using CustomerOrderSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Services.Implementations;

public class OrderItemService(AppDbContext dbContext) : IOrderItemService
{
    public async Task<IReadOnlyCollection<OrderItemResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.OrderItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(i => ToDto(i))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderItemResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException($"Order item with id '{id}' was not found.");
        }

        return ToDto(item);
    }

    public async Task<IReadOnlyCollection<OrderItemResponseDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var orderExists = await dbContext.Orders.AnyAsync(o => o.Id == orderId, cancellationToken);
        if (!orderExists)
        {
            throw new NotFoundException($"Order with id '{orderId}' was not found.");
        }

        return await dbContext.OrderItems
            .AsNoTracking()
            .Where(i => i.OrderId == orderId)
            .OrderBy(i => i.Id)
            .Select(i => ToDto(i))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderItemResponseDto> CreateAsync(CreateOrderItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var orderExists = await dbContext.Orders.AnyAsync(o => o.Id == request.OrderId, cancellationToken);
        if (!orderExists)
        {
            throw new NotFoundException($"Order with id '{request.OrderId}' was not found.");
        }

        var item = new OrderItem
        {
            OrderId = request.OrderId,
            ProductName = request.ProductName.Trim(),
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };

        dbContext.OrderItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(item);
    }

    public async Task UpdateAsync(int id, UpdateOrderItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.OrderItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null)
        {
            throw new NotFoundException($"Order item with id '{id}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            item.ProductName = request.ProductName.Trim();
        }

        if (request.Quantity.HasValue)
        {
            item.Quantity = request.Quantity.Value;
        }

        if (request.UnitPrice.HasValue)
        {
            item.UnitPrice = request.UnitPrice.Value;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.OrderItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null)
        {
            throw new NotFoundException($"Order item with id '{id}' was not found.");
        }

        dbContext.OrderItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static OrderItemResponseDto ToDto(OrderItem item)
    {
        return new OrderItemResponseDto(
            item.Id,
            item.OrderId,
            item.ProductName,
            item.Quantity,
            item.UnitPrice,
            item.Quantity * item.UnitPrice);
    }
}

