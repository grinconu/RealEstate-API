using System.Linq.Expressions;
using RealEstate.Domain.DTOs.Base;
using RealEstate.Domain.Entities.Base;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Domain.Interfaces.Services.Base;

public interface IBaseService<TEntity, TKey> where TEntity : BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> filter);
    Task<Result<TKey>> InsertAsync(TEntity entity);
    Task<Result<TKey>> InsertTransactAsync(TEntity entity);
    Task<Result<TKey>> InsertAsync<TModel>(TModel model) where TModel : class;
    Task<Result<TKey>> InsertTransactAsync<TModel>(TModel model) where TModel : class;
    Task<Result<Unit>> UpdateTransactAsync<TModel>(TModel model)
        where TModel : IPersistenceUpdate<TKey>;
}

public interface IBaseService<TEntity> : IBaseService<TEntity, Guid> where TEntity : BaseEntity<Guid>;