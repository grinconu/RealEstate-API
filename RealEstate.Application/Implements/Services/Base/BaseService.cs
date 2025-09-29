using System.Linq.Expressions;
using MapsterMapper;
using RealEstate.Domain.DTOs.Base;
using RealEstate.Domain.Entities.Base;
using RealEstate.Domain.Interfaces.Repositories.Base;
using RealEstate.Domain.Interfaces.Services.Base;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Application.Implements.Services.Base;

public class BaseService<TEntity, TKey>(IBaseRepository<TEntity, TKey> repository, IMapper mapper) : IBaseService<TEntity, TKey> 
    where TEntity : BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    public async Task<Result<bool>> AnyAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await repository.AnyAsync(filter);
    }
    
    public async Task<Result<TKey>> InsertAsync(TEntity entity)
    {
        return await repository.InsertAsync(entity);
    }

    public async Task<Result<TKey>> InsertTransactAsync(TEntity entity)
    {
        return await repository.InsertTransactAsync(entity);
    }

    public async Task<Result<TKey>> InsertAsync<TModel>(TModel model) where TModel : class
    {
        var entity = mapper.Map<TEntity>(model);
        return await InsertAsync(entity);
    }

    public async Task<Result<TKey>> InsertTransactAsync<TModel>(TModel model) where TModel : class
    {
        var entity = mapper.Map<TEntity>(model);
        return await InsertAsync(entity);
    }

    public async Task<Result<Unit>> UpdateTransactAsync<TModel>(TModel model) where TModel : IPersistenceUpdate<TKey>
    {
        var entityDb = await repository.FindAsync(model.Id);
        mapper.Map(model, entityDb);
        return await UpdateTransactAsync(entityDb);
    }
    
    public async Task<Result<Unit>> UpdateTransactAsync(TEntity entity)
    {
        await repository.UpdateTransactAsync(entity);
        return Result.Unit;
    }
}

public class BaseService<TEntity>(IBaseRepository<TEntity, Guid> repository, IMapper mapper) 
    : BaseService<TEntity, Guid>(repository, mapper), IBaseService<TEntity> where TEntity : BaseEntity;