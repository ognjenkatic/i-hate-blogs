using IHateBlogs.Application.Commands;
using IHateBlogs.Application.Queries;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IHateBlogs.Pages
{
    public class RequestModel : PageModel
    {
        private readonly IMediator mediator;

        [BindProperty]
        public string? SelectedTones { get; set; }
        [BindProperty]
        public string? SelectedSubjects { get; set; }
        [BindProperty]
        public string? SelectedAudiences { get; set; }
        public TagBundle? TagBundle { get; set; }

        public RequestModel(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task OnGet()
        {
            TagBundle = await mediator.Send(new GetTagsQuery());
        }

        public async Task<IActionResult> OnPost()    
        {
            var post = await mediator.Send(
                new CreatePostCommand
                {
                    Tags = $"{SelectedTones};{SelectedSubjects};{SelectedAudiences}",
                    RequesterIp = HttpContext.Connection.RemoteIpAddress!.ToString()
                }
            );

            await mediator.Send(new WritePostCommand
            {
                Id = post
            });

            return RedirectToPage("Index");
        }    
    }
}
