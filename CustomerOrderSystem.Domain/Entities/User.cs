using Microsoft.AspNetCore.Identity;

namespace CustomerOrderSystem.Domain.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}