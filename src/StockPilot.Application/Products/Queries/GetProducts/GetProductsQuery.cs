using MediatR;

namespace StockPilot.Application.Products.Queries.GetProducts;

public record GetProductsQuery(Guid WarehouseId) : IRequest<List<ProductDto>>;

public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    string Category,
    int Stock,
    int LowStockThreshold,
    bool IsLowStock
);
