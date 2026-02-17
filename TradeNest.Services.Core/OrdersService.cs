using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.GCommon;
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

    public async Task<IEnumerable<OrderViewModel>> GetAllOrdersByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(nameof(userId));

        return await this._dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.SubmittedOn) // nulls go on the bottom
            .Select(o => new OrderViewModel()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn.HasValue 
                    ? o.SubmittedOn.Value.ToString(ApplicationConstants.DatesFormat)
                    : null,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductViewModel()
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        QuantityOrdered = op.ProductsQuantity,
                        UnitPrice = op.Product.SellingPrice
                            .ToString(ApplicationConstants.PricesFormat),
                        TotalPrice = (op.ProductsQuantity * op.Product.SellingPrice)
                            .ToString(ApplicationConstants.PricesFormat),
                    })
                    .ToArray(),
            })
            .ToArrayAsync();
    }

    public async Task AddProductToOrderAsync(Guid userId, Guid productId, int productQuantity)
    {
        if (userId == Guid.Empty || productId == Guid.Empty || productQuantity <= 0)
            throw new ArgumentException("Invalid argument's were provided.");

        Order? ongoingOrder = await this._dbContext.Orders
            .Where(o => o.UserId == userId)
            .SingleOrDefaultAsync(o => !o.IsSubmitted);
        if (ongoingOrder == null)
            ongoingOrder = await this.CreateOngoingOrder(userId);

        Product? product = await this._dbContext.Products.FindAsync(productId);
        if(product == null)
            throw new ArgumentException("Invalid argument were provided.", nameof(productId));

        if (product.QuantityInStock < productQuantity)
            throw new ArgumentException("Insufficient product quantity.");

        ongoingOrder.TotalPrice += product.SellingPrice * productQuantity;
        
        OrderProduct orderProduct = new OrderProduct()
        {
            OrderId = ongoingOrder.Id,
            ProductId = product.Id,
            ProductsQuantity = productQuantity,
        };

        await this._dbContext.OrdersProducts.AddAsync(orderProduct);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task<OrderViewModel?> GetUserOngoingOrderWithProductsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(nameof(userId));
        
        return await this._dbContext
            .Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId && !o.IsSubmitted)
            .Select(o => new OrderViewModel()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn.HasValue 
                    ? o.SubmittedOn.Value.ToString(ApplicationConstants.DatesFormat)
                    : null,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductViewModel()
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        QuantityOrdered = op.ProductsQuantity,
                        UnitPrice = op.Product.SellingPrice
                            .ToString(ApplicationConstants.PricesFormat),
                        TotalPrice = (op.ProductsQuantity * op.Product.SellingPrice)
                            .ToString(ApplicationConstants.PricesFormat),
                    })
                    .ToArray(),
            })
            .SingleOrDefaultAsync();
    }

    private async Task<Order> CreateOngoingOrder(Guid userId)
    {
        Order newOngoingOrder = new Order()
        {
            IsSubmitted = false,
            TotalPrice = 0m,
            UserId = userId
        };

        await this._dbContext.AddAsync(newOngoingOrder);
        return newOngoingOrder;
    }
}