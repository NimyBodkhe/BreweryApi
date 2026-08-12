using Asp.Versioning;
using BreweryApi.Clients;
using BreweryApi.Interfaces;
using BreweryApi.Mappers;
using BreweryApi.Middleware;
using BreweryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IBreweryApiClient, BreweryApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IBreweryService, BreweryService>();
builder.Services.AddScoped<IBreweryMapper, BreweryMapper>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
