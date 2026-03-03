using Microsoft.EntityFrameworkCore;
using StockPilot.Domain.Entities;

namespace StockPilot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<StockMovement> StockMovements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
