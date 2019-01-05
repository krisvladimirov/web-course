using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyBlog.Data.Seeds;

namespace MyBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            InitializeDB(host);
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        private static void InitializeDB(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedData.InitializeAsync(services).Wait();
                }
                catch (Exception e)
                {
                    var logger = services
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Error while seeding to DB.");
                }
            }
        }
    }
}
