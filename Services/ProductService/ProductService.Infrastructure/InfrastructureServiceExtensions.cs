using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Infrastructure;

/// <summary>
/// Extensão para registro da camada "Flesh" (Infrastructure).
/// Mantém o Program.cs limpo e as dependências encapsuladas.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddProductInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core — usa InMemory para desenvolvimento, SQL Server para produção
        var connectionString = configuration.GetConnectionString("ProductDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<ProductDbContext>(opts =>
                opts.UseInMemoryDatabase("ProductDb"));
        }
        else
        {
            services.AddDbContext<ProductDbContext>(opts =>
                opts.UseSqlServer(connectionString, sql =>
                    sql.EnableRetryOnFailure(maxRetryCount: 3)));
        }

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
