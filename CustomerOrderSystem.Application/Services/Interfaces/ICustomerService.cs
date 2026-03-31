using CustomerOrderSystem.DTOs.Customers;

namespace CustomerOrderSystem.Services.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyCollection<CustomerResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateCustomerRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

