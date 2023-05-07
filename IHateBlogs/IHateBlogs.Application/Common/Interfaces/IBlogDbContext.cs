using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IHateBlogs.Application.Common.Interfaces
{
    public interface IBlogDbContext
    {
        DbSet<Post> Posts { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Requester> Requesters { get; set; }
        Task SaveChangesAsync();
    }
}
