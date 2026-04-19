using System.Reflection;
using System.Text.Json;

using TradeNest.Data.Models;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public class CartsSeeder : BaseEntitySeeder, ICartsSeeder
{
    private readonly ICartsRepository _cartsRepository;
    private readonly IProductsRepository _productsRepository;

    public CartsSeeder(ICartsRepository cartsRepository, IProductsRepository productsRepository)
    {
        this._cartsRepository = cartsRepository;
        this._productsRepository = productsRepository;
    }

    public override string? PathToFile { get; protected set; }

    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "carts.json");

        string? dataAsJsonString = await this.GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));

        IEnumerable<CartImportDto>? cartDtos =
            JsonSerializer.Deserialize<IEnumerable<CartImportDto>>(dataAsJsonString);
        if (cartDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        ICollection<Cart> cartsToImport = new List<Cart>();
        foreach (CartImportDto cartDto in cartDtos)
        {
            if (!IsValid(cartDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            bool cartExists = await this._cartsRepository
                .ExistsAsync(c => c.Id == cartDto.Id && c.CartOwnerId == cartDto.CartOwnerId);
            if (cartExists)
                continue;

            Cart cart = new Cart()
            {
                Id = cartDto.Id,
                CartOwnerId = cartDto.CartOwnerId,
            };

            foreach (CartProductImportDto cpDto in cartDto.CartProducts)
            {
                bool productExists = await this._productsRepository
                    .ExistsIncludingArchivedAndNotApprovedAsync(p => p.Id == cpDto.ProductId);
                if (!productExists)
                {
                    throw new ArgumentException(string.Format(NotFoundMessage,
                        nameof(Product), cpDto.ProductId));
                }

                cart.CartProducts.Add(new CartProduct()
                {
                    CartId = cart.Id,
                    ProductId = cpDto.ProductId,
                    ProductQuantityAdded = cpDto.ProductQuantityAdded,
                    AddedOn = cpDto.AddedOn
                });
            }

            cartsToImport.Add(cart);
        }

        if (cartsToImport.Any())
        {
            bool result = await this._cartsRepository.AddRangeAsync(cartsToImport);
            if (!result)
            {
                throw new DataPersistException(nameof(result), this.GetType().Name);
            }
        }
    }
}