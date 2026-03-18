using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Aggregates;

namespace ProductService.Infrastructure.Persistence;

/// <summary>
/// DbContext do ProductService.
/// Camada "Flesh" da Tomato Architecture — implementa o acesso a dados.
/// </summary>
public sealed class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
