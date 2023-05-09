using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IHateBlogs.Application.Queries
{
    public class GetPostQuery : IRequest<Post?>
    {
        public required Guid Id { get; set; }
        public class Handler : IRequestHandler<GetPostQuery, Post?>
        {
            private readonly IBlogDbContext dbContext;

            public Handler(IBlogDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<Post?> Handle(GetPostQuery request, CancellationToken cancellationToken) 
                => await dbContext.Posts
                .Include(p => p.Requester)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
        }
    }
}
