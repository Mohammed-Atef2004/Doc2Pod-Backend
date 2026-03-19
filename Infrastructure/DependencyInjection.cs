using Application.Features.Users.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Users;
using EducationalPlatform.Infrastructure.Services.Token;
using Infrastructure.Presistence.Data;
using Infrastructure.Presistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Register Interceptor
        services.AddScoped<DomainEventInterceptor>();

        // 2. Register DbContext
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventInterceptor>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
            options.AddInterceptors(interceptor);
        });

        // 3. Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 4. Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IPodcastRepository, PodcastRepository>();

        // 5. Register Services
        services.AddScoped<ITotpService, TotpService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddHttpClient<IPythonRagService, PythonRagService>();

        return services; 
    }
}