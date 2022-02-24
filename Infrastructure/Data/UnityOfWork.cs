using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Infrastructure.Data;
public class UnityOfWork : IUnitOfWork
{
    private readonly StoreContext storeContext;
    private Hashtable repositories;

    public UnityOfWork(StoreContext storeContext)
    {
        this.storeContext = storeContext;
    }

    public IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity
    {
        if (repositories == null)
        {
            repositories = new Hashtable();
        }

        string typeName = typeof(TEntity).Name;

        if (!repositories.ContainsKey(typeName))
        {
            Type repositoryType = typeof(GenericRepository<>);
            object? repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), storeContext);
            repositories.Add(typeName, repositoryInstance);
        }

        return (IGenericRepository<TEntity>?)repositories[typeName];
    }

    public async Task<int> Complete()
    {
        return await storeContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        storeContext.Dispose();
    }
}
