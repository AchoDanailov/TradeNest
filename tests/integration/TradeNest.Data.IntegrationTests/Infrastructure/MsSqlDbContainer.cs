using Testcontainers.MsSql;

namespace TradeNest.Data.IntegrationTests.Infrastructure;

internal sealed class MsSqlDbContainer
{
    private static MsSqlContainer? _dbContainer;
    private static readonly object? _lock = new object();

    private MsSqlDbContainer() { }

    internal static MsSqlContainer Instance()
    {
        if (_dbContainer == null)
        {
            lock (_lock)
            {
                if (_dbContainer == null)
                {
                    // NOTE: Setting db name is required so the tests don't run in master db which can lead to bugs (like trying to drop master db between tests, which will lead to exceptions)
                    _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                        .WithDatabase("TradeNestTestDb") 
                        .Build();
                }
            }
        }
        
        return _dbContainer;
    }
}