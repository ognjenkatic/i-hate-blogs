﻿using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Application.Models;
using IHateBlogs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Diagnostics;
using System.Formats.Tar;
using System.Text;
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

                var conversationResult = new ConversationContext
                {
                   Post = post
                };

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

                    post.State = Post.PostState.BeingWritten;
                    post.WritingStartedAt = DateTimeOffset.UtcNow;
                    await dbContext.SaveChangesAsync();

                    await RecurseToResult(prompt, conversationResult);

                    if (string.IsNullOrEmpty(conversationResult.Content.ToString()))
                    {
                        post.State = Post.PostState.Failed;
                        throw new Exception("Something went wrong, openAI returned empty response");
                    }

                    post.State = Post.PostState.Completed;
                }
                catch (Exception)
                {
                    post.State = Post.PostState.Incomplete;
                    throw;
                }
                finally
                {
                    post.Content = conversationResult.Content.ToString();
                    post.WritingStoppedAt = DateTimeOffset.UtcNow;
                    await dbContext.SaveChangesAsync();
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

                    var counter = 0;

                    await convo.StreamResponseFromChatbotAsync(async res =>
                    {
                        conversationResult.Content.Append(res);

                        if (counter % 5 == 0)
                        {
                            conversationResult.Post.Content = conversationResult.Content.ToString();
                            // it makes no sense saving this here, better save to to memory/cache while it's in progress
                            await dbContext.SaveChangesAsync();
                        }

                        counter++;
                        
                    });
                } catch (IOException)
                {
                    await RecurseToResult(prompt, conversationResult);
                }
            }
        }
    }
}
