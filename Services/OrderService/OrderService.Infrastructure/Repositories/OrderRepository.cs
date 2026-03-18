using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;
using SharedKernel.Common;

namespace OrderService.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context) => _context = context;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<PagedResult<Order>> GetByCustomerAsync(
        string email, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Orders.Include(o => o.Items)
            .Where(o => o.CustomerEmail == email);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Order>(items, total, page, pageSize);
    }

    public async Task<PagedResult<Order>> GetAllAsync(
        int page, int pageSize, OrderStatus? status = null, CancellationToken ct = default)
    {
        var query = _context.Orders.Include(o => o.Items).AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Order>(items, total, page, pageSize);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await _context.Orders.AddAsync(order, ct);

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _context.Orders.AnyAsync(o => o.Id == id, ct);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
