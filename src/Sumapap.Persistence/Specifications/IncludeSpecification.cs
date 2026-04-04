using System.Linq.Expressions;
using Sumapap.Persistence.Specifications;

namespace Sumapap.Persistence.Specification
{
    public class IncludeSpecification<T> : BaseSpecification<T>
    {
        public IncludeSpecification(IList<string> includes)
            : base(includes)
        {
        }

        public IncludeSpecification(Expression<Func<T, bool>> criteria, IList<string> includes)
            : base(criteria, includes)
        {
        }
    }
}
