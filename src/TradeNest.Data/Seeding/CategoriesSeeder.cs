using System.Reflection;
using System.Text.Json;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Seeding.Dtos;
using TradeNest.Data.Seeding.Interfaces;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;

namespace TradeNest.Data.Seeding;

public class CategoriesSeeder : BaseEntitySeeder, ICategoriesSeeder
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoriesSeeder(ICategoriesRepository categoriesRepository)
    {
        this._categoriesRepository = categoriesRepository;
    }

    public override string? PathToFile { get; protected set; }
    
    public override async Task SeedEntityDataAsync(string? pathToFile = null)
    {
        string assemblyDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        this.PathToFile = pathToFile ?? 
                          Path.Combine(assemblyDirPath, "Seeding", "Datasets", "categories.json");
        
        string? dataAsJsonString = await this.GetSeedDataFromFileAsync();
        if (dataAsJsonString == null)
            throw new ArgumentException(string.Format(FileNotFound, pathToFile));
        
        IEnumerable<CategoryImportDto>? categoryDtos =
            JsonSerializer.Deserialize<IEnumerable<CategoryImportDto>>(dataAsJsonString);
        if (categoryDtos == null)
            throw new InvalidOperationException(string.Format(SeedingError, this.GetType().Name));

        IEnumerable<Category> categoriesInDb = (await this._categoriesRepository
                .GetAllAsync(queryOptions => queryOptions.AsReadOnly()))
            .ToArray();

        ICollection<Category> categoriesToImport = new List<Category>();
        foreach (CategoryImportDto categoryDto in categoryDtos)
        {
            if(!IsValid(categoryDto))
                throw new ArgumentException(string.Format(SeedingError, this.GetType().Name));

            if (categoryDto.Id == Guid.Empty)
            {
                throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                    nameof(categoryDto.Id)));
            }

            if (categoriesInDb.Any(c => c.Id == categoryDto.Id))
                continue;
            
            categoriesToImport.Add(new Category()
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name
            });
        }

        if (categoriesToImport.Any())
        {
            bool addCategoriesResult = await this._categoriesRepository
                .AddRangeAsync(categoriesToImport);
            if (!addCategoriesResult)
                throw new DataPersistException(nameof(addCategoriesResult), this.GetType().Name);
        }
    }
}