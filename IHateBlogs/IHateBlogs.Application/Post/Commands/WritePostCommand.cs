using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Application.Models;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            private readonly IPostCompletionService completionService;

            public Handler(IBlogDbContext dbContext, OpenAiConfiguration configuration, IPostCompletionService completionService)
            {
                this.dbContext = dbContext;
                this.configuration = configuration;
                this.completionService = completionService;
            }

            public async Task Handle(WritePostCommand request, CancellationToken cancellationToken)
            {
                var post = await dbContext.Posts
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken)
                        ?? throw new InvalidOperationException("No post exists with supplied id");

                var conversationResult = new ConversationContext
                {
                   Post = post
                };

                try
                {
                    if (post.Tags == null || post.Tags.Count == 0)
                    {
                        throw new InvalidOperationException("Cannot generate post without tags");
                    }

                    if (!post.Tags.Exists(t => t.Kind == Tag.TagKind.Subject))
                    {
                        throw new InvalidOperationException("Cannot generate post without subject tag");
                    }

                    if (!post.Tags.Exists(t => t.Kind == Tag.TagKind.Tone))
                    {
                        throw new InvalidOperationException("Cannot generate post without tone tag");
                    }

                    var prompt = ReadResource(BlogResource.Prompt, new()
                    {
                        { "{tone}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Tone).Select(t => t.Name).ToList())},
                        { "{subject}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Subject).Select(t => t.Name).ToList())},
                        { "{audience}", string.Join(',', post.Tags.Where(t => t.Kind == Tag.TagKind.Audience).Select(t => t.Name).ToList())}
                    });

                    post.State = Post.PostState.BeingWritten;
                    post.WritingStartedAt = DateTimeOffset.UtcNow;
                    await dbContext.SaveChangesAsync();

                    await RecurseToResult(prompt, conversationResult);

                    if (string.IsNullOrEmpty(conversationResult.Content.ToString()))
                    {
                        post.State = Post.PostState.Failed;
                        throw new InvalidOperationException("Something went wrong, openAI returned empty response");
                    }

                    post.State = Post.PostState.Completed;
                }
                catch (InvalidOperationException)
                {
                    post.State = Post.PostState.Incomplete;
                    throw;
                }
                finally
                {
                    post.Content = conversationResult.Content.ToString();
                    post.WritingStoppedAt = DateTimeOffset.UtcNow;
                    await dbContext.SaveChangesAsync();
                    await completionService.Refresh(post.Id);
                }
            }

            private async Task RecurseToResult(string prompt, ConversationContext conversationResult)
            {
                try
                {
                    var api = new OpenAIAPI(configuration.ApiKey)
                    {
                        HttpClientFactory = new OpenAiHttpClientFactory()
                    };

                    var convo = api.Chat.CreateConversation(new ChatRequest
                    {
                        Temperature = 0.7,
                        MaxTokens = 2048,
                        Model = Model.GPT4
                    });

                    convo.AppendUserInput($"{prompt}{conversationResult.Content}");

                    await convo.StreamResponseFromChatbotAsync(async res =>
                    {
                        conversationResult.Content.Append(res);
                        await completionService.Update(conversationResult.Post.Id, res);

                    });
                } catch (IOException)
                {
                    await RecurseToResult(prompt, conversationResult);
                }
            }
        }
    }
}
