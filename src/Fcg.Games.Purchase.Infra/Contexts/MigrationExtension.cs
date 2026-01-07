using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Games.Purchase.Infra.Contexts;
public static class MigrationExtension
{
    public static async Task<WebApplication> ApplyMigrationsWithSeedsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!dbContext.Database.IsRelational())
            return app;

        dbContext.Database.Migrate();

        return app;
    }
}
