using Application;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Presistence.Data;
using Infrastructure.Services.PythonService;
using Microsoft.EntityFrameworkCore;
using WebApi.Middlewares;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddHttpClient<IPythonRagService, PythonRagService>(client =>
            {
                var baseUrl = builder.Configuration["PythonService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;

            });


            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}