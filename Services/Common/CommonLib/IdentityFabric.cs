using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib
{
    public static class IdentityFabric
    {
        private const int HASH_LENGHT = 7;

        private const string VOLUME_PREFIX = "vol";
        private const string INSTANCE_PREFIX = "i";
        
        public static string GenInstanceId()
        {
            return GenerateId(VOLUME_PREFIX);
        }

        public static string GenVolumeId()
        {
            return GenerateId(INSTANCE_PREFIX);
        }

        private static string GenerateId(string prefix)
        {
            Guid guid = Guid.NewGuid();
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(guid.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return MakeFormat(prefix, sb.ToString());
        }

        private static string MakeFormat(string prefix, string hash)
        {
            var shortHash = string.Concat(hash.Take(HASH_LENGHT));
            return string.Format("{0}-{1}", prefix, shortHash).ToLower();
        }
    }
}


