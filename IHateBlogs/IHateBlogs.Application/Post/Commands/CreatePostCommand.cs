using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Application.Queries;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static IHateBlogs.Application.Common.Util.EmbededResources;

namespace IHateBlogs.Application.Commands
{
    public class CreatePostCommand : IRequest<Guid>
    {
        public required string RequesterIp { get; set; }
        public required string Tags { get; set; }
        public class Handler : IRequestHandler<CreatePostCommand, Guid>
        {
            private readonly IBlogDbContext dbContext;
            private readonly IMediator mediator;

            public Handler(IBlogDbContext dbContext, IMediator mediator)
            {
                this.dbContext = dbContext;
                this.mediator = mediator;
            }

            public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                var ipHash = HashUtil.ComputeSha256Hash(request.RequesterIp);

                var requester = await mediator.Send(new CreateOrGetRequesterQuery { IpHash = ipHash });

                var tagList = request.Tags.Split(';');

                var tags = await dbContext.Tags.Where(t => tagList.Contains(t.Name)).ToListAsync(cancellationToken: cancellationToken);

                if (tags.Count != tagList.Length)
                {
                    throw new Exception("Invalid tags supplied");
                }

                if (!tags.Any(t => t.Kind == Tag.TagKind.Subject))
                {
                    throw new Exception("Cannot generate post without subject tag");
                }

                if (!tags.Any(t => t.Kind == Tag.TagKind.Tone))
                {
                    throw new Exception("Cannot generate post without tone tag");
                }

                if (!tags.Any(t => t.Kind == Tag.TagKind.Audience))
                {
                    throw new Exception("Cannot generate post without audience tag");
                }

                var post = new Post
                {
                    Requester = requester,
                    Tags = tags,
                    Content = "...",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await dbContext.Posts.AddAsync(post, cancellationToken);
                await dbContext.SaveChangesAsync();

                return post.Id;
            }

            
        }
    }
}
