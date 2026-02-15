namespace TradeNest.Services.Core.Interfaces;

public interface ICategoriesService
{
    Task<bool> CategoryExists(Guid id);
}