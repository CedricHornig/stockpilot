using MediatR;
using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Common.Interfaces;

namespace StockPilot.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(p => p.WarehouseId == request.WarehouseId)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Sku,
                p.Category,
                p.Stock,
                p.LowStockThreshold,
                p.IsLowStock()
            ))
            .ToListAsync(cancellationToken);
    }
}
