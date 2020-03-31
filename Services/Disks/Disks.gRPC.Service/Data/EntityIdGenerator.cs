using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Data
{
    public static class EntityIdGenerator
    {
        private const string PREFIX = "vol";
        private const int HASH_LENGHT = 7;

        public static string Create()
        {
            Guid guid = Guid.NewGuid();
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(guid.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return MakeFormat(PREFIX, sb.ToString());
        }

        private static string MakeFormat(string prefix, string hash)
        {
            var shortHash = string.Concat(hash.Take(HASH_LENGHT));
            return string.Format("{0}-{1,7}", prefix, shortHash).ToLower();
        }
    }
}
