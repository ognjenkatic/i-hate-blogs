using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IHateBlogs.Application.Common.Util.EmbededResources;

namespace IHateBlogs.Application.Queries
{
    public class CreateOrGetRequesterQuery : IRequest<Requester>
    {
        public required string IpHash { get; set; }
        public class Handler : IRequestHandler<CreateOrGetRequesterQuery, Requester>
        {
            private static readonly Random Random = new();
            private readonly IBlogDbContext dbContext;

            public Handler(IBlogDbContext dbContext)
            {
                this.dbContext = dbContext;
            }
            public async Task<Requester> Handle(CreateOrGetRequesterQuery request, CancellationToken cancellationToken)
            {
                return await dbContext.Requesters
                    .FirstOrDefaultAsync(r => r.IpHash == request.IpHash, cancellationToken: cancellationToken) 
                    ?? await AddNewRequester(request.IpHash, cancellationToken);
            }

            private async Task<Requester> AddNewRequester(string ipHash, CancellationToken cancellationToken)
            {
                var name = GetRandomName();

                // will not bother locking this..
                while (await dbContext.Requesters.AnyAsync(r => r.Name == name, cancellationToken: cancellationToken))
                {
                    name = GetRandomName();
                }

                var requester = new Requester
                {
                    IpHash = ipHash,
                    Name = name
                };

                await dbContext.Requesters.AddAsync(requester, cancellationToken);
                await dbContext.SaveChangesAsync();

                return requester;
            }

            private static string GetRandomName()
            {
                var names = ReadResource(BlogResource.Names).Split('\n').ToList();
                var titles = ReadResource(BlogResource.Titles).Split('\n').ToList();

                var firstName = names[Random.Next(names.Count)];
                var secondName = names[Random.Next(names.Count)];
                var title = titles[Random.Next(titles.Count)];

                return $"{title} {firstName} {secondName}";
            }
        }

        
    }
}
