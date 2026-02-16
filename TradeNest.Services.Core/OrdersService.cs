using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Order;

namespace TradeNest.Services.Core;

public class OrdersService : IOrdersService
{
    private readonly TradeNestDbContext _dbContext;

    public OrdersService(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<IEnumerable<AllOrdersViewModel>> GetAllOrdersByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(nameof(userId));

        IEnumerable<AllOrdersViewModel> userOrders = await this._dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.SubmittedOn) // nulls go on the bottom
            .Select(o => new AllOrdersViewModel()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn.HasValue
                    ? DateOnly.FromDateTime(o.SubmittedOn.Value)
                    : null,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductViewModel()
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        QuantityOrdered = op.ProductsQuantity,
                        UnitPrice = op.Product.SellingPrice,
                        TotalPrice = op.ProductsQuantity * op.Product.SellingPrice
                    })
                    .ToArray(),
            })
            .ToArrayAsync();

        return userOrders;
    }
}