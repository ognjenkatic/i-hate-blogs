using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Common.Interfaces
{
    public interface ICacheService
    {
        T Set<T>(string key, T value);
        T? Get<T>(string key);
    }
}
