using Devices.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.DataContexts;

public class DevicesContext : DbContext
{
    public DevicesContext(DbContextOptions<DevicesContext> options) : base(options)
    {
        
    }
    
    public DbSet<Charger> Chargers { get; set; }
    public DbSet<Laptop> Laptops { get; set; }
    public DbSet<Smartphone> Smartphones { get; set; }
}