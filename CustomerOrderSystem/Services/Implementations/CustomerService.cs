using CustomerOrderSystem.Data;
using CustomerOrderSystem.DTOs.Customers;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Models;
using CustomerOrderSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Services.Implementations;

public class CustomerService(AppDbContext dbContext) : ICustomerService
{
    public async Task<IReadOnlyCollection<CustomerResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => ToDto(c))
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        return ToDto(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken = default)
    {
        var emailInUse = await dbContext.Customers.AnyAsync(c => c.Email == request.Email, cancellationToken);
        if (emailInUse)
        {
            throw new ConflictException($"Email '{request.Email}' is already in use.");
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber?.Trim()
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(customer);
    }

    public async Task UpdateAsync(int id, UpdateCustomerRequestDto request, CancellationToken cancellationToken = default)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var emailInUse = await dbContext.Customers.AnyAsync(c => c.Id != id && c.Email == normalizedEmail, cancellationToken);
            if (emailInUse)
            {
                throw new ConflictException($"Email '{request.Email}' is already in use.");
            }

            customer.Email = normalizedEmail;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            customer.Name = request.Name.Trim();
        }

        if (request.PhoneNumber is not null)
        {
            customer.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await dbContext.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        if (customer.Orders.Count > 0)
        {
            throw new ConflictException("Cannot delete customer with existing orders. Delete orders first.");
        }

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CustomerResponseDto ToDto(Customer customer)
    {
        return new CustomerResponseDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.PhoneNumber,
            customer.CreatedAtUtc);
    }
}

