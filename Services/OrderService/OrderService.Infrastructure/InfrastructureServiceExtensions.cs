using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddOrderInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrderDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
            services.AddDbContext<OrderDbContext>(opts => opts.UseInMemoryDatabase("OrderDb"));
        else
            services.AddDbContext<OrderDbContext>(opts =>
                opts.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3)));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
