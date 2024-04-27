using App.Core.Specifications;
using System.Linq.Expressions;

namespace App.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class, IBaseEntity
    {
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec);
        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);
        Task<int> GetCountAsync();
        Task<int> GetCountAsync(Expression<Func<T, bool>> expression);
        Task<T?> GetByIdAsync(int Id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
