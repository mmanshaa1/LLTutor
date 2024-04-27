using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Specifications.Specs
{
    public class OTPSpec : BaseSpecifications<OTPForConfirm>
    {
        public OTPSpec(Expression<Func<OTPForConfirm, bool>> expression) : base(expression)
        {

        }
    }
}
