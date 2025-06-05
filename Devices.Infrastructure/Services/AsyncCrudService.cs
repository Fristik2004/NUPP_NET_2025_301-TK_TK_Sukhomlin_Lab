using Devices.Infrastructure.Contracts;

namespace Devices.Infrastructure.Services;

public class AsyncCrudService<T> : ICrudServiceAsync<T> where T : class
{
    public AsyncCrudService(IRepository<T> repository)
    {
        Repository = repository;
    }
    
    public IRepository<T> Repository { get; }

    public Task<bool> CreateAsync(T element)
    {
        return Repository.AddAsync(element);
    }

    public Task<T> ReadAsync(Guid id)
    {
        return Repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        return Repository.GetAllAsync();
    }
    
    public Task<bool> UpdateAsync(T element)
    {
        return Repository.UpdateAsync(element);
    }

    public Task<bool> RemoveAsync(T element)
    {
        return Repository.DeleteAsync(element);
    }
}