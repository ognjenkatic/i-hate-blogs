namespace IHateBlogs.Domain.Entities
{
    public class Requester
    {
        public Guid Id { get; set; }
        public required string IpHash { get; set; }
        public required string Name { get; set; }
        public List<Post> Posts { get; set; } = new();
    }
}
