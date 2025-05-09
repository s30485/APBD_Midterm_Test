using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddSingleton<ICurrencyService, CurrencyService>(s => new CurrencyService(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/api/adventurers", async (CurrencyRequestDTO request, ICurrencyService service) =>
{
    try
    {
        //can validate here
        Validator.ValidateCurrency(request);
        var result = await service.AddCurrency(request);
        return result ? Results.NoContent() : Results.BadRequest();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/search", async (string type, string query, ICurrencyService service) =>
{
    try
    {
        var result = await service.SearchCurrency(type, query);
        return result is not null ? Results.Ok(result) : Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});