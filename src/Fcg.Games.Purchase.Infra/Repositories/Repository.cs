using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fcg.Games.Purchase.Infra.Repositories;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> ObterAsync(Expression<Func<T, bool>>? filtro = null)
    {
        IQueryable<T> query = _dbSet;

        if (filtro is not null)
            query = query.Where(filtro);

        return await query.ToListAsync();
    }

    public async Task<T?> ObterPorIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<T> AdicionarAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
        await _context.SaveChangesAsync();

        return entidade;
    }

    public async Task Atualizar(T entidade)
    {
        _dbSet.Update(entidade);
        await _context.SaveChangesAsync();
    }

    public async Task Remover(T entidade)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync();
    }
}