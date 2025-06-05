using System.ComponentModel.DataAnnotations;

namespace Devices.Infrastructure.Models;

public abstract class Device
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }
    
    public double Price { get; set; }
}