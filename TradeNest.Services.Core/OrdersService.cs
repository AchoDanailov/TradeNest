using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class OrdersService : IOrdersService
{
    private readonly IRepository _repository;

    public OrdersService(IRepository repository)
    {
        this._repository = repository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        return await this._repository.AllAsReadonly<Order>()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.SubmittedOn) // nulls go on the bottom
            .ThenByDescending(o => o.TotalPrice)
            .ThenBy(o => o.OrderProducts.Count)
            .Select(o => new OrderDto()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductDto()
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
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(productId)));
        
        if (prodQtyToAdd <= 0)
        {
            throw new ArgumentException(
                string.Format(CantBeZeroOrNegativeNumberMessage, nameof(prodQtyToAdd)));
        }

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
            throw new ArgumentException(string.Format(NotFoundMessage, nameof(ApplicationUser), userId));

        Product? product = await this._repository.FindByIdAsync<Product>(productId);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productId);

        if (product.OwnerId == userId)
        {
            throw new InvalidOperationException(
                string.Format(OwnerCantOrderProductHeOwnsMessage, userId, productId));
        }
        
        Order? ongoingOrder = await this._repository.All<Order>()
            .Include(o => o.OrderProducts)
            .Where(o => o.UserId == userId)
            .SingleOrDefaultAsync(o => !o.IsSubmitted);
        if (ongoingOrder == null)
            ongoingOrder = await this.CreateOngoingOrder(userId);

        int userAlreadyAddedProdQty = 0;
        if (ongoingOrder.OrderProducts.Any())
        {
            userAlreadyAddedProdQty = ongoingOrder
                .OrderProducts
                .SingleOrDefault(op => op.ProductId == productId)?.ProductsQuantity ?? 0;
        }

        if (!product.IsEnabled)
            throw new OrderingDisabledProductException(productId, userId, ongoingOrder.Id);
        
        if (product.QuantityInStock < prodQtyToAdd + userAlreadyAddedProdQty)
        {
            throw new InsufficientProductQuantityInStockException(
                userId: userId,
                productId: productId,
                productQtyInStock: product.QuantityInStock,
                productQtyRequested: prodQtyToAdd + userAlreadyAddedProdQty);
        }

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

            await this._repository.AddAsync<OrderProduct>(orderProduct);
        }
        
        await this._repository.SaveChangesAsync();
    }

    public async Task<OrderDto?> GetUserOngoingOrderWithProductsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        return await this._repository.AllAsReadonly<Order>()
            .Where(o => o.UserId == userId && !o.IsSubmitted)
            .Select(o => new OrderDto()
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                IsSubmitted = o.IsSubmitted,
                SubmittedOn = o.SubmittedOn,
                OrderProducts = o.OrderProducts
                    .Select(op => new OrderProductDto()
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
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(productId)));
        if(orderId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(orderId)));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        Product? product = await this._repository.FindByIdAsync<Product>(productId);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productId);

        if (product.OwnerId == userId)
        {
            throw new InvalidOperationException(
                string.Format(OwnerCantRemoveProductHeOwnsFromOrderMessage, userId, productId));
        }

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId && !o.IsSubmitted,
            includeProducts: true);

        bool prodExistsInOngoingOrder = order.OrderProducts
            .Any(op => op.ProductId == productId);
        if (!prodExistsInOngoingOrder)
        {
            throw new InvalidOperationException(
                $"Can not remove product with id {productId} from the user's order if it isn't already there.");
        }

        OrderProduct orderProductToRemove = order.OrderProducts
            .Single(op => op.ProductId == productId);
        
        order.OrderProducts.Remove(orderProductToRemove);
        order.TotalPrice -= orderProductToRemove.ProductsQuantity * product.SellingPrice;

        if (!order.OrderProducts.Any())
            this._repository.Remove<Order>(order);

        await this._repository.SaveChangesAsync();
    }

    public async Task<bool> OrderExistsByIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
            return false;

        return await this._repository
            .ExistsAsync<Order>(o => o.Id == orderId);
    }

    public async Task CancelOrderAsync(Guid userId, Guid orderId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
        if(orderId == Guid.Empty)
            throw new ArgumentException("OrderId can not be empty.", nameof(orderId));

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId,
            includeProducts: false);

        this._repository.Remove<Order>(order);
        await this._repository.SaveChangesAsync();
    }

    public async Task SubmitOrderAsync(Guid userId, Guid orderId)
    {
        if(userId == Guid.Empty )
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
        if(orderId == Guid.Empty )
            throw new ArgumentException("OrderId can not be empty", nameof(orderId));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
            throw new ArgumentException($"User with id: {userId} not found.", nameof(userId));

        Order order = await this.GetOrderForModificationWithOrderProductsAttachedAsync(
            userId,
            orderId,
            o => o.Id == orderId,
            includeProducts: true);

        foreach (OrderProduct orderProduct in order.OrderProducts)
        {
            if (!orderProduct.Product.IsEnabled)
            {
                order.OrderProducts.Remove(orderProduct);
                
                throw new OrderingDisabledProductException(orderProduct.ProductId,
                    userId, orderId);
            }
            
            if (orderProduct.Product.QuantityInStock < orderProduct.ProductsQuantity)
            {
                order.OrderProducts.Remove(orderProduct);
                
                throw new InsufficientProductQuantityInStockException(
                    userId: userId,
                    productId: orderProduct.ProductId,
                    productQtyInStock: orderProduct.Product.QuantityInStock,
                    productQtyRequested: orderProduct.ProductsQuantity);
            }

            orderProduct.Product.QuantityInStock -= orderProduct.ProductsQuantity;

            if (orderProduct.Product.QuantityInStock == 0)
                orderProduct.Product.IsEnabled = false;
        }

        order.IsSubmitted = true;
        order.SubmittedOn = DateTime.UtcNow;
        order.TotalPrice = order.OrderProducts
            .Sum(op => op.ProductsQuantity * op.Product.SellingPrice);

        await this._repository.SaveChangesAsync();
    }

    public async Task<bool> IsValidProductQtyToOrderAsync(Guid userId,
        ProductQtyValidationDto model)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
        
        if (model.Id == Guid.Empty)
        {
            throw new ArgumentException("Input model Id can not be empty",
                nameof(model.Id));
        }
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
            throw new ArgumentException($"User with id: {userId} not found.", nameof(userId));

        Product? prod = await this._repository.FindByIdAsync<Product>(model.Id);
        if (prod == null)
            throw new ResourceNotFoundException(nameof(Product), model.Id);

        OrderProduct? prodAlreadyAddedToOrder = await this._repository.All<OrderProduct>()
            .Include(op => op.Order)
            .SingleOrDefaultAsync(op => op.Order.UserId == userId &&
                                        !op.Order.IsSubmitted &&
                                        op.ProductId == model.Id);
        if (prodAlreadyAddedToOrder == null)
            return model.Quantity > 0 && model.Quantity <= prod.QuantityInStock;

        return model.Quantity > 0 &&
               model.Quantity <= prod.QuantityInStock - prodAlreadyAddedToOrder.ProductsQuantity;
    }

    private async Task<Order> GetOrderForModificationWithOrderProductsAttachedAsync(
        Guid userId, 
        Guid orderId,
        Expression<Func<Order, bool>> filterPredicate,
        bool includeProducts = false)
    {
        IQueryable<Order> orderQuery = this._repository.All<Order>();
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
            throw new ResourceNotFoundException(nameof(Order), orderId);

        if (order.UserId != userId)
            throw new UnauthorizedOperationException(userId, nameof(Order), orderId);

        if (order.IsSubmitted)
        {
            throw new InvalidOperationException(
                string.Format(OrderAlreadySubmittedMessage, orderId));
        }

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

        await this._repository.AddAsync<Order>(newOngoingOrder);
        return newOngoingOrder;
    }
}