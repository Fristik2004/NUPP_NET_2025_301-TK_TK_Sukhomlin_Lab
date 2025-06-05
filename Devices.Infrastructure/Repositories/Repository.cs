using Devices.Infrastructure.Contracts;
using Devices.Infrastructure.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public Repository(DevicesContext context)
    {
        Context = context;
        Entries = context.Set<T>();
    }
    
    public DevicesContext Context { get; }
    public DbSet<T> Entries { get; set; }
    
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await Entries.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entries.ToListAsync();
    }

    public async Task<bool> AddAsync(T entity)
    {
        Entries.Add(entity);
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        Entries.Update(entity);
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        Entries.Remove(entity);
        return await Context.SaveChangesAsync() > 0;
    }
}