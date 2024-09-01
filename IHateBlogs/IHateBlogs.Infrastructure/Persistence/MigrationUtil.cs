using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IHateBlogs.Infrastructure.Persistence
{
    public static class MigrationUtil
    {
        public static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            context.Database.Migrate();
        }
    }
}