using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Disks.gRPC.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                         .ConfigureKestrel(serverOptions =>
                         {
                             serverOptions.AllowSynchronousIO = true;
                             serverOptions.ConfigureHttpsDefaults(listenOptions =>
                             {
                                // certificate is an X509Certificate2
                                listenOptions.ServerCertificate = Certificate.Certificate.Get();
                             });
                         })
                         .UseStartup<Startup>();
                });
    }
}
