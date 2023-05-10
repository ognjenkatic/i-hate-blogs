using IHateBlogs.Domain.Entities;
using System.Text;

namespace IHateBlogs.Application.Models
{
    public class ConversationContext
    {
        public Post Post { get; set; }
        public StringBuilder Content { get; set; } = new();
    }
}
