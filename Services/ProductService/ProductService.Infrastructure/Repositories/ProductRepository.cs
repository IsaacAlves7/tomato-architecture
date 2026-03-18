using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Aggregates;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence;
using SharedKernel.Common;

namespace ProductService.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do IProductRepository.
/// Camada "Flesh" — implementa contratos definidos no "Seed" (Domain).
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku, ct);

    public async Task<PagedResult<Product>> GetAllAsync(
        int page, int pageSize, string? category = null, CancellationToken ct = default)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>(items, total, page, pageSize);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default) =>
        await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await _context.Products.AddAsync(product, ct);

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct);
        if (product is not null) _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products.AnyAsync(p => p.Id == id, ct);

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default) =>
        await _context.Products.AnyAsync(p => p.Sku == sku, ct);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
