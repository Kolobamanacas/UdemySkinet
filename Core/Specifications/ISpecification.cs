using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Crieteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>> OrderByAscending { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    int Skip { get; }
    int Take { get; }
    bool IsPagingEnabled { get; }
}
