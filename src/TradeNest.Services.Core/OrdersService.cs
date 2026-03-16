using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Cart.Enums;
using TradeNest.Services.Models.Order;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class OrdersService : IOrdersService
{
    private readonly IRepository _repository;
    private readonly IOrdersMapper _ordersMapper;

    public OrdersService(IRepository repository, IOrdersMapper ordersMapper)
    {
        this._repository = repository;
        this._ordersMapper = ordersMapper;
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

        IEnumerable<Order> orders = await this._repository.All<Order>()
            .Include(o => o.OrderProducts)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.SubmittedOn) 
            .ThenByDescending(o => o.TotalPrice)
            .ThenBy(o => o.OrderProducts.Count)
            .ToArrayAsync();
        
        return this._ordersMapper.ToOrderDtos(orders);
    }

    public async Task<bool> OrderExistsByIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
            return false;

        return await this._repository
            .ExistsAsync<Order>(o => o.Id == orderId);
    }

    public async Task<SubmitOrderResultDto> SubmitOrderAsync(Guid userId)
    {
        if(userId == Guid.Empty )
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        Cart? userCart = await this._repository.All<Cart>()
            .Include(c => c.CartProducts)
            .ThenInclude(cp => cp.Product)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
        if (userCart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        try
        {
            ICollection<OrderProduct> orderProducts = new List<OrderProduct>();
            ICollection<ErrorProductDto> errorProducts = new List<ErrorProductDto>();
            
            foreach (CartProduct cartProduct in userCart.CartProducts)
            {
                bool productIsEnabled = cartProduct.Product.IsEnabled;
                bool enoughQtyIsAvailable 
                    = cartProduct.Product.QuantityInStock >= cartProduct.ProductQuantityAdded;
                
                if (!productIsEnabled || !enoughQtyIsAvailable)
                {
                    ErrorProductDto errorProductDto = new ErrorProductDto()
                    {
                        ProductId = cartProduct.ProductId,
                        ProductName = cartProduct.Product.Name
                    };

                    if (!productIsEnabled)
                    {
                        errorProductDto.ProductErrorReasons
                            .Add(ProductErrorReason.StatusChange);
                    }

                    if (!enoughQtyIsAvailable)
                    {
                        errorProductDto.ProductErrorReasons
                            .Add(ProductErrorReason.QuantityChange);
                    }
                    
                    errorProducts.Add(errorProductDto);
                    continue;
                }

                cartProduct.Product.QuantityInStock -= cartProduct.ProductQuantityAdded;

                orderProducts.Add(new OrderProduct()
                {
                    OriginalProductId = cartProduct.ProductId,
                    ProductNameAtOrderTime = cartProduct.Product.Name,
                    CostPriceAtOrderTime = cartProduct.Product.CostPrice,
                    UnitSellingPriceAtOrderTime = cartProduct.Product.SellingPrice,
                    QuantityOrdered = cartProduct.ProductQuantityAdded,
                    TotalProductPriceAtOrderTime = cartProduct.Product.SellingPrice *
                                                   cartProduct.ProductQuantityAdded,
                });   
            }

            if (errorProducts.Any())
                return SubmitOrderResultDto.Failure(errorProducts: errorProducts);
            
            Order newOrder = new Order()
            {
                UserId = userId,
                OrderProducts = orderProducts.ToHashSet(),
                TotalPrice = orderProducts.Sum(op => op.TotalProductPriceAtOrderTime),
                SubmittedOn = DateTime.UtcNow
            };
            
            await this._repository.AddAsync(newOrder);
            this._repository.Remove(userCart);

            await this._repository.SaveChangesAsync();
            return SubmitOrderResultDto.Success();
        }
        catch (DbUpdateConcurrencyException concurrencyEx)
        {
            ICollection<ErrorProductDto> errorProducts = new List<ErrorProductDto>();
            
            IEnumerable<EntityEntry> originalEntitiesEntries = concurrencyEx.Entries;
            foreach (EntityEntry originalEntityEntry in originalEntitiesEntries)
            {
                if (originalEntityEntry is EntityEntry<Product> originalProductEntry)
                {
                    PropertyValues? productEntityDbValues
                        = await originalProductEntry.GetDatabaseValuesAsync();
                    if (productEntityDbValues == null)
                    {
                        // entity is hard deleted
                        errorProducts.Add(new ErrorProductDto()
                        {
                            ProductId = originalProductEntry.Entity.Id,
                            ProductName = originalProductEntry.Entity.Name,
                            ProductErrorReasons = new List<ProductErrorReason>()
                            {
                                ProductErrorReason.Deleted,
                            }
                        });
                        
                        continue;
                    }

                    ICollection<ProductErrorReason> errorReasons
                        = this.FindRelevantValuesMissMatches(userId, originalProductEntry, productEntityDbValues);
                    if (!errorReasons.Any())
                    {
                        // no relevant properties changed => set the db values;
                        originalProductEntry.OriginalValues.SetValues(productEntityDbValues);
                        continue;
                    }
                    
                    errorProducts.Add(new ErrorProductDto()
                    {
                        ProductId = originalProductEntry.Entity.Id,
                        ProductName = originalProductEntry.Entity.Name,
                        ProductErrorReasons = errorReasons
                    });
                }
            }

            if (errorProducts.Any())
                return SubmitOrderResultDto.Failure(errorProducts: errorProducts);

            await this._repository.SaveChangesAsync();
            return SubmitOrderResultDto.Success();
        }
    }

    private ICollection<ProductErrorReason> FindRelevantValuesMissMatches(
        Guid userId,
        EntityEntry<Product> originalProductEntry,
        PropertyValues productEntityDbValues)
    {
        ICollection<ProductErrorReason> errorReasons = new List<ProductErrorReason>();
        foreach (PropertyEntry propertyEntry in originalProductEntry.Properties)
        {
            string propertyName = propertyEntry.Metadata.Name;
            object propDbValue = productEntityDbValues[propertyName]!;
            object propOriginalValue = propertyEntry.OriginalValue!;
            
            if (IsNotRelevantCaseToHandle(propertyName, propDbValue,
                    originalProductEntry, userId))
            {
                continue;
            }
                        
            if (!propOriginalValue.Equals(propDbValue))
            {
                ProductErrorReason? errorReason = propertyName switch
                {
                    nameof(originalProductEntry.Entity.QuantityInStock) 
                        => ProductErrorReason.QuantityChange,
                    
                    nameof(originalProductEntry.Entity.SellingPrice) 
                        => ProductErrorReason.PriceChange,
                    
                    nameof(originalProductEntry.Entity.IsEnabled) 
                        => ProductErrorReason.StatusChange,
                    
                    nameof(originalProductEntry.Entity.IsDeleted) 
                        => ProductErrorReason.Deleted,
                    
                    _ => null,
                };
                errorReasons.Add(errorReason!.Value);
            }
        }
        
        return errorReasons;
        
        bool IsNotRelevantCaseToHandle(string propertyName, object propDbValue,
            EntityEntry<Product> originalProductEntry, Guid userId)
        {
            Product originalProductEntity = originalProductEntry.Entity;
        
            bool isSellingPriceProperty = propertyName == nameof(originalProductEntity.SellingPrice);
            bool isStatusChangeProperty = propertyName == nameof(originalProductEntity.IsEnabled);
            bool isSoftDeleteProperty = propertyName == nameof(originalProductEntity.IsDeleted);
        
            // The following expression describes case where the QuantityInStock property has changed value but there is still enough qty available to satisfy the cart product requested qty to order.
            bool isQtyPropAndNewValueIsAboveCartProductRequestedQty 
                = propertyName == nameof(originalProductEntity.QuantityInStock) &&
                  originalProductEntity.ProductCarts.Single(cp => cp.Cart.CartOwnerId == userId)
                      .ProductQuantityAdded <= (int)propDbValue;

            return !isSellingPriceProperty && !isStatusChangeProperty &&
                   !isSoftDeleteProperty && !isQtyPropAndNewValueIsAboveCartProductRequestedQty;
        }
    }
    
}