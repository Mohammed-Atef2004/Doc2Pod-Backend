<<<<<<< HEAD
﻿using Application.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Presistence.Data;
using Infrastructure.Presistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using Infrastructure.Services.PythonService.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
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

            //  4. Register Repositories 
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IPodcastRepository, PodcastRepository>();

            //  5. Register Services
            services.AddScoped<IFileStorageService, FileStorageService>();

            ///  6.Register Mapper
            services.AddAutoMapper(typeof(PythonMappingProfile));

            return services;
        }
    }
=======
﻿using Application.Features.Users.Services;
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
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName)); // ✅ JWT options

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
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // 8. Register Python RAG Service
        services.AddHttpClient<IPythonRagService, PythonRagService>(client =>
        {
            var baseUrl = configuration["PythonAI:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
        });

        return services;
    }
>>>>>>> master
}