using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Queries
{
    public class TagBundle
    {
        public required List<Tag> Audience { get; set; }
        public required List<Tag> Tones { get; set; }
        public required List<Tag> Subjects { get; set; }
    }
    public class GetTagsQuery : IRequest<TagBundle>
    {
        public class Handler : IRequestHandler<GetTagsQuery, TagBundle>
        {
            private readonly IBlogDbContext dbContext;

            public Handler(IBlogDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<TagBundle> Handle(GetTagsQuery request, CancellationToken cancellationToken)
            {
                return new TagBundle
                {
                    Audience = await dbContext.Tags.Where(t => t.Kind == Tag.TagKind.Audience).ToListAsync(cancellationToken: cancellationToken),
                    Subjects = await dbContext.Tags.Where(t => t.Kind == Tag.TagKind.Subject).ToListAsync(cancellationToken: cancellationToken),
                    Tones = await dbContext.Tags.Where(t => t.Kind == Tag.TagKind.Tone).ToListAsync(cancellationToken: cancellationToken)
                };
            }
        }
    }
}
