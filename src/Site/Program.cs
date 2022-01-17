using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace FeatureFlags.Site
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureLogging(x => x.ClearProviders())
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureAppConfiguration(config =>
                         {
                             var settings = config.Build();
                             var conn = settings.GetConnectionString("AppConfig");
                             config.AddAzureAppConfiguration(options => options.Connect(conn)
                             .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5)));

                         });

                        webBuilder.UseStartup<Startup>();
                    });
        }
    }
}
