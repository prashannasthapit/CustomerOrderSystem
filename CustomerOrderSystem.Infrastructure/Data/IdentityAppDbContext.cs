using CustomerOrderSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderSystem.Data;

public class IdentityAppDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    
}