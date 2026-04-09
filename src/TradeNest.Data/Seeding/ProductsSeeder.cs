using System.Reflection;
using System.Text.Json;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public class ProductsSeeder : BaseEntitySeeder, IProductsSeeder
{
    private readonly IProductsRepository _productsRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUsersRepository _usersRepository;
    
    public ProductsSeeder(IProductsRepository productsRepository,
        ICategoriesRepository categoriesRepository, IUsersRepository usersRepository)
    {
        this._productsRepository = productsRepository;
        this._categoriesRepository = categoriesRepository;
        this._usersRepository = usersRepository;
    }

    public override string? PathToFile { get; protected set; }
    
    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "products.json");
        
        string? dataAsJsonString = await this.GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));
        
        IEnumerable<ProductImportDto>? productDtos =
            JsonSerializer.Deserialize<IEnumerable<ProductImportDto>>(dataAsJsonString);
        if (productDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        ICollection<Product> productsToImport = new List<Product>();
        foreach (ProductImportDto productDto in productDtos)
        {
            if (!IsValid(productDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            if (productDto.Id == Guid.Empty ||
                productDto.CategoryId == Guid.Empty ||
                productDto.OwnerId == Guid.Empty ||
                productDto.ApprovalDecisionMakerId == Guid.Empty ||
                productDto.Images.Any(i => i.Id == Guid.Empty))
            {
                throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                    nameof(productDto.Id)));
            }

            bool productExists = await this._productsRepository
                .ExistsIncludingArchivedAndNotApprovedAsync(p => p.Id == productDto.Id);
            if(productExists)
                continue;

            Category? category = await this._categoriesRepository
                .FindByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException(string.Format(NotFoundMessage,
                    nameof(Category), productDto.CategoryId));
            }

            ApplicationUser? owner = await this._usersRepository
                .FindByIdWithForgottenIncludedAsync(productDto.OwnerId);
            if (owner == null)
            {
                throw new ArgumentException(string.Format(NotFoundMessage,
                    nameof(ApplicationUser), productDto.OwnerId));
            }

            bool isValidApprovalStatus = Enum.TryParse(
                productDto.ApprovalDecision.ApprovalStatus.ToString(),
                out ApprovalStatus validApprovalStatus);
            if (!isValidApprovalStatus)
                throw new ArgumentException(nameof(productDto.ApprovalDecision.ApprovalStatus));

            bool createdOnIsValidDate = CreatedOnNotAfterTimeOfDecision(productDto.CreatedOn,
                productDto.ApprovalDecision.TimeOfDecision);
            if (!createdOnIsValidDate)
            {
                throw new InvalidOperationException(
                    string.Format(ProductCreatedOnAfterApprovalTimeOfDecision, productDto.Id));
            }
            
            ICollection<Image> imagesToImport = new List<Image>();
            foreach (ImageImportDto imageDto in productDto.Images)
            {
                imagesToImport.Add(new Image()
                {
                    Id = imageDto.Id,
                    Url = imageDto.Url,
                    IsFrontImage = imageDto.IsFrontImage,
                    ProductId = productDto.Id
                });
            }
            
            productsToImport.Add(new Product()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                QuantityInStock = productDto.QuantityInStock,
                CostPrice = productDto.CostPrice,
                SellingPrice = productDto.SellingPrice,
                CategoryId = productDto.CategoryId,
                OwnerId = productDto.OwnerId,
                ApprovalDecisionMakerId = productDto.ApprovalDecisionMakerId,
                ApprovalDecision = new ApprovalDecision()
                {
                    ApprovalStatus = validApprovalStatus,
                    DecisionJustification = productDto.ApprovalDecision.DecisionJustification,
                    TimeOfDecision = productDto.ApprovalDecision.TimeOfDecision
                },
                CreatedOn = productDto.CreatedOn,
                IsEnabled = productDto.IsEnabled,
                IsDeleted = productDto.IsDeleted,
                Images = imagesToImport,
            });   
        }

        if (productsToImport.Any())
        {
            bool addProductsResult = await this._productsRepository
                .AddRangeAsync(productsToImport);
            if (!addProductsResult)
                throw new DataPersistException(nameof(addProductsResult), this.GetType().Name);
        }
    }

    private static bool CreatedOnNotAfterTimeOfDecision(
        DateTime createdOn,
        DateTime? timeOfDecision)
    {
        // ReSharper disable once SimplifyConditionalTernaryExpression
        bool createdOnIsNotAfterTimeOfDecision = timeOfDecision != null
            ? createdOn <= timeOfDecision
            : true;

        return createdOnIsNotAfterTimeOfDecision;
    }
}