using System.Linq.Expressions;
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
            throw new ArgumentException("Invalid userId provided.", nameof(userId));

        return await this._dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.SubmittedOn) // nulls go on the bottom
            .Select(o => new OrderViewModel()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductViewModel()
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        QuantityOrdered = op.ProductsQuantity,
                        UnitPrice = op.Product.SellingPrice,
                        TotalPrice = op.ProductsQuantity * op.Product.SellingPrice,
                    })
                    .ToArray(),
            })
            .ToArrayAsync();
    }

    public async Task AddProductToOrderAsync(Guid userId, Guid productId, int prodQtyToAdd)
    {
        if (userId == Guid.Empty || productId == Guid.Empty || prodQtyToAdd <= 0)
            throw new ArgumentException("Invalid argument's were provided.");

        Order? ongoingOrder = await this._dbContext.Orders
            .Include(o => o.OrderProducts)
            .Where(o => o.UserId == userId)
            .SingleOrDefaultAsync(o => !o.IsSubmitted);
        if (ongoingOrder == null)
            ongoingOrder = await this.CreateOngoingOrder(userId);

        Product? product = await this._dbContext.Products.FindAsync(productId);
        if(product == null)
            throw new ArgumentException("Invalid argument was provided.", nameof(productId));

        int userAlreadyAddedProdQty = ongoingOrder
            .OrderProducts
            .SingleOrDefault(op => op.ProductId == productId)?.ProductsQuantity ?? 0;
        if (product.QuantityInStock < prodQtyToAdd + userAlreadyAddedProdQty)
            throw new ArgumentException("Insufficient product quantity.");

        ongoingOrder.TotalPrice += product.SellingPrice * prodQtyToAdd;

        if (userAlreadyAddedProdQty > 0)
        {
            ongoingOrder.OrderProducts
                .Single(op => op.ProductId == productId)
                .ProductsQuantity += prodQtyToAdd;
        }
        else
        {
            OrderProduct orderProduct = new OrderProduct()
            {
                OrderId = ongoingOrder.Id,
                ProductId = product.Id,
                ProductsQuantity = prodQtyToAdd,
            };
            
            await this._dbContext.OrdersProducts.AddAsync(orderProduct);
        }
        
        await this._dbContext.SaveChangesAsync();
    }

    public async Task<OrderViewModel?> GetUserOngoingOrderWithProductsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid userId provided.", nameof(userId));
        
        return await this._dbContext
            .Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId && !o.IsSubmitted)
            .Select(o => new OrderViewModel()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductViewModel()
                    {
                        Id = op.Product.Id,
                        Name = op.Product.Name,
                        QuantityOrdered = op.ProductsQuantity,
                        UnitPrice = op.Product.SellingPrice,
                        TotalPrice = op.ProductsQuantity * op.Product.SellingPrice,
                    })
                    .ToArray(),
            })
            .SingleOrDefaultAsync();
    }

    public async Task RemoveProductFromOrderAsync(Guid userId, Guid productId, Guid orderId)
    {
        if(userId == Guid.Empty || productId == Guid.Empty || orderId == Guid.Empty)
            throw new ArgumentException("Invalid argument's were provided.");

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId && !o.IsSubmitted,
            includeProducts: true);

        Product? product = await this._dbContext.Products.FindAsync(productId);
        bool prodExistsInOngoingOrder = order.OrderProducts
            .Any(op => op.ProductId == productId);
        if (product == null || !prodExistsInOngoingOrder)
        {
            throw new InvalidOperationException(
                $"Can not remove product with id {productId} from the user's order if it isn't already there.");
        }

        OrderProduct orderProductToRemove = order.OrderProducts
            .Single(op => op.ProductId == productId);
        
        order.OrderProducts.Remove(orderProductToRemove);
        order.TotalPrice -= orderProductToRemove.ProductsQuantity * product.SellingPrice;

        if (!order.OrderProducts.Any())
            this._dbContext.Orders.Remove(order);

        await this._dbContext.SaveChangesAsync();
    }

    public async Task<bool> OrderExistsByIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
            return false;

        return await this._dbContext.Orders
            .AnyAsync(o => o.Id == orderId);
    }

    public async Task CancelOrderAsync(Guid userId, Guid orderId)
    {
        if(orderId == Guid.Empty || userId == Guid.Empty)
            throw new ArgumentException("Invalid argument were provided.", nameof(orderId));

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId,
            includeProducts: false);

        this._dbContext.Orders.Remove(order);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task SubmitOrderAsync(Guid userId, Guid orderId)
    {
        if(orderId == Guid.Empty || userId == Guid.Empty)
            throw new ArgumentException("Invalid argument were provided.", nameof(orderId));

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId,
            includeProducts: true);

        foreach (OrderProduct orderProduct in order.OrderProducts)
        {
            if (orderProduct.Product.QuantityInStock < orderProduct.ProductsQuantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient amount of stock for product {orderProduct.Product.Name}");
            }

            orderProduct.Product.QuantityInStock -= orderProduct.ProductsQuantity;

            if (orderProduct.Product.QuantityInStock == 0)
                orderProduct.Product.IsEnabled = false;
        }

        order.IsSubmitted = true;
        order.SubmittedOn = DateTime.UtcNow;
        order.TotalPrice = order.OrderProducts
            .Sum(op => op.ProductsQuantity * op.Product.SellingPrice);

        await this._dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsValidProductQtyToOrder(Guid userId,
        ValidateProductQtyInputModel inputModel)
    {
        if (inputModel.Id == Guid.Empty)
            return false;
        
        Product? prod = await this._dbContext.Products.FindAsync(inputModel.Id);
        if (prod == null)
            return false;

        OrderProduct? prodAlreadyAddedToOrder = await this._dbContext.OrdersProducts
            .Include(op => op.Order)
            .SingleOrDefaultAsync(op => op.Order.UserId == userId &&
                                        !op.Order.IsSubmitted &&
                                        op.ProductId == inputModel.Id);
        if (prodAlreadyAddedToOrder == null)
            return inputModel.Quantity > 0 && inputModel.Quantity <= prod.QuantityInStock;

        return inputModel.Quantity > 0 &&
               inputModel.Quantity <= prod.QuantityInStock - prodAlreadyAddedToOrder.ProductsQuantity;
    }

    private async Task<Order> GetOrderForModificationWithOrderProductsAttachedAsync(
        Guid userId, 
        Guid orderId,
        Expression<Func<Order, bool>> filterPredicate,
        bool includeProducts = false)
    {
        IQueryable<Order> orderQuery = this._dbContext.Orders;
        if (includeProducts)
        {
            orderQuery = orderQuery
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product);
        }
        else
        {
            orderQuery = orderQuery
                .Include(o => o.OrderProducts);
        }
        Order? order = await orderQuery
            .SingleOrDefaultAsync(filterPredicate);
        
        if (order == null)
            throw new ArgumentException("Order not found.", nameof(orderId));

        if (order.UserId != userId)
            throw new InvalidOperationException("Unauthorized operation attempt.");

        if (order.IsSubmitted)
            throw new InvalidOperationException($"Order with id: {orderId} already is submitted.");

        return order;
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