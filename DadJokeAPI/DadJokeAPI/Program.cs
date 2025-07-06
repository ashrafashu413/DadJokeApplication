using Domain.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.Extensions.Options;
using Middleware;
using Nest;
using Services.Implementations;
using Services.Interfaces;
using Utilities;
using Validators;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from environment or local
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables();

// Register settings
builder.Services.Configure<DadJokeSettings>(builder.Configuration.GetSection("DadJokeSettings"));

builder.Services.Configure<ElasticSearchSettings>(builder.Configuration.GetSection("ElasticSearch"));

builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ElasticSearchSettings>>().Value;

    var connectionSettings = new ConnectionSettings(new Uri(settings.Uri))
        .DefaultIndex(settings.DefaultIndex)
        .EnableDebugMode(); // Optional for debugging

    return new ElasticClient(connectionSettings);
});


// ✅ Add wildcard CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Services
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IJokeRepository, JokeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJokeService, JokeService>();
builder.Services.AddScoped<IHttpUtility, Utilities.HttpUtility>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("DadJokeClient");

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<SearchTermValidator>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();
// ✅ Use the policy
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
