using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace IHateBlogs.Service
{
    public class PostCompletionService : IPostCompletionService
    {
        private readonly IHubContext<PostCompletionHub> hubContext;

        public PostCompletionService(IHubContext<PostCompletionHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Refresh(Guid id)
        {
            await hubContext.Clients.Group(id.ToString()).SendAsync("RefreshPost");
        }

        public async Task Update(Guid id, string chunk)
        {
            await hubContext.Clients.Group(id.ToString()).SendAsync("UpdatePost", chunk);
        }
    }
}
