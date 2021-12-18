using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data;

public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        IQueryable<TEntity> query = inputQuery;

        // First filter.
        if (specification.Crieteria != null)
        {
            query = query.Where(specification.Crieteria);
        }
        
        // Then sort.
        if (specification.OrderByAscending != null)
        {
            query = query.OrderBy(specification.OrderByAscending);
        }

        if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // And only then make paging.
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        return query;
    }
}
