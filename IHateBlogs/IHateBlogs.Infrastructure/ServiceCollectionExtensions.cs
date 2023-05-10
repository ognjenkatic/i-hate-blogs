using IHateBlogs.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Infrastructure.Cache;

namespace IHateBlogs.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterPersistence(services, configuration);

            return services;
        }

        private static void RegisterPersistence(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database") ?? throw new Exception("Connection string not found.");
            services.AddDbContext<BlogDbContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                }
            );

            services.AddSingleton<ICacheService>(new CacheService());

            services.AddScoped<IBlogDbContext>(p => p.GetRequiredService<BlogDbContext>());
        }
    }
}
