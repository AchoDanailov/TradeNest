using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Cart;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Product;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class CartsService : ICartsService
{
    private ICartsRepository _cartsRepository;
    private IUsersRepository _usersRepository;
    private IProductsRepository _productsRepository;
    private ICartsMapper _cartsMapper;

    public CartsService(
        ICartsRepository cartsRepository,
        IUsersRepository usersRepository,
        IProductsRepository productsRepository,
        ICartsMapper cartsMapper)
    {
        this._cartsRepository = cartsRepository;
        this._productsRepository = productsRepository;
        this._usersRepository = usersRepository;
        
        this._cartsMapper = cartsMapper;
    }

    public async Task<CartDto?> GetCartByUserIdAsync(Guid userId) 
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        ApplicationUser? user = await this._usersRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? cart = await this._cartsRepository
            .GetUserCartWithProductsDetailsAsync(userId, asReadOnly: true);
        if(cart == null)
            return null;

        CartDto cartDto = this._cartsMapper.ToCartDto(cart);
        cartDto.CartProducts = cartDto.CartProducts.OrderBy(cp => cp.AddedOn);

        return cartDto;
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

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._productsRepository.FindByIdAsync(productId);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productId);

        if (product.OwnerId == userId)
        {
            throw new InvalidOperationException(
                string.Format(OwnerCantAddToCartProductHeOwnsMessage, userId, productId));
        }
        
        if (!product.IsEnabled)
            throw new ProductDisabledException(productId, userId);

        bool isNewCart = false;
        
        Cart? cart = await _cartsRepository.GetCartWithCartProductsByUserIdAsync(userId);
        if (cart == null)
        {
            isNewCart = true;
            cart = new Cart() { CartOwnerId = userId };
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
        }
        
        if (isNewCart) 
        {
            bool addCartResult = await this._cartsRepository.AddAsync(cart);
            if (addCartResult == false)
            {
                throw new DataPersistException(nameof(addCartResult), 
                    $"{nameof(userId)}: {userId}");
            }
        }
        else
        {
            bool updateCartResult = await this._cartsRepository.UpdateAsync(cart);
            if (updateCartResult == false)
            {
                throw new DataPersistException(nameof(updateCartResult), 
                    $"{nameof(userId)}: {userId}, cartId: {cart.Id}");
            }
        }
    }
    
    public async Task RemoveProductFromCartAsync(Guid userId, Guid productId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(productId)));

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._productsRepository.FindByIdAsync(productId);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productId);

        if (product.OwnerId == userId)
        {
            throw new InvalidOperationException(
                string.Format(OwnerCantRemoveProductHeOwnsFromCartMessage, userId, productId));
        }

        Cart? userCart = await _cartsRepository.GetCartWithCartProductsByUserIdAsync(userId);
        if (userCart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        CartProduct? cartProduct = userCart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == productId);
        if (cartProduct == null)
            throw new ArgumentException(string.Format(ProductNotFoundInCartMessage, productId));

        userCart.CartProducts.Remove(cartProduct);

        bool updateCartResult = await this._cartsRepository.UpdateAsync(userCart);
        if (updateCartResult == false) 
        {
            throw new DataPersistException(nameof(updateCartResult), 
                $"{nameof(userId)}: {userId}, cartId: {userCart.Id}");
        }
    }

    public async Task DeleteCartAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        ApplicationUser? user = (await this._usersRepository.GetAllAsync(options => 
                options
                    .WithRelated(u => u.Cart!)
                    .AddFilter(u => u.Id == userId)))
            .SingleOrDefault();
        if (user == null)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        if (user.Cart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        bool deleteCartResult = await this._cartsRepository.DeleteAsync(user.Cart);
        if (deleteCartResult == false)
        {
            throw new DataPersistException(nameof(deleteCartResult), 
                $"{nameof(userId)}: {userId}, cartId: {user.Cart.Id}");
        }
    }

    public async Task<bool> IsValidProductQtyToAddToCartAsync(Guid userId,
        ProductQtyValidationDto model)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if (model.Id == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(model.Id)));

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._productsRepository.FindByIdAsync(model.Id);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), model.Id);

        Cart? userCart = await _cartsRepository.GetCartWithCartProductsByUserIdAsync(userId);
        if (userCart == null)
        {
            return model.QuantityRequested > 0 &&
                   product.QuantityInStock >= model.QuantityRequested;
        }

        int productQtyAlreadyAdded = userCart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == product.Id)?
            .ProductQuantityAdded ?? 0;

        return model.QuantityRequested > 0 && 
               model.QuantityRequested <= product.QuantityInStock - productQtyAlreadyAdded;
    }

    public async Task<bool> UpdateCartProductAsync(Guid userId, UpdateCartProductDto updateCartProductDto)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if (updateCartProductDto.ProductId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                nameof(updateCartProductDto.ProductId)));
        }
        if (updateCartProductDto.CartId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                nameof(updateCartProductDto.CartId)));
        }
        if (updateCartProductDto.Quantity <= 0)
        {
            throw new ArgumentException(string.Format(CantBeZeroOrNegativeNumberMessage,
                nameof(updateCartProductDto.Quantity)));
        }

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? userCart = await this._cartsRepository
            .GetCartWithProductsDetailsAsync(updateCartProductDto.CartId);
        if (userCart == null)
        {
            throw new ResourceNotFoundException(nameof(Cart), 
                updateCartProductDto.CartId);
        }

        if (userCart.CartOwnerId != userId)
            throw new UnauthorizedOperationException(userId, nameof(Cart), userCart.Id);

        CartProduct? targetCartProduct = userCart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == updateCartProductDto.ProductId);
        if (targetCartProduct == null)
        {
            throw new ResourceNotFoundException(nameof(CartProduct),
                updateCartProductDto.ProductId);
        }

        if (targetCartProduct.Product.QuantityInStock < updateCartProductDto.Quantity)
        {
            throw new InsufficientProductQuantityInStockException(
                userId: userId,
                productId: targetCartProduct.ProductId,
                productQtyInStock: targetCartProduct.Product.QuantityInStock,
                productQtyRequested: updateCartProductDto.Quantity);
        }
            
        targetCartProduct.ProductQuantityAdded = updateCartProductDto.Quantity;

        bool updateCartResult = await this._cartsRepository.UpdateAsync(userCart);
        if (!updateCartResult)
        {
            throw new DataPersistException(nameof(updateCartResult),
                $"{nameof(userId)}: {userId}, cartId: {userCart.Id}");
        }

        return updateCartResult;
    }

    public async Task<CartProductDto?> GetCartProductDataByUserIdAndProductId(Guid userId, Guid productId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(productId)));

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? cart = await this._cartsRepository
            .GetUserCartWithProductsDetailsAsync(userId, asReadOnly: true);
        if (cart == null)
            return null;

        CartProduct? targetCartProduct = cart.CartProducts
            .SingleOrDefault(cp => cp.ProductId == productId);
        if (targetCartProduct == null)
            return null;

        return this._cartsMapper.ToCartProductDto(targetCartProduct);
    }
}