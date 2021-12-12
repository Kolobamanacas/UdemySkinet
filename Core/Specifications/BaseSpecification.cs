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

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
}
