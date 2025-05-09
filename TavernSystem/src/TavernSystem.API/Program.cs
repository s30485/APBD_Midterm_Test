using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using TavernSystem.Repositories.DTOs;
using TavernSystem.Repositories.Interfaces;
using TavernSystem.Repositories.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DB");

builder.Services.AddSingleton<IAdventurerService, AdventurerService>(s => new AdventurerService(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/adventurers", async (IAdventurerService service) =>
{
    var result = await service.GetAllAdventurersAsync();
    return result.Count == 0 ? Results.NoContent() : Results.Ok(result);
});

app.MapGet("/api/adventurers/{id:int}", async (int id, IAdventurerService service) =>
{
    var result = await service.GetAdventurerByIdAsync(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/api/adventurers", async (CreateAdventurerDTO dto, IAdventurerService service) =>
{
    try
    {
        var result = await service.CreateAdventurerAsync(dto);
        return result ? Results.Created("/api/adventurers", dto) : Results.Conflict("Adventurer already exists for this person.");
    }
    catch (InvalidOperationException ex)
    {
        return Results.StatusCode(403); //person has bounty or not found
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});



app.Run();
