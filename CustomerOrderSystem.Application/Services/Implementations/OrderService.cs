using CustomerOrderSystem.DTOs.Orders;
using CustomerOrderSystem.Domain.Abstractions;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Services.Interfaces;

namespace CustomerOrderSystem.Services.Implementations;

public class OrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<IReadOnlyCollection<OrderResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await orderRepository.GetAllWithItemsAsync(cancellationToken))
            .Select(o => ToDto(o))
            .ToList();
    }

    public async Task<OrderResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(id, asNoTracking: true, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order with id '{id}' was not found.");
        }

        return ToDto(order);
    }

    public async Task<IReadOnlyCollection<OrderResponseDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customerExists = await customerRepository.ExistsByIdAsync(customerId, cancellationToken);
        if (!customerExists)
        {
            throw new NotFoundException($"Customer with id '{customerId}' was not found.");
        }

        return (await orderRepository.GetByCustomerIdWithItemsAsync(customerId, cancellationToken))
            .Select(o => ToDto(o))
            .ToList();
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var customerExists = await customerRepository.ExistsByIdAsync(request.CustomerId, cancellationToken);
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

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        orderRepository.Create(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ToDto(order);
    }

    public async Task UpdateAsync(int id, UpdateOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken);
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

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(id, asNoTracking: false, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException($"Order with id '{id}' was not found.");
        }

        if (order.OrderItems.Count > 0)
        {
            throw new ConflictException("Cannot delete order with existing order items. Delete order items first.");
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        orderRepository.Delete(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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

