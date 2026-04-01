using System.ComponentModel.DataAnnotations;

using TradeNest.Data.Seeding.Interfaces;

namespace TradeNest.Data.Seeding;

public abstract class BaseEntitySeeder : IEntitySeeder
{
    public abstract string? PathToFile { get; protected set; } 
    
    public abstract Task SeedEntityDataAsync(string? pathToFile = null);

    protected async Task<string?> GetSeedDataFromFileAsync()
    {
        if(File.Exists(this.PathToFile))
            return await File.ReadAllTextAsync(this.PathToFile);

        return null;
    }
    
    protected static bool IsValid(object modelInstance)
    {
        ValidationContext validationContext = new ValidationContext(modelInstance);
        ICollection<ValidationResult> validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(modelInstance, validationContext, validationResults);
    }
}