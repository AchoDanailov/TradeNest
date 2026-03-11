using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Product;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class CartsService : ICartsService
{
    private IRepository _repository;

    public CartsService(IRepository repository)
    {
        this._repository = repository;
    }

    public async Task<CartDto?> GetCartByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        ApplicationUser? user = await this._repository.FindByIdAsync<ApplicationUser>(userId);
        if (user == null)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? cart = await this._repository.All<Cart>()
            .Include(c => c.CartProducts)
            .ThenInclude(cp => cp.Product)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
        if(cart == null)
            return null;

        return new CartDto()
        {
            CartId = cart.Id,
            TotalPrice = cart.CartProducts
                .Select(cp => cp.ProductQuantityAdded * cp.Product.SellingPrice)
                .Sum(),
            CartProducts = cart.CartProducts
                .OrderBy(cp => cp.AddedOn)
                .Select(cp => new CartProductDto()
                {
                    Id = cp.ProductId,
                    Name = cp.Product.Name,
                    QuantityAdded = cp.ProductQuantityAdded,
                    UnitPrice = cp.Product.SellingPrice,
                    TotalPrice = cp.ProductQuantityAdded * cp.Product.SellingPrice,
                    AddedOn = cp.AddedOn
                }),
        };
    }
    
    public async Task AddProductToCartAsync(Guid userId, Guid productId, int prodQtyToAdd)
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
                string.Format(OwnerCantAddToCartProductHeOwnsMessage, userId, productId));
        }
        
        if (!product.IsEnabled)
            throw new ProductDisabledException(productId, userId);

        Cart? cart = await this._repository.All<Cart>()
            .Include(c => c.CartProducts)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
        if (cart == null)
        {
            cart = new Cart() { CartOwnerId = userId };
            await this._repository.AddAsync(cart);
        }
        
        int userAlreadyAddedProdQty = cart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == productId)?
            .ProductQuantityAdded ?? 0;

        if (product.QuantityInStock < prodQtyToAdd + userAlreadyAddedProdQty)
        {
            throw new InsufficientProductQuantityInStockException(
                userId: userId,
                productId: productId,
                productQtyInStock: product.QuantityInStock,
                productQtyRequested: prodQtyToAdd + userAlreadyAddedProdQty);
        }

        try
        {
            if (userAlreadyAddedProdQty > 0)
            {
                cart.CartProducts
                    .Single(cp => cp.ProductId == productId)
                    .ProductQuantityAdded += prodQtyToAdd;
            }
            else
            {
                CartProduct newCartProduct = new CartProduct()
                {
                    ProductId = productId,
                    ProductQuantityAdded = prodQtyToAdd,
                };

                cart.CartProducts.Add(newCartProduct);
                await this._repository.AddAsync(newCartProduct);
            }

            await this._repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException concurrencyEx)
        {
            EntityEntry originalEntry = concurrencyEx.Entries.Single();
            if (originalEntry is EntityEntry<Product> originalProductEntry)
            {
                Product originalProductEntity = originalProductEntry.Entity;
                PropertyValues? propsDbValues = await originalProductEntry.GetDatabaseValuesAsync();

                bool isHardOrSoftDeleted = propsDbValues == null ||
                                           propsDbValues[nameof(originalProductEntity.IsDeleted)] is true;
                if (isHardOrSoftDeleted)
                {
                    throw new ResourceNotFoundException(
                        nameof(Product),
                        originalProductEntry.Entity.Id,
                        concurrencyEx);
                }

                bool prodIsEnabled 
                    = propsDbValues![nameof(originalProductEntity.IsEnabled)] is true;
                if (!prodIsEnabled)
                {
                    throw new ProductDisabledException(originalProductEntity.Id,
                        userId, concurrencyEx);
                }

                int qtyInStock 
                    = (int)(propsDbValues[nameof(originalProductEntity.QuantityInStock)] ?? 0);
                bool isEnoughQtyLeft = qtyInStock >= prodQtyToAdd;
                if (!isEnoughQtyLeft)
                {
                    throw new InsufficientProductQuantityInStockException(
                        userId: userId,
                        productId: originalProductEntity.Id,
                        productQtyInStock: qtyInStock,
                        productQtyRequested: prodQtyToAdd,
                        innerException: concurrencyEx);
                }
                
                originalProductEntry.OriginalValues.SetValues(propsDbValues);
                await this._repository.SaveChangesAsync();
            }
            
            // unhandled case
            throw new InvalidOperationException(string.Format(UnhandledExceptionMessage,
                nameof(InvalidOperationException)), concurrencyEx);
        }
    }
    
    public async Task RemoveProductFromCartAsync(Guid userId, Guid productId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(productId)));
        
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
                string.Format(OwnerCantRemoveProductHeOwnsFromCartMessage, userId, productId));
        }
        
        Cart? userCart = await this._repository.All<Cart>()
            .Include(c => c.CartProducts)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
        if (userCart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        CartProduct? cartProduct = userCart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == productId);
        if (cartProduct == null)
            throw new ArgumentException(string.Format(ProductNotFoundInCartMessage, productId));

        userCart.CartProducts.Remove(cartProduct);

        if (!userCart.CartProducts.Any())
            this._repository.Remove(userCart);

        await this._repository.SaveChangesAsync();
    }

    public async Task DeleteCart(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        ApplicationUser? user = await this._repository.All<ApplicationUser>()
            .Include(u => u.Cart)
            .SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? userCart = await this._repository.FindByIdAsync<Cart>(user.Cart!.Id);
        if (userCart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        this._repository.Remove<Cart>(userCart);
        await this._repository.SaveChangesAsync();
    }

    public async Task<bool> IsValidProductQtyToAddToCartAsync(Guid userId,
        ProductQtyValidationDto model)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if (model.Id == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(model.Id)));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._repository.FindByIdAsync<Product>(model.Id);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), model.Id);

        Cart? userCart = await this._repository.All<Cart>()
            .Include(c => c.CartProducts)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
        if (userCart == null)
        {
            return model.QuantityRequested > 0 &&
                   product.QuantityInStock > model.QuantityRequested;
        }

        int productQtyAlreadyAdded = userCart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == product.Id)?
            .ProductQuantityAdded ?? 0;

        return model.QuantityRequested > 0 && 
               model.QuantityRequested <= product.QuantityInStock - productQtyAlreadyAdded;
    }
}