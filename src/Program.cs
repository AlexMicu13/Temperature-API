using Backend.Config;
using Backend.Models;
using Backend.Repositories;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var settings = new ServerConfig();
builder.Configuration.Bind(settings);
var context = new BackendContext(settings.MongoDB);

// Add services to the container.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tema2 SPRC Micu Alexandru API", Version = "v1" });
});

builder.Services.AddSingleton<IBackendContext>(context);
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<ITownsRepository, TownsRepository>();
builder.Services.AddScoped<ITemperaturesRepository, TemperaturesRepository>();
builder.Services.AddSingleton<IMongoClient, MongoClient>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tema2 SPRC Micu Alexandru API");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
