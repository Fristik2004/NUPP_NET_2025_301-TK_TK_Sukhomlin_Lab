using Devices.Infrastructure.Contracts;
using Devices.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zoo.Rest;

namespace Devices.Rest.Controllers;

[Route("/laptops")]
public class LaptopController : Controller
{
    [HttpGet]
    public async Task<IResult> GetAll(
        [FromServices] ICrudServiceAsync<Laptop> service)
    {
        try
        {
            return Results.Ok(await service.ReadAllAsync());
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error!",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> Get(
        [FromRoute] Guid id,
        [FromServices] ICrudServiceAsync<Laptop> service)
    {
        try
        {
            var laptop = await service.ReadAsync(id);

            if (laptop == null)
            {
                return Results.NotFound($"Ноутбук з id {id} не знайдено");
            }

            return Results.Ok(laptop);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error!",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IResult> Create(
        [FromBody] Laptop value,
        [FromServices] ICrudServiceAsync<Laptop> service)
    {
        try
        {
            var result = await service.CreateAsync(value);

            if (result)
                return Results.Created($"/courses/{value.Id}", value);

            return Results.BadRequest();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error!",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPatch("{id:guid}")]
    public async Task<IResult> Update(
        [FromRoute] Guid id,
        [FromBody] Laptop value,
        [FromServices] ICrudServiceAsync<Laptop> service)
    {
        try
        {
            value.Id = id;

            var result = await service.UpdateAsync(value);

            if (result)
                return Results.Ok(value);

            return Results.BadRequest();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error!",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICrudServiceAsync<Laptop> service)
    {
        try
        {
            var found = await service.ReadAsync(id);

            if (found == null)
                return Results.NotFound();

            var result = await service.RemoveAsync(found);

            if (result)
                return Results.Ok();

            return Results.BadRequest();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error!",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
    
    [Authorize]
    [HttpPost("/role/{role}")]
    public async Task<IResult> Grant(
        [FromRoute] string role,
        [FromServices] UserManager<IdentityUser> userManager)
    {
        var user = await userManager.GetUserAsync(User);
        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
            return Results.Ok();
        }
            
        return Results.BadRequest();
    }
}