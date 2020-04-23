using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Instances.API.Certificate
{
    public class Certificate
    {
        public static X509Certificate2 Get()
        {
            var path = Path.Combine("Certificate", "localhost.pfx");
            return new X509Certificate2(ReadFile(path), "lion");
        }

        private static byte[] ReadFile(string fileName)
        {
            FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();
            return data;
        }
    }
}
