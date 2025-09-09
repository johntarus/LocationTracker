using LocationTracker.API.Hubs;
using LocationTracker.Core.Services;
using LocationTracker.Core.UseCases;
using LocationTracker.Domain.Interfaces;
using LocationTracker.Infrastructure.Data;
using LocationTracker.Infrastructure.Messaging;
using LocationTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Location Tracker API", Version = "v1" });
});

// Add SignalR
builder.Services.AddSignalR();

// Add DbContext
builder.Services.AddDbContext<TrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register dependencies
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IngestLocationUseCase>();

// Register RabbitMQ Consumer as a hosted service
builder.Services.AddHostedService<RabbitConsumerService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location Tracker API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseCors("AllowAll");
app.UseRouting(); // Add this line
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // Map SignalR hub
    endpoints.MapHub<LocationHub>("/locationHub");
});

app.Run();