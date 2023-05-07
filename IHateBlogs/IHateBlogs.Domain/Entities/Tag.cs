namespace IHateBlogs.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set;}
        public required string Name { get; set;}
        public required string Description { get; set;}
        public List<Post> Posts { get; set; } = new();
        public required TagKind Kind { get; set; }
        public enum TagKind
        {
            Tone,
            Subject,
            Audience
        }
    }
}
