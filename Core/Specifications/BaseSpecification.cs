using System.Linq.Expressions;

namespace Core.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Crieteria { get; }

    public BaseSpecification()
    {
    }

    public BaseSpecification(Expression<Func<T, bool>> crieteria)
    {
        Crieteria = crieteria;
    }

    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
    public Expression<Func<T, object>> OrderByAscending { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddOrderByAscending(Expression<Func<T, object>> orderByAscendingExpression)
    {
        OrderByAscending = orderByAscendingExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
