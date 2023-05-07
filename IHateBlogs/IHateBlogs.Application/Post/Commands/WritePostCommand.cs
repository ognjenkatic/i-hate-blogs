using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using static IHateBlogs.Application.Common.Util.EmbededResources;

namespace IHateBlogs.Application.Commands
{
    public class WritePostCommand : IRequest
    {
        public required Guid Id { get; set; }
        public class Handler : IRequestHandler<WritePostCommand>
        {
            private readonly IBlogDbContext dbContext;
            private readonly OpenAiConfiguration configuration;

            public Handler(IBlogDbContext dbContext, OpenAiConfiguration configuration)
            {
                this.dbContext = dbContext;
                this.configuration = configuration;
            }

            public async Task Handle(WritePostCommand request, CancellationToken cancellationToken)
            {
                var post = await dbContext.Posts
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken)
                        ?? throw new Exception("No post exists with supplied id");

                try
                {
                    if (post.Tags == null || post.Tags.Count == 0)
                    {
                        throw new Exception("Cannot generate post without tags");
                    }

                    if (!post.Tags.Any(t => t.Kind == Tag.TagKind.Subject))
                    {
                        throw new Exception("Cannot generate post without subject tag");
                    }

                    if (!post.Tags.Any(t => t.Kind == Tag.TagKind.Tone))
                    {
                        throw new Exception("Cannot generate post without tone tag");
                    }

                    var prompt = ReadResource(BlogResource.Prompt, new()
                    {
                        { "{tone}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Tone).Select(t => t.Name).ToList())},
                        { "{subject}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Subject).Select(t => t.Name).ToList())},
                        { "{audience}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Audience).Select(t => t.Name).ToList())}
                    });

                    var api = new OpenAIAPI(configuration.ApiKey)
                    {
                        HttpClientFactory = new OpenAiHttpClientFactory()
                    };

                    var result = await api.Chat.CreateChatCompletionAsync(
                        new ChatRequest
                        {

                            Temperature = 0.7,
                            MaxTokens = 2048,
                            Model = Model.GPT4,
                            Messages = new List<ChatMessage>() { new ChatMessage { Content = prompt } }
                        });

                    var response = result.Choices[0].Message.Content.Trim();

                    if (string.IsNullOrEmpty(response))
                    {
                        post.State = Post.PostState.Abandoned;
                        throw new Exception("Something went wrong, openAI returned empty response");
                    }

                    post.Content = response;
                    post.State = Post.PostState.Completed;
                    

                }
                catch (Exception)
                {
                    post.State = Post.PostState.Abandoned;
                    throw;
                }
                finally
                {
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
