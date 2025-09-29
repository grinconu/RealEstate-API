using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities.Base;
using RealEstate.Domain.Interfaces.Repositories.Base;

namespace RealEstate.Infrastructure.Repositories.Base;

public class BaseRepository<TEntity, TKey>(DbContext context) : IBaseRepository<TEntity, TKey>
where TEntity : BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();
    protected readonly DbContext Context = context;
    
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await DbSet.AnyAsync(expression);
    }

    public virtual async Task<TKey> InsertAsync(TEntity entity)
    {
        var response = (await DbSet.AddAsync(entity)).Entity.Id;
        await Context.SaveChangesAsync();
        return response;
    }

    public Task<TKey> InsertTransactAsync(TEntity entity)
    {
        var response = DbSet.Add(entity).Entity.Id;
        return Task.FromResult(response);
    }

    public virtual async Task<TEntity> FindAsync(TKey id)
    {
        return (await DbSet.FindAsync(id))!;
    }
    
    public Task<bool> UpdateTransactAsync(TEntity entity)
    {
        DbSet.Update(entity);
        return Task.FromResult(true);
    }
}

public class BaseRepository<TEntity> : BaseRepository<TEntity, Guid>, IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected BaseRepository(DbContext context) : base(context)
    {
    }
}