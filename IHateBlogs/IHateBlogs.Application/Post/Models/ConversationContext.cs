using IHateBlogs.Domain.Entities;
using System.Text;

namespace IHateBlogs.Application.Models
{
    public class ConversationContext
    {
        public int TotalCompletionTokens { get; set; }
        public Post Post { get; set; }
        public StringBuilder Content { get; set; } = new();
    }
}
