using IHateBlogs.Application.Commands;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Application.Queries;
using IHateBlogs.Domain.Entities;
using Markdig;
using Markdig.Prism;
using MediatR;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace IHateBlogs.Pages
{
    public class PostModel : PageModel
    {
        private readonly IMediator mediator;
        public HtmlString MarkdownContent { get; set; }
        public string? ReadTime { get; set; }
        public string? State { get; set; }
        public string? Tone { get; set; }
        public string? Audience { get; set; }
        public string? Requester { get; set; }
        public string? FinishedAt { get; set; }
        public string? PostId { get; set; }
        public PostModel(IMediator mediator)
        {
            this.mediator = mediator;
            MarkdownContent = new HtmlString("<p>...</p>");
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePrism()
                .Build();

            var post = await mediator.Send(new GetPostQuery { Id = id});

            
            if (post is null)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ReadTime = string.Format("{0:0.##} minutes to read", (post.Content.Length / 4.7f) / 183);
                State = post.State.ToString();
                Tone = string.Join(',', post.Tags.Where(t => t.Kind == Domain.Entities.Tag.TagKind.Tone).Select(t => t.Name));
                Audience = string.Join(',', post.Tags.Where(t => t.Kind == Domain.Entities.Tag.TagKind.Audience).Select(t => t.Name));
                Requester = post.Requester.Name;
                MarkdownContent = new HtmlString(Markdown.Parse(post.Content, pipeline).ToHtml());
                FinishedAt = post.State == Post.PostState.Completed ? post.WritingStoppedAt.ToString("D") : "";
                PostId = post.Id.ToString();
                return Page();
            }
        }
    }
}
