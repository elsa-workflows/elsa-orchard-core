using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Modules;

namespace Elsa.OrchardCore.Features.Core.StartupTasks;

/// <summary>
/// Executes EF Core migrations using the specified <see cref="DbContext"/> type.
/// </summary>
public class RunMigrationsStartupTask<TDbContext> : ModularTenantEvents where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _dbContextFactory;
    public RunMigrationsStartupTask(IDbContextFactory<TDbContext> dbContextFactoryFactory) => _dbContextFactory = dbContextFactoryFactory;

    public override async Task ActivatingAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        await dbContext.DisposeAsync();
    }
}