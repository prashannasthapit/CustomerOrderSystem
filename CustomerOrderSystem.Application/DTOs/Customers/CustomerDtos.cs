using System.ComponentModel.DataAnnotations;

namespace CustomerOrderSystem.DTOs.Customers;

public record CustomerResponseDto(
    int Id,
    string Name,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAtUtc);

public class CreateCustomerRequestDto
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(30)]
    public string? PhoneNumber { get; set; }
}

public class UpdateCustomerRequestDto
{
    [StringLength(150, MinimumLength = 2)]
    public string? Name { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(30)]
    public string? PhoneNumber { get; set; }
}

