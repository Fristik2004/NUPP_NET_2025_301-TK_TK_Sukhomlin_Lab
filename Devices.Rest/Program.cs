using Devices.Infrastructure.Contracts;
using Devices.Infrastructure.DataContexts;
using Devices.Infrastructure.Models;
using Devices.Infrastructure.Repositories;
using Devices.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zoo.Rest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddOpenApiDocument(config =>
{
    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Name = "Authorization",
        Description = "Type 'Bearer' followed by a space and your token"
    });
    
    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

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

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DevicesContext>()
    .AddDefaultTokenProviders();

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

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = [Roles.Admin, Roles.Moderator];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.UseAuthentication();
app.UseAuthorization();

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
app.MapIdentityApi<IdentityUser>();

app.Run();
