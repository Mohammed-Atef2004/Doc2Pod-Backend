using Application.Features.Users.Services;
using Application.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Settings;
using Domain.Users;
using EducationalPlatform.Infrastructure.Services.Token;
using Infrastructure.Identity;
using Infrastructure.Presistence.Data;
using Infrastructure.Presistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using Infrastructure.Services.PythonService;
using Infrastructure.Services.PythonService.Mapping;
using Microsoft.AspNetCore.Identity;
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

        // 3. Register Identity with token providers
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // 4. Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 5. Register Settings
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // 6. Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IPodcastRepository, PodcastRepository>();

        // 7. Register Services
        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITotpService, TotpService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddDataProtection();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // 8. Register Python RAG Service & AI Integration
        services.AddHttpClient<IPythonRagService, PythonRagService>(client =>
        {
            var baseUrl = configuration["PythonAI:BaseUrl"] ?? "http://localhost:5000";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
        });

        // 9. Register Mapper (AutoMapper)
        services.AddAutoMapper(typeof(PythonMappingProfile));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}