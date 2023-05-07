namespace IHateBlogs.Domain.Entities
{
   
    public class Post
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public required string Content { get; set; }
        public List<Tag> Tags { get; set; } = new();
        public required Requester Requester { get; set; }
        public PostState State { get; set; } = PostState.Requested;
        public enum PostState
        {
            Requested,
            BeingWritten,
            Completed,
            Abandoned
        }
    }
}
