using Microsoft.AspNetCore.SignalR;

namespace IHateBlogs.Hubs
{
    public class PostCompletionHub : Hub
    {
        public async Task WatchPost(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId);
        }

        public async Task StopWatchingPost(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
        }
    }
}
