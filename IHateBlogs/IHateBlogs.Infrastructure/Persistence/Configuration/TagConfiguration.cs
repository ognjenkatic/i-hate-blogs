using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IHateBlogs.Infrastructure.Persistence.Configuration
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.Kind).HasConversion<string>();
        }
    }
}
