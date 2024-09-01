using IHateBlogs.Application.Common.Interfaces;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IHateBlogs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var openAiConfig = configuration.GetSection("OpenAi").Get<OpenAiConfiguration>() ?? throw new InvalidOperationException("Could not read OpenAi config");

            services.AddSingleton(openAiConfig);
            services.AddSignalR();
            services.AddSingleton<IPostCompletionService, PostCompletionService>();
            return services;
        }
    }
}