using System.Linq.Expressions;

namespace Sumapap.Persistence.Specifications
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
