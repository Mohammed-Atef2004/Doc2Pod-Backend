using Application;
using Application.Interfaces;
using Domain.Interfaces.Services;
using EducationalPlatform.Infrastructure.Services.Token;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Presistence.Data;
using Infrastructure.Services;
using Infrastructure.Services.PythonService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebApi.Middlewares;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddUserSecrets<Program>();

            // ==========================
            // 1. Controllers & Swagger
            // ==========================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();



            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Doc2Pod API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter your JWT token like this: Bearer {your_token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                });




            builder.Services.AddLogging();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // ==========================
            // 2. Database & Core Services
            // ==========================
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHttpClient<IPythonRagService, PythonRagService>(client =>
            {
                var baseUrl = builder.Configuration["PythonService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl!);
                client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            });

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

            // ==========================
            // 3. Identity Setup
            // ==========================
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IIdentityService, IdentityService>();

            //// ==========================
            //// 4. JWT Authentication
            //// ==========================

            var jwtSection = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtOptions>(jwtSection);
            var jwtOptions = jwtSection.Get<JwtOptions>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200") 
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials(); 
                    });
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // ==========================
            // 5. Middleware Pipeline
            // ==========================
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}