using CarSalesManagementAPI.API.Middleware;
using CarSalesManagementAPI.Application.Mapping;
using CarSalesManagementAPI.Application.Services;
using CarSalesManagementAPI.Application.Validators;
using CarSalesManagementAPI.Domain.Interfaces;
using CarSalesManagementAPI.Infrastructure.Data;
using CarSalesManagementAPI.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCarModelDtoValidator>();

// AutoMapper - Manual configuration to avoid ambiguity
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton<IMapper>(mapper);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Car Sales Management API",
        Version = "v1",
        Description = "API for managing car models and generating commission reports"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Static Files (for image serving)
builder.Services.AddDirectoryBrowser();

// Dependency Injection
// Infrastructure
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Repositories
builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Services
builder.Services.AddScoped<ICarModelService, CarModelService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICacheService, CacheService>();

// HttpContextAccessor (for future authentication)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Sales Management API v1");
    });
}

// Global Error Handling Middleware (should be early in pipeline)
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngularApp");

// Static Files for image serving
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
