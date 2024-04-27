namespace App.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : class, IBaseEntity;
        IQueryable<TEntity>? Set<TEntity>() where TEntity : class, IBaseEntity;
        Task<int> CompleteAsync();
    }
}
