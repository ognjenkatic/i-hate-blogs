using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IHateBlogs.Infrastructure.Persistence.Configuration
{
    public class RequesterConfiguration : IEntityTypeConfiguration<Requester>
    {
        public void Configure(EntityTypeBuilder<Requester> builder)
        {
            builder.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
        }
    }
}
