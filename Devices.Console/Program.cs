using Devices.Infrastructure.Contracts;
using Devices.Infrastructure.DataContexts;
using Devices.Infrastructure.Models;
using Devices.Infrastructure.Repositories;
using Devices.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

//var connection = builder.Configuration.GetConnectionString("SqlServerConnection")!;
var connection = builder.Configuration.GetConnectionString("MongoDbConnection")!;

builder.Services.AddDbContext<DevicesContext>(x => x
    //.UseSqlServer(connection)
    .UseMongoDB(connection, "Devices")
);

builder.Services.AddScoped<IRepository<Charger>, Repository<Charger>>();
builder.Services.AddScoped<IRepository<Smartphone>, Repository<Smartphone>>();
builder.Services.AddScoped<IRepository<Laptop>, Repository<Laptop>>();

builder.Services.AddScoped<ICrudServiceAsync<Charger>, AsyncCrudService<Charger>>();
builder.Services.AddScoped<ICrudServiceAsync<Smartphone>, AsyncCrudService<Smartphone>>();
builder.Services.AddScoped<ICrudServiceAsync<Laptop>, AsyncCrudService<Laptop>>();

var app = builder.Build();

using var scope = app.Services.CreateAsyncScope();

var service = scope.ServiceProvider.GetRequiredService<ICrudServiceAsync<Laptop>>();

List<Laptop> laptops =
[
    new Laptop { Id = Guid.CreateVersion7(), Name = "Laptop 7 ", Price = Random.Shared.Next(100, 400), Processor = "Inter Core i5"},
    new Laptop { Id = Guid.CreateVersion7(), Name = "Laptop 12", Price = Random.Shared.Next(100, 400), Processor = "Inter Core i5"},
    new Laptop { Id = Guid.CreateVersion7(), Name = "Laptop 5 ", Price = Random.Shared.Next(100, 400), Processor = "Inter Core i5"},
    new Laptop { Id = Guid.CreateVersion7(), Name = "Laptop 3 ", Price = Random.Shared.Next(100, 400), Processor = "Inter Core i5"},
    new Laptop { Id = Guid.CreateVersion7(), Name = "Laptop 11", Price = Random.Shared.Next(100, 400), Processor = "Inter Core i5"}
];


foreach (var laptop in laptops)
{
    if (await service.CreateAsync(laptop))
    {
        Console.WriteLine("Laptop created successfully");
    }
}

if (await service.ReadAsync(laptops.First().Id) != null)
{
    Console.WriteLine("Laptop read successfully");
}

if (await service.RemoveAsync(laptops.First()))
{
    Console.WriteLine("Laptop removed successfully");
}

foreach (var laptop in await service.ReadAllAsync())
{
    Console.WriteLine($"Laptop read successfully {laptop.Id} {laptop.Name}");
}
