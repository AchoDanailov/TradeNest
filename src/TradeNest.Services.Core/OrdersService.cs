using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Cart.Enums;
using TradeNest.Services.Models.Order;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICartsRepository _cartsRepository;
    private readonly IOrdersMapper _ordersMapper;

    public OrdersService(
        IOrdersRepository ordersRepository,
        IUsersRepository usersRepository,
        ICartsRepository cartsRepository,
        IOrdersMapper ordersMapper)
    {
        this._ordersRepository = ordersRepository;
        this._usersRepository = usersRepository;
        this._cartsRepository = cartsRepository;
        
        this._ordersMapper = ordersMapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        IEnumerable<Order> orders = await this._ordersRepository
            .GetAllAsReadOnlyAsync(queryOptions => 
                queryOptions
                    .WithRelated(o => o.OrderProducts)
                    .AddFilter(o => o.UserId == userId)
                    .AddOrderDesc(o => o.SubmittedOn)
                    .AddOrderDesc(o => o.TotalPrice)
                    .AddOrderAsc(o => o.OrderProducts.Count));
        
        return this._ordersMapper.ToOrderDtos(orders);
    }

    public async Task<bool> OrderExistsByIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
            return false;

        return await this._ordersRepository.ExistsAsync(o => o.Id == orderId);
    }
    
    public async Task<SubmitOrderResultDto> SubmitOrderAsync(Guid userId)
    {
        try
        {
            return await this.TrySubmitOrder(userId);
        }
        catch (DataConcurrencyConflictException)
        {
            return await this.RetrySubmitOrder(userId);
        }
    }
    
    private async Task<SubmitOrderResultDto> RetrySubmitOrder(Guid userId)
    {
        try
        {
            return await this.TrySubmitOrder(userId);
        }
        catch (DataConcurrencyConflictException concurrencyEx)
        {
            throw new DataPersistException(innerException: concurrencyEx, $"userId: {userId}");
        }
    }

    private async Task<SubmitOrderResultDto> TrySubmitOrder(Guid userId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Cart? userCart = await this._cartsRepository
            .GetUserCartWithProductsDetailsAsync(userId);
        if (userCart == null)
            throw new ArgumentException(string.Format(EmptyCartMessage, userId));

        ICollection<OrderProduct> orderProducts = new List<OrderProduct>();
        ICollection<CartProduct> cartProductsToRemove = new List<CartProduct>();
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

            OrderProduct newOrderProduct = this._ordersMapper
                .OrderProductFromCartProduct(cartProduct);
            orderProducts.Add(newOrderProduct);

            cartProductsToRemove.Add(cartProduct);
        }

        if (errorProducts.Any())
            return SubmitOrderResultDto.Failure(errorProducts: errorProducts);
            
        Order newOrder = new Order()
        {
            UserId = userId,
            OrderProducts = orderProducts,
            TotalPrice = orderProducts.Sum(op => op.TotalProductPriceAtOrderTime),
            SubmittedOn = DateTime.UtcNow
        };

        foreach (CartProduct cartProductToRemove in cartProductsToRemove)
            userCart.CartProducts.Remove(cartProductToRemove);

        bool addNewOrderResult = await this._ordersRepository.AddAsync(newOrder);
        if (addNewOrderResult == false)
        {
            throw new DataPersistException(nameof(addNewOrderResult),
                $"{nameof(userId)}: {userId}", $"cartId: {userCart.Id}");
        }

        return SubmitOrderResultDto.Success();
    }
}