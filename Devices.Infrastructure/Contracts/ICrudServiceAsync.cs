namespace Devices.Infrastructure.Contracts;

public interface ICrudServiceAsync<T>
{
    Task<bool> CreateAsync(T element);
    Task<T> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
}