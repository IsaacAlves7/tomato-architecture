using OrderService.Domain.Aggregates;
using SharedKernel.Common;

namespace OrderService.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Order>> GetByCustomerAsync(string email, int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<Order>> GetAllAsync(int page, int pageSize, OrderStatus? status = null, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
