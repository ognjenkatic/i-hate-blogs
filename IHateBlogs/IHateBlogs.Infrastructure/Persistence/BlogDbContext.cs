using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IHateBlogs.Infrastructure.Persistence
{
    public class BlogDbContext : DbContext, IBlogDbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Requester> Requesters { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
    }
}
