using Devices.Infrastructure.Contracts;
using Devices.Infrastructure.DataContexts;
using Devices.Infrastructure.Models;
using Devices.Infrastructure.Repositories;
using Devices.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument();

var connection = builder.Configuration.GetConnectionString("SqlServerConnection")!;

builder.Services.AddDbContext<DevicesContext>(x => x
    .UseSqlServer(connection)
);

builder.Services.AddScoped<IRepository<Charger>, Repository<Charger>>();
builder.Services.AddScoped<IRepository<Smartphone>, Repository<Smartphone>>();
builder.Services.AddScoped<IRepository<Laptop>, Repository<Laptop>>();

builder.Services.AddScoped<ICrudServiceAsync<Charger>, AsyncCrudService<Charger>>();
builder.Services.AddScoped<ICrudServiceAsync<Smartphone>, AsyncCrudService<Smartphone>>();
builder.Services.AddScoped<ICrudServiceAsync<Laptop>, AsyncCrudService<Laptop>>();


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseOpenApi();
app.UseSwaggerUi();
app.UseReDoc(config =>
{
    config.Path = "/redoc";
    config.DocumentPath = "/swagger/v1/swagger.json";
});

app.MapControllers();

app.Run();
