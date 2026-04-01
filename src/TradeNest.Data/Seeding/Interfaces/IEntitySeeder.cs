namespace TradeNest.Data.Seeding.Interfaces;

public interface IEntitySeeder
{
    string? PathToFile { get; }   
    
    Task SeedEntityDataAsync(string? pathToFile = null);
}