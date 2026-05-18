using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Motocross.Application.Interfaces;
using Motocross.Domain.Abstractions;
using Motocross.Domain.Services;
using Motocross.Infrastructure.Persistence;
using Motocross.Infrastructure.Persistence.Repositories;
using Motocross.Infrastructure.Realtime;

namespace Motocross.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MotocrossDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Domain Services
        services.AddScoped<ILapDetectionService, LapDetectionService>();

        // Real-time
        services.AddSignalR();
        services.AddScoped<IRealtimePublisher, RealtimeTrackingPublisher>();

        return services;
    }
}
