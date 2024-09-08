namespace Base.Contracts.Domain;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}