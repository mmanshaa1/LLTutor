using App.Core.Interfaces;
using App.Core.Specifications;
using App.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
    {
        private readonly MainContext mainContext;

        // ASK CLR to create object from MainContext implicitly
        public GenericRepository(MainContext mainContext)
        {
            this.mainContext = mainContext;
        }

        public async Task AddAsync(T entity)
            => await mainContext.AddAsync(entity);

        public void Delete(T entity)
            => mainContext.Remove(entity);

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await mainContext.Set<T>().ToListAsync();

        public void Update(T entity)
            => mainContext.Update(entity);

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
            => SpecificationEvaluator<T>.GetQuery(mainContext.Set<T>(), spec);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).ToListAsync();

        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).FirstOrDefaultAsync();

        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).CountAsync();

        public async Task<T?> GetByIdAsync(int Id)
            => await mainContext.Set<T>().FindAsync(Id);

        public async Task<int> GetCountAsync()
            => await mainContext.Set<T>().CountAsync();

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
            => await mainContext.Set<T>().AnyAsync(expression);

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
            => mainContext.Set<T>().Where(expression);

        public Task<int> GetCountAsync(Expression<Func<T, bool>> expression)
            => mainContext.Set<T>().CountAsync(expression);
    }
}
