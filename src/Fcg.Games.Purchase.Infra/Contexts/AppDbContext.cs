using Fcg.Games.Purchase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Games.Purchase.Infra.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<EventoDeCompraEntity> EventsStore => Set<EventoDeCompraEntity>();
    public DbSet<TransacaoJogosEntity> Aquisicoes => Set<TransacaoJogosEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.EnableSensitiveDataLogging();
    }
}