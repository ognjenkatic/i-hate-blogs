using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IHateBlogs.Infrastructure.Persistence.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.HasMany(p => p.Tags)
                .WithMany(t => t.Posts);

            builder.HasOne(p => p.Requester)
                .WithMany(r => r.Posts);

            builder.Property(p => p.State).HasConversion<string>();
        }
    }
}
