using Core.Entities;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> GetOneAsync(ISpecification<T> specification);
    Task<IReadOnlyList<T>> GetManyAsync(ISpecification<T> specification);
    Task<int> CountAsync(ISpecification<T> specification);
}
