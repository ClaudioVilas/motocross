using Motocross.Application.Interfaces;
using Motocross.Application.Services;
using Motocross.Infrastructure;
using Motocross.Infrastructure.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Motocross Tracking API",
        Version = "v1",
        Description = "Real-time motorsports tracking platform API"
    });
});

// Add Infrastructure (DbContext, Repositories, SignalR)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddScoped<ISessionService, SessionService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Vite dev server
                "http://localhost:3000",
                "https://*.vercel.app",  // Vercel deployments
                "https://*.vercel.app/",
                builder.Configuration["Frontend:Url"] ?? "http://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<TrackingHub>("/hubs/tracking");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.Run();
