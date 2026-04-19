using System.Reflection;
using System.Text.Json;

using TradeNest.Data.Models;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public class OrdersSeeder : BaseEntitySeeder, IOrdersSeeder
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IProductsRepository _productsRepository;

    public OrdersSeeder(
        IOrdersRepository ordersRepository, 
        IUsersRepository usersRepository, 
        IProductsRepository productsRepository)
    {
        this._ordersRepository = ordersRepository;
        this._usersRepository = usersRepository;
        this._productsRepository = productsRepository;
    }

    public override string? PathToFile { get; protected set; }

    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "orders.json");

        string? dataAsJsonString = await this.GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));

        IEnumerable<OrderImportDto>? orderDtos =
            JsonSerializer.Deserialize<IEnumerable<OrderImportDto>>(dataAsJsonString);
        if (orderDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        ICollection<Order> ordersToImport = new List<Order>();
        foreach (OrderImportDto orderDto in orderDtos)
        {
            if (!IsValid(orderDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            bool orderExists = await this._ordersRepository
                .ExistsAsync(o => o.Id == orderDto.Id);
            if (orderExists)
                continue;

            bool userExists = await this._usersRepository
                .ExistsByIdWithForgottenIncludedAsync(orderDto.UserId);
            if (!userExists)
            {
                throw new ArgumentException(string.Format(NotFoundMessage,
                    nameof(ApplicationUser), orderDto.UserId));
            }

            Order order = new Order()
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                SubmittedOn = orderDto.SubmittedOn,
                TotalPrice = orderDto.TotalPrice,
            };

            foreach (OrderProductImportDto opDto in orderDto.OrderProducts)
            {
                bool productExists = await this._productsRepository
                    .ExistsIncludingArchivedAndNotApprovedAsync(p => p.Id == opDto.OriginalProductId);
                if (!productExists)
                {
                    throw new ArgumentException(string.Format(NotFoundMessage,
                        nameof(Product), opDto.OriginalProductId));
                }

                order.OrderProducts.Add(new OrderProduct()
                {
                    Id = opDto.Id,
                    OrderId = order.Id,
                    ProductNameAtOrderTime = opDto.ProductNameAtOrderTime,
                    OriginalProductId = opDto.OriginalProductId,
                    QuantityOrdered = opDto.QuantityOrdered,
                    CostPriceAtOrderTime = opDto.CostPriceAtOrderTime,
                    UnitSellingPriceAtOrderTime = opDto.UnitSellingPriceAtOrderTime
                });
            }

            ordersToImport.Add(order);
        }

        if (ordersToImport.Any())
        {
            bool result = await this._ordersRepository.AddRangeAsync(ordersToImport);
            if (!result)
            {
                throw new DataPersistException(nameof(result), this.GetType().Name);
            }
        }
    }
}