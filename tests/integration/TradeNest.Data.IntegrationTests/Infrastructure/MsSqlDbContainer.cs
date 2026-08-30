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
                    _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                        .Build();
                }
            }
        }
        
        return _dbContainer;
    }
}