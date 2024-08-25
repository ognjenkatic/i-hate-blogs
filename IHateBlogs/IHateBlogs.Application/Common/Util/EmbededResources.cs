using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IHateBlogs.Application.Common.Util
{
    public static class EmbededResources
    {
        public enum BlogResource
        {
            Names,
            Titles,
            Prompt,
            Tone,
            Subject,
            Audience
        }

        private static string Read(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"IHateBlogs.Application.Resources.{resourceName}";

            using Stream stream = assembly.GetManifestResourceStream(resourcePath)!;
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        public static string ReadResource(BlogResource resource)
        {
            return resource switch
            {
                BlogResource.Names => Read("Names.md"),
                BlogResource.Prompt => Read("Prompt.md"),
                BlogResource.Titles => Read("Titles.md"),
                BlogResource.Tone => Read("Moods.md"),
                BlogResource.Subject => Read("Subjects.md"),
                BlogResource.Audience => Read("Levels.md"),
                _ => throw new InvalidOperationException("Unsupported resource")
            };
        }
        
        public static string ReadResource(BlogResource resource, Dictionary<string,string> parameters)
        {
            var value = ReadResource(resource);

            foreach (var parameter in parameters)
            {
                value = value.Replace(parameter.Key, parameter.Value);
            }

            return value;
        }

    }
}