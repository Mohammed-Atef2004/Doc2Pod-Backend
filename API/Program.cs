using Application;
using Application.Interfaces;
using Domain.Interfaces.Services;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Presistence.Data;
using Infrastructure.Services;
using Infrastructure.Services.PythonService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Middlewares;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================
            // 1. Controllers & Swagger
            // ==========================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // ==========================
            // 2. Database & Core Services
            // ==========================
            // بنسجل الـ DbContext هنا عشان الـ Identity يحس بيه
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // تسجيل الـ Python Service (الخاصة بالـ Podcast Module)
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
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(
            //            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
            //    };
            //});

            builder.Services.AddAuthorization();
            builder.Configuration.AddUserSecrets<Program>();

            var app = builder.Build();

            // ==========================
            // 5. Middleware Pipeline
            // ==========================
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}