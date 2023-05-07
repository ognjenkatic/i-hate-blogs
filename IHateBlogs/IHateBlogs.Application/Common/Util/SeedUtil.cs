using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Common.Util
{
    public static class SeedUtil
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IBlogDbContext>();

            var tones = EmbededResources.ReadResource(EmbededResources.BlogResource.Tone).Split('\n').ToList();
            var subjects = EmbededResources.ReadResource(EmbededResources.BlogResource.Subject).Split('\n').ToList();
            var audience = EmbededResources.ReadResource(EmbededResources.BlogResource.Audience).Split('\n').ToList();

            foreach (var tone in tones)
            {
                if (!await dbContext.Tags.AnyAsync(t => t.Name == tone))
                {
                    await dbContext.Tags.AddAsync(new Tag { Description = "", Kind = Tag.TagKind.Tone, Name = tone });
                }
            }

            foreach (var subject in subjects)
            {
                if (!await dbContext.Tags.AnyAsync(t => t.Name == subject))
                {
                    await dbContext.Tags.AddAsync(new Tag { Description = "", Kind = Tag.TagKind.Subject, Name = subject });
                }
            }

            foreach (var aud in audience)
            {
                if (!await dbContext.Tags.AnyAsync(t => t.Name == aud))
                {
                    await dbContext.Tags.AddAsync(new Tag { Description = "", Kind = Tag.TagKind.Audience, Name = aud });
                }
            }

            await dbContext.SaveChangesAsync();

        }
    }
}
