using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IHateBlogs.Application.Common.Util
{
    public static class HashUtil
    {
        public static string ComputeSha256Hash(string data)
         => Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(data)));
    }
}
