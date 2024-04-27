using App.Core.Interfaces;
using System.Linq.Expressions;

namespace App.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : class, IBaseEntity
    {
        public Expression<Func<T, bool>> Crieteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; set; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagingEnabled { get; set; }
        public BaseSpecifications()
        {

        }

        public BaseSpecifications(Expression<Func<T, bool>> CrieteriaExp)
        {
            Crieteria = CrieteriaExp;
        }

        public void AddInclude(Expression<Func<T, object>> IncludeExp)
        {
            Includes.Add(IncludeExp);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            OrderByDesc = null;
        }

        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
            OrderBy = null;
        }

        public void ApplyPaging(int pageIndex, int pageSize)
        {
            Skip = pageSize * (pageIndex - 1);
            Take = pageSize;
            IsPagingEnabled = true;
        }
    }
}
