using LocationTracker.API.Hubs;
using LocationTracker.Core.Interfaces.Services;
using LocationTracker.Core.Mapping;
using LocationTracker.Core.Services;
using LocationTracker.Core.UseCases;
using LocationTracker.Domain.Interfaces;
using LocationTracker.Infrastructure.Data;
using LocationTracker.Infrastructure.Messaging;
using LocationTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Location Tracker API", Version = "v1" });
});

builder.Services.AddSignalR();

builder.Services.AddDbContext<TrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IngestLocationUseCase>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, typeof(MappingProfile));builder.Services.AddHostedService<RabbitConsumerService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location Tracker API");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<LocationHub>("/locationHub");
});

app.Run();