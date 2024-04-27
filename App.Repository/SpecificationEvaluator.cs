using App.Core.Interfaces;
using App.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace App.Repository
{
    public static class SpecificationEvaluator<TEntity> where TEntity : class, IBaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            if (spec is not null)
            {
                if (spec.Crieteria is not null)
                    query = query.Where(spec.Crieteria);

                if (spec.OrderBy is not null)
                    query = query.OrderBy(spec.OrderBy);

                else if (spec.OrderByDesc is not null)
                    query = query.OrderByDescending(spec.OrderByDesc);

                if (spec.IsPagingEnabled)
                    query = query.Skip(spec.Skip).Take(spec.Take);

                if (spec.Includes.Count != 0)
                    query = spec.Includes.Aggregate(query, (currentQuery, includeExp) => currentQuery.Include(includeExp));

                if (spec.IncludeStrings.Count != 0)
                    query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }
    }
}
