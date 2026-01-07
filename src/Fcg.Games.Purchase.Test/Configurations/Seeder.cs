using Fcg.Games.Purchase.Infra.Contexts;

namespace Fcg.Games.Purchase.Test.Configurations;

public static class Seeder
{
    private static readonly object _lock = new();

    public static AppDbContext Seed(this AppDbContext db)
    {
        lock (_lock)
        {
            
        }

        return db;
    }
}
