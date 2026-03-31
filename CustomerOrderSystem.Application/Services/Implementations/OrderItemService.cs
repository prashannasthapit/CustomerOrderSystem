using CustomerOrderSystem.DTOs.OrderItems;
using CustomerOrderSystem.Domain.Abstractions;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Services.Interfaces;

namespace CustomerOrderSystem.Services.Implementations;

public class OrderItemService(
    IOrderItemRepository orderItemRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IOrderItemService
{
    public async Task<IReadOnlyCollection<OrderItemResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await orderItemRepository.FindAllAsync())
            .Select(i => ToDto(i))
            .ToList();
    }

    public async Task<OrderItemResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await orderItemRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException($"Order item with id '{id}' was not found.");
        }

        return ToDto(item);
    }

    public async Task<IReadOnlyCollection<OrderItemResponseDto>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var orderExists = await orderRepository.ExistsByIdAsync(orderId, cancellationToken);
        if (!orderExists)
        {
            throw new NotFoundException($"Order with id '{orderId}' was not found.");
        }

        return (await orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken))
            .Select(i => ToDto(i))
            .ToList();
    }

    public async Task<OrderItemResponseDto> CreateAsync(CreateOrderItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var orderExists = await orderRepository.ExistsByIdAsync(request.OrderId, cancellationToken);
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

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        orderItemRepository.Create(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ToDto(item);
    }

    public async Task UpdateAsync(int id, UpdateOrderItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var item = await orderItemRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken);
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

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await orderItemRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken);
        if (item is null)
        {
            throw new NotFoundException($"Order item with id '{id}' was not found.");
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        orderItemRepository.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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

