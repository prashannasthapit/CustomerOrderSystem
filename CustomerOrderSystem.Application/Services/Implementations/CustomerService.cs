using CustomerOrderSystem.DTOs.Customers;
using CustomerOrderSystem.Domain.Abstractions;
using CustomerOrderSystem.Domain.Entities;
using CustomerOrderSystem.Domain.Repositories;
using CustomerOrderSystem.Exceptions;
using CustomerOrderSystem.Services.Interfaces;

namespace CustomerOrderSystem.Services.Implementations;

public class CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<IReadOnlyCollection<CustomerResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await customerRepository.FindAllAsync())
            .Select(c => ToDto(c))
            .ToList();
    }

    public async Task<CustomerResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, asNoTracking: true, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        return ToDto(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailInUse = await customerRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken: cancellationToken);
        if (emailInUse)
        {
            throw new ConflictException($"Email '{request.Email}' is already in use.");
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PhoneNumber = request.PhoneNumber?.Trim()
        };

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        customerRepository.Create(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ToDto(customer);
    }

    public async Task UpdateAsync(int id, UpdateCustomerRequestDto request, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var emailInUse = await customerRepository.ExistsByEmailAsync(normalizedEmail, id, cancellationToken);
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

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdWithOrdersAsync(id, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException($"Customer with id '{id}' was not found.");
        }

        if (customer.Orders.Count > 0)
        {
            throw new ConflictException("Cannot delete customer with existing orders. Delete orders first.");
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        customerRepository.Delete(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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

