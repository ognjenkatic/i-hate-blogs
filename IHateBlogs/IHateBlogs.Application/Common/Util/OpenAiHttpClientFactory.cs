using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Common.Util
{
    public class OpenAiHttpClientFactory : IHttpClientFactory
    {

        public OpenAiHttpClientFactory()
        {
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(15)
            };
        }
    }
}
