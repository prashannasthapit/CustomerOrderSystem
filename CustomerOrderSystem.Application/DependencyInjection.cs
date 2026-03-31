using CustomerOrderSystem.Services.Implementations;
using CustomerOrderSystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerOrderSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderItemService, OrderItemService>();

        return services;
    }
}

