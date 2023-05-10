using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Common.Interfaces
{
    public interface IPostCompletionService
    {
        Task Update(Guid id, string chunk);
        Task Refresh(Guid id);
    }
}
