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
                        //Not a fan of the magic string here not sure if it can be another way.
                        if (Environments.Production == Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                        {
                            webBuilder.ConfigureAppConfiguration(config =>
                            {
                                var settings = config.Build();
                                config.AddAzureAppConfiguration(options => options.Connect(settings["ConnectionStrings:AppConfig"])
                                .UseFeatureFlags(featureFlagOptions => featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(5)));

                            });
                        }
                        webBuilder.UseStartup<Startup>();
                    });
        }
    }
}
