using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IHateBlogs.Application.Queries
{
    public class GetPostsQuery : IRequest<List<Post>>
    {
        public class Handler : IRequestHandler<GetPostsQuery, List<Post>>
        {
            private readonly IBlogDbContext dbContext;

            public Handler(IBlogDbContext dbContext)
            {
                this.dbContext = dbContext;
            }


            public Task<List<Post>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
            {
                // not going to bother with pagination for now
                return dbContext.Posts.OrderByDescending(p => p.CreatedAt)
                    .ToListAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
