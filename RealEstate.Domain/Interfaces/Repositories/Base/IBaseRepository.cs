using System.Linq.Expressions;
using RealEstate.Domain.Entities.Base;

namespace RealEstate.Domain.Interfaces.Repositories.Base;

public interface IBaseRepository<TEntity, TKey>
{
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);
    Task<TKey> InsertAsync(TEntity entity);
    Task<TKey> InsertTransactAsync(TEntity entity);
    Task<TEntity> FindAsync(TKey id);
    Task<bool> UpdateTransactAsync(TEntity entity);
}

public interface IBaseRepository<TEntity> : IBaseRepository<TEntity, Guid> where TEntity : BaseEntity;