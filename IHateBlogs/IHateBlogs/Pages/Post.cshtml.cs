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
        public Post Post { get; set; }
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
            this.Post = post;
            if (post is null)
            {
                return RedirectToPage("Index");
            }
            else
            {
                MarkdownContent = new HtmlString(Markdown.Parse(post.Content, pipeline).ToHtml());
                return Page();
            }
        }
    }
}
