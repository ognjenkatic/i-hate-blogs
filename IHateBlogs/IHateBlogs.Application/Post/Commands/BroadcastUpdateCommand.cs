using MediatR;

namespace IHateBlogs.Application.Commands
{
    public record BroadcastUpdateCommand(string Chunk, Guid PostId) : IRequest
    {
        public class Handler : IRequestHandler<BroadcastUpdateCommand>
        {
            public Task Handle(BroadcastUpdateCommand request, CancellationToken cancellationToken)
            {
                Console.Write(request.Chunk);
                return Task.CompletedTask;
            }
        }
    }
}
