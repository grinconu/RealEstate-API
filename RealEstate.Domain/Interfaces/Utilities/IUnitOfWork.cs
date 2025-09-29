namespace RealEstate.Domain.Interfaces.Utilities;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}