using App.Core.Interfaces;
using App.Repository.Data.Contexts;
using System.Collections;

namespace App.Repository
{
    public class UnitOfWork(MainContext dbContext) : IUnitOfWork
    {
        private readonly MainContext _dbContext = dbContext;
        private readonly Hashtable repositories = [];
        private readonly Hashtable dbSets = [];

        public IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : class, IBaseEntity
        {
            var type = typeof(TEntity).Name;

            if (!repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(_dbContext);
                repositories.Add(type, repository);
            }

            return repositories[type] as IGenericRepository<TEntity>;
        }

        public IQueryable<TEntity>? Set<TEntity>() where TEntity : class, IBaseEntity
        {
            var type = typeof(TEntity).Name;

            if (!dbSets.ContainsKey(type))
            {
                var dbSet = _dbContext.Set<TEntity>();
                dbSets.Add(type, dbSet);
            }

            return dbSets[type] as IQueryable<TEntity>;
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();
    }
}
