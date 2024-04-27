using App.Core.Interfaces;
using System.Linq.Expressions;

namespace App.Core.Specifications
{
    public interface ISpecification<T> where T : class, IBaseEntity
    {
        public Expression<Func<T, bool>> Crieteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagingEnabled { get; set; }
    }
}
