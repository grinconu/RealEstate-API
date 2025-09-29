using Microsoft.EntityFrameworkCore.Storage;
using RealEstate.Domain.Interfaces.Utilities;
using RealEstate.Infrastructure.Context;

namespace RealEstate.Infrastructure.Utilities;

public class UnitOfWork(
    RealEstateDbContext dbContext
    ) : IUnitOfWork, IDisposable
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction ??= await dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await dbContext.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }
        finally
        {
            await _transaction!.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}