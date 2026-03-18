using ProductService.Domain.Aggregates;
using SharedKernel.Common;

namespace ProductService.Domain.Repositories;

/// <summary>
/// Interface do repositório de Produto — definida no "Seed" (Domain),
/// implementada no "Flesh" (Infrastructure). Princípio de inversão de dependência.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<PagedResult<Product>> GetAllAsync(int page, int pageSize, string? category = null, CancellationToken ct = default);
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
